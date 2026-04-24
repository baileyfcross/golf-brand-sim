using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class SponsorshipMarketScreen : IScreen
{
    private static readonly int[] ColumnWidths = [190, 55, 55, 55, 95, 95, 80];

    private int _selectedGolferIndex;
    private OfferField _selectedField = OfferField.WeeklyRetainer;

    private decimal _offerWeeklyRetainer;
    private decimal _offerSigningBonus;
    private decimal _offerShareRate;
    private int _offerDurationWeeks = 36;

    private Guid? _offerTargetGolferId;
    private string _feedback = "CLICK A GOLFER, ADJUST TERMS, THEN CLICK SUBMIT";

    public string TabLabel => "MARKET";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
        var available = GetAvailableGolfers(session);
        if (available.Count == 0)
            return;

        if (_selectedGolferIndex >= available.Count)
            _selectedGolferIndex = available.Count - 1;

        var selectedGolfer = available[_selectedGolferIndex];
        EnsureOfferInitialized(selectedGolfer, session);

        if (input.IsNewLeftClick())
        {
            if (TrySelectGolferFromClick(input.MousePosition, bounds, available.Count))
            {
                selectedGolfer = available[_selectedGolferIndex];
                EnsureOfferInitialized(selectedGolfer, session);
            }

            if (TryHandleOfferEditingClick(input.MousePosition, bounds))
                return;

            if (GetSubmitButtonBounds(bounds).Contains(input.MousePosition))
                SubmitOffer(session, selectedGolfer);
        }
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "SPONSORSHIP MARKET");

        var available = GetAvailableGolfers(session);
        if (available.Count == 0)
        {
            ui.DrawText("NO AVAILABLE FREE AGENTS THIS WEEK", new Vector2(bounds.X + 18, bounds.Y + 56), Theme.TextMuted, 2);
            return;
        }

        var tableRows = available
            .Take(18)
            .Select(g =>
            {
                var ask = GolferMarketValueService.Calculate(g, session.State.SeasonStandings);
                return new[]
                {
                    g.FullName.ToUpperInvariant(),
                    g.Overall.ToString(),
                    g.Popularity.ToString(),
                    g.Marketability.ToString(),
                    Formatters.Money(ask.MinWeeklyRetainer),
                    Formatters.Money(ask.MinSigningBonus),
                    Formatters.Percent(ask.MinWinningsShare)
                };
            })
            .ToArray();

        UiToolkit.DrawTable(
            ui,
            GetMarketTableBounds(bounds),
            ["GOLFER", "OVR", "POP", "MKT", "ASK/WK", "ASK BONUS", "ASK SHARE"],
            ColumnWidths,
            tableRows,
            _selectedGolferIndex);

        var selectedGolfer = available[_selectedGolferIndex];
        var askValue = GolferMarketValueService.Calculate(selectedGolfer, session.State.SeasonStandings);

        var termsBounds = GetTermsBounds(bounds);
        ui.FillRectangle(termsBounds, Theme.PanelRaised);
        ui.DrawBorder(termsBounds, Theme.PanelBorder, 2);

        DrawOfferLine(ui, termsBounds, OfferField.WeeklyRetainer, $"WEEKLY RETAINER: {Formatters.Money(_offerWeeklyRetainer)}", askValue.MinWeeklyRetainer);
        DrawOfferLine(ui, termsBounds, OfferField.SigningBonus, $"SIGNING BONUS: {Formatters.Money(_offerSigningBonus)}", askValue.MinSigningBonus);
        DrawOfferLine(ui, termsBounds, OfferField.ShareRate, $"WIN SHARE: {Formatters.Percent(_offerShareRate)}", askValue.MinWinningsShare);
        DrawOfferLine(ui, termsBounds, OfferField.Duration, $"DURATION WEEKS: {_offerDurationWeeks}", 0m);

        var submitBounds = GetSubmitButtonBounds(bounds);
        ui.FillRectangle(submitBounds, Theme.Accent);
        ui.DrawBorder(submitBounds, Theme.AccentHighlight, 2);
        ui.DrawCenteredText("SUBMIT OFFER", submitBounds, Theme.Header, 2);
        ui.DrawText(_feedback, new Vector2(bounds.X + 28, bounds.Bottom - 58), Theme.TextPrimary, 1);
    }

    private bool TrySelectGolferFromClick(Point mousePosition, Rectangle bounds, int golferCount)
    {
        var tableBounds = GetMarketTableBounds(bounds);
        if (!tableBounds.Contains(mousePosition))
            return false;

        const int headerHeight = 32;
        const int rowHeight = 28;

        var relativeY = mousePosition.Y - tableBounds.Y - headerHeight;
        if (relativeY < 0)
            return false;

        var rowIndex = relativeY / rowHeight;
        if (rowIndex < 0 || rowIndex >= Math.Min(18, golferCount))
            return false;

        _selectedGolferIndex = rowIndex;
        return true;
    }

    private bool TryHandleOfferEditingClick(Point mousePosition, Rectangle bounds)
    {
        foreach (var field in Enum.GetValues<OfferField>())
        {
            var rowBounds = GetOfferFieldRowBounds(bounds, field);
            if (!rowBounds.Contains(mousePosition))
                continue;

            _selectedField = field;
            if (GetAdjustButtonBounds(bounds, field, true).Contains(mousePosition))
                AdjustField(field, +1);
            else if (GetAdjustButtonBounds(bounds, field, false).Contains(mousePosition))
                AdjustField(field, -1);

            return true;
        }

        return false;
    }

    private void SubmitOffer(GameSession session, Golfer golfer)
    {
        var offer = new ContractOffer(
            Guid.NewGuid(),
            golfer.Id,
            _offerSigningBonus,
            _offerWeeklyRetainer,
            _offerShareRate,
            _offerDurationWeeks,
            session.State.CurrentWeekNumber);

        var result = session.EvaluateOffer(offer, golfer);
        _feedback = result.Message;
    }

    private void DrawOfferLine(UiContext ui, Rectangle bounds, OfferField field, string text, decimal ask)
    {
        var rowBounds = GetOfferFieldRowBounds(bounds, field);
        var color = _selectedField == field ? Theme.AccentHighlight : Theme.TextPrimary;
        ui.DrawText(text, new Vector2(rowBounds.X + 12, rowBounds.Y + 6), color, 1);

        if (ask > 0m && field is not OfferField.Duration)
        {
            var askText = field == OfferField.ShareRate ? Formatters.Percent(ask) : Formatters.Money(ask);
            ui.DrawText($"ASK {askText}", new Vector2(rowBounds.Right - 230, rowBounds.Y + 6), Theme.TextMuted, 1);
        }

        var minusBounds = GetAdjustButtonBounds(bounds, field, false);
        var plusBounds = GetAdjustButtonBounds(bounds, field, true);

        ui.FillRectangle(minusBounds, Theme.Panel);
        ui.DrawBorder(minusBounds, Theme.PanelBorder, 1);
        ui.DrawCenteredText("-", minusBounds, Theme.TextPrimary, 2);

        ui.FillRectangle(plusBounds, Theme.Panel);
        ui.DrawBorder(plusBounds, Theme.PanelBorder, 1);
        ui.DrawCenteredText("+", plusBounds, Theme.TextPrimary, 2);
    }

    private void EnsureOfferInitialized(Golfer golfer, GameSession session)
    {
        if (_offerTargetGolferId == golfer.Id)
            return;

        var marketValue = GolferMarketValueService.Calculate(golfer, session.State.SeasonStandings);
        _offerTargetGolferId = golfer.Id;
        _offerWeeklyRetainer = marketValue.MinWeeklyRetainer;
        _offerSigningBonus = marketValue.MinSigningBonus;
        _offerShareRate = marketValue.MinWinningsShare;
        _offerDurationWeeks = 36;
    }

    private void AdjustField(OfferField field, int direction)
    {
        switch (field)
        {
            case OfferField.WeeklyRetainer:
                _offerWeeklyRetainer = Math.Max(0m, _offerWeeklyRetainer + direction * 100m);
                break;
            case OfferField.SigningBonus:
                _offerSigningBonus = Math.Max(0m, _offerSigningBonus + direction * 1_000m);
                break;
            case OfferField.ShareRate:
                _offerShareRate = Math.Clamp(_offerShareRate + direction * 0.01m, 0.01m, 0.35m);
                break;
            case OfferField.Duration:
                _offerDurationWeeks = Math.Clamp(_offerDurationWeeks + direction, 8, 104);
                break;
        }
    }

    private static Rectangle GetMarketTableBounds(Rectangle bounds)
    {
        var tableBottom = GetTermsBounds(bounds).Y - 8;
        return new Rectangle(bounds.X + 16, bounds.Y + 52, bounds.Width - 32, tableBottom - (bounds.Y + 52));
    }

    private static Rectangle GetTermsBounds(Rectangle bounds)
    {
        return new Rectangle(bounds.X + 16, bounds.Bottom - 148, bounds.Width - 32, 172);
    }

    private static Rectangle GetOfferFieldRowBounds(Rectangle bounds, OfferField field)
    {
        var termsBounds = GetTermsBounds(bounds);
        var index = (int)field;
        return new Rectangle(termsBounds.X + 4, termsBounds.Y + 4 + index * 22, termsBounds.Width - 8, 20);
    }

    private static Rectangle GetAdjustButtonBounds(Rectangle bounds, OfferField field, bool increment)
    {
        var rowBounds = GetOfferFieldRowBounds(bounds, field);
        var x = increment ? rowBounds.Right - 28 : rowBounds.Right - 56;
        return new Rectangle(x, rowBounds.Y + 1, 26, 18);
    }

    private static Rectangle GetSubmitButtonBounds(Rectangle bounds)
    {
        return new Rectangle(bounds.Right - 220, bounds.Bottom - 56, 200, 34);
    }

    private static List<Golfer> GetAvailableGolfers(GameSession session)
    {
        var state = session.State;
        var week = state.CurrentWeekNumber;
        var takenByPlayer = state.PlayerBrand.Contracts
            .Where(c => c.IsActiveForWeek(week))
            .Select(c => c.GolferId)
            .ToHashSet();

        var takenByCompetitors = state.CompetitorBrands
            .SelectMany(b => b.SponsoredGolferIds)
            .ToHashSet();

        return state.Golfers
            .Where(g => !takenByPlayer.Contains(g.Id) && !takenByCompetitors.Contains(g.Id))
            .OrderByDescending(g => g.Overall)
            .ThenByDescending(g => g.Marketability)
            .ToList();
    }

    private enum OfferField
    {
        WeeklyRetainer,
        SigningBonus,
        ShareRate,
        Duration
    }
}

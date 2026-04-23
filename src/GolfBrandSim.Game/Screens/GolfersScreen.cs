using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GolfBrandSim.Game.Screens;

public sealed class GolfersScreen : IScreen
{
    private static readonly int[] ColumnWidths = [290, 80, 90, 90, 90, 120, 120];
    private const int SortButtonWidth = 170;
    private const int SortButtonHeight = 30;

    private GolferSortField _sortField = GolferSortField.Skill;

    public string TabLabel => "GOLFERS";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
        if (input.IsNewKeyPress(Keys.S))
        {
            _sortField = GolferSortField.Skill;
        }
        else if (input.IsNewKeyPress(Keys.P))
        {
            _sortField = GolferSortField.Popularity;
        }
        else if (input.IsNewKeyPress(Keys.C))
        {
            _sortField = GolferSortField.ContractShare;
        }

        if (input.IsNewLeftClick())
        {
            TrySetSortFromButtonClick(input.MousePosition, bounds);
            TrySetSortFromClick(input.MousePosition, bounds);
        }
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "GOLFER DATABASE");
        ui.DrawText("CLICK SORT BUTTONS OR TABLE HEADERS", new Vector2(bounds.X + 18, bounds.Y + 18), Theme.TextMuted, 2);

        DrawSortButtons(ui, bounds);

        var contracts = session.State.PlayerBrand.Contracts.ToDictionary(contract => contract.GolferId);
        var orderedGolfers = OrderGolfers(session.State.Golfers, contracts);
        var rows = orderedGolfers
            .Select(golfer => BuildRow(golfer, contracts))
            .ToArray();

        UiToolkit.DrawTable(
            ui,
            GetTableBounds(bounds),
            ["NAME", "CTR", "RATING", "CONS", "POP", "SPONSORED", "SHARE"],
            ColumnWidths,
            rows);
    }

    private void DrawSortButtons(UiContext ui, Rectangle bounds)
    {
        DrawSortButton(ui, GetSortButtonBounds(bounds, 0), "SORT RATING", _sortField == GolferSortField.Skill);
        DrawSortButton(ui, GetSortButtonBounds(bounds, 1), "SORT POPULARITY", _sortField == GolferSortField.Popularity);
        DrawSortButton(ui, GetSortButtonBounds(bounds, 2), "SORT SHARE", _sortField == GolferSortField.ContractShare);
    }

    private static void DrawSortButton(UiContext ui, Rectangle buttonBounds, string label, bool active)
    {
        var fill = active ? Theme.Accent : Theme.PanelRaised;
        var text = active ? Theme.Header : Theme.TextPrimary;
        ui.FillRectangle(buttonBounds, fill);
        ui.DrawBorder(buttonBounds, active ? Theme.AccentHighlight : Theme.PanelBorder, 2);
        ui.DrawCenteredText(label, buttonBounds, text, 2);
    }

    private void TrySetSortFromButtonClick(Point point, Rectangle bounds)
    {
        if (GetSortButtonBounds(bounds, 0).Contains(point))
        {
            _sortField = GolferSortField.Skill;
            return;
        }

        if (GetSortButtonBounds(bounds, 1).Contains(point))
        {
            _sortField = GolferSortField.Popularity;
            return;
        }

        if (GetSortButtonBounds(bounds, 2).Contains(point))
        {
            _sortField = GolferSortField.ContractShare;
        }
    }

    private void TrySetSortFromClick(Point point, Rectangle bounds)
    {
        var tableBounds = GetTableBounds(bounds);
        var headerBounds = new Rectangle(tableBounds.X, tableBounds.Y, tableBounds.Width, 32);
        if (!headerBounds.Contains(point))
        {
            return;
        }

        var cursorX = tableBounds.X;
        for (var index = 0; index < ColumnWidths.Length; index++)
        {
            var cellBounds = new Rectangle(cursorX, tableBounds.Y, ColumnWidths[index], 32);
            if (!cellBounds.Contains(point))
            {
                cursorX += ColumnWidths[index];
                continue;
            }

            _sortField = index switch
            {
                2 => GolferSortField.Skill,
                4 => GolferSortField.Popularity,
                6 => GolferSortField.ContractShare,
                _ => _sortField
            };

            return;
        }
    }

    private static Rectangle GetTableBounds(Rectangle bounds)
    {
        return new Rectangle(bounds.X + 16, bounds.Y + 98, bounds.Width - 32, bounds.Height - 114);
    }

    private static Rectangle GetSortButtonBounds(Rectangle bounds, int index)
    {
        return new Rectangle(bounds.X + 16 + index * (SortButtonWidth + 12), bounds.Y + 52, SortButtonWidth, SortButtonHeight);
    }

    private IEnumerable<Golfer> OrderGolfers(IReadOnlyList<Golfer> golfers, IReadOnlyDictionary<Guid, SponsorshipContract> contracts)
    {
        return _sortField switch
        {
            GolferSortField.Popularity => golfers.OrderByDescending(golfer => golfer.Popularity).ThenByDescending(golfer => golfer.SkillRating),
            GolferSortField.ContractShare => golfers.OrderByDescending(golfer => contracts.TryGetValue(golfer.Id, out var contract) ? contract.WinningsShareRate : -1m)
                .ThenByDescending(golfer => golfer.SkillRating),
            _ => golfers.OrderByDescending(golfer => golfer.SkillRating).ThenByDescending(golfer => golfer.Consistency)
        };
    }

    private static string[] BuildRow(Golfer golfer, IReadOnlyDictionary<Guid, SponsorshipContract> contracts)
    {
        var sponsored = contracts.TryGetValue(golfer.Id, out var contract);

        return
        [
            golfer.FullName.ToUpperInvariant(),
            golfer.CountryCode,
            golfer.SkillRating.ToString(),
            golfer.Consistency.ToString(),
            golfer.Popularity.ToString(),
            sponsored ? "YES" : "NO",
            sponsored ? Formatters.Percent(contract!.WinningsShareRate) : "OPEN"
        ];
    }

    private enum GolferSortField
    {
        Skill,
        Popularity,
        ContractShare
    }
}
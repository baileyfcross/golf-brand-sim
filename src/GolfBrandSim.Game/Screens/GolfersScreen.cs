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
            TrySetSortFromClick(input.MousePosition, bounds);
        }
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "GOLFER DATABASE");
        ui.DrawText("SORT S RATING  P POPULARITY  C CONTRACT SHARE", new Vector2(bounds.X + 18, bounds.Y + 18), Theme.TextMuted, 2);

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
        return new Rectangle(bounds.X + 16, bounds.Y + 60, bounds.Width - 32, bounds.Height - 76);
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
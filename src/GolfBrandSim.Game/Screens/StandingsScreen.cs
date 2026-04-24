using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class StandingsScreen : IScreen
{
    public string TabLabel => "STANDINGS";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "SEASON STANDINGS");

        var state = session.State;
        var golferMap = state.Golfers.ToDictionary(g => g.Id);
        var contractedIds = state.PlayerBrand.Contracts
            .Where(c => c.IsActiveForWeek(state.CurrentWeekNumber))
            .Select(c => c.GolferId)
            .ToHashSet();

        var ranked = state.SeasonStandings.GetRankedList();
        var rows = ranked
            .Select(entry =>
            {
                golferMap.TryGetValue(entry.Stats.GolferId, out var golfer);
                var name = golfer?.FullName.ToUpperInvariant() ?? entry.Stats.GolferId.ToString()[..8];
                var country = golfer?.CountryCode ?? "??";
                var sponsored = contractedIds.Contains(entry.Stats.GolferId) ? "* " : "  ";
                return new[]
                {
                    entry.Rank.ToString(),
                    sponsored + name,
                    country,
                    entry.Stats.Points.ToString(),
                    entry.Stats.EventsPlayed.ToString(),
                    entry.Stats.CutsMade.ToString(),
                    entry.Stats.Wins.ToString(),
                    entry.Stats.MajorWins.ToString(),
                    entry.Stats.Top10s.ToString(),
                    Formatters.Money(entry.Stats.Earnings)
                };
            })
            .ToArray();

        var headerNote = "  * = YOUR SPONSORED GOLFER";
        ui.DrawText(headerNote, new Vector2(bounds.X + 18, bounds.Y + 18), Theme.TextMuted, 2);

        UiToolkit.DrawTable(
            ui,
            new Rectangle(bounds.X + 16, bounds.Y + 52, bounds.Width - 32, bounds.Height - 68),
            ["RANK", "GOLFER", "CTR", "POINTS", "EVT", "CUT", "WINS", "MAJ", "TOP10", "EARNINGS"],
            [56, 220, 50, 72, 50, 50, 58, 50, 58, 116],
            rows);
    }
}

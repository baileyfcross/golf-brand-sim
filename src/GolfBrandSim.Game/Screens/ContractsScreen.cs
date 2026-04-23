using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class ContractsScreen : IScreen
{
    public string TabLabel => "CONTRACTS";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "ACTIVE CONTRACTS");

        var state = session.State;
        var week = state.CurrentWeekNumber;
        var golferMap = state.Golfers.ToDictionary(g => g.Id);

        var rows = state.PlayerBrand.Contracts
            .Where(c => c.IsActiveForWeek(week))
            .Select(c =>
            {
                golferMap.TryGetValue(c.GolferId, out var golfer);
                var name = golfer?.FullName.ToUpperInvariant() ?? c.GolferId.ToString()[..8];
                var country = golfer?.CountryCode ?? "??";
                var ovr = golfer?.Overall.ToString() ?? "-";
                var duration = c.EndWeek - c.StartWeek + 1;
                var dealType = duration <= 20 ? "PROSPECT" : duration <= 38 ? "BALANCED" : "STAR";
                var retainer = Formatters.Money(c.WeeklyRetainer);
                var share = Formatters.Percent(c.WinningsShareRate);
                var weeksLeft = Math.Max(0, c.EndWeek - week + 1).ToString();
                var earned = "N/A";
                return new[] { name, country, ovr, dealType, retainer, share, weeksLeft, earned };
            })
            .ToArray();

        UiToolkit.DrawTable(
            ui,
            new Rectangle(bounds.X + 16, bounds.Y + 52, bounds.Width - 32, bounds.Height - 68),
            ["GOLFER", "CTR", "OVR", "DEAL TYPE", "RETAINER/WK", "SHARE", "WKS LEFT", "EARNED"],
            [220, 50, 50, 110, 130, 80, 90, 130],
            rows);
    }
}

using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class TournamentResultsScreen : IScreen
{
    public string TabLabel => "RESULTS";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "TOURNAMENT RESULTS");

        if (session.State.LastWeekResult is null)
        {
            ui.DrawText("NO TOURNAMENT PLAYED YET", new Vector2(bounds.X + 24, bounds.Y + 72), Theme.TextPrimary, 3);
            ui.DrawText("PRESS SPACE TO SIMULATE THE NEXT WEEK", new Vector2(bounds.X + 24, bounds.Y + 112), Theme.TextMuted, 2);
            return;
        }

        var weekResult = session.State.LastWeekResult;
        var tournament = weekResult.TournamentResult.Tournament;
        var winner = weekResult.TournamentResult.Winner;

        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 16, bounds.Y + 52, 340, 110), tournament.Name, winner.Golfer.FullName.ToUpperInvariant(), $"WINNING SCORE {winner.TotalScore}");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 376, bounds.Y + 52, 220, 110), "CUT", weekResult.TournamentResult.CutScore.ToString(), "AFTER ROUND TWO");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 616, bounds.Y + 52, 220, 110), "SPONSOR INCOME", Formatters.Money(weekResult.SponsorshipIncome), "THIS WEEK");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 856, bounds.Y + 52, 220, 110), "PRODUCT PROFIT", Formatters.Money(weekResult.ProductProfit), "THIS WEEK");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 1096, bounds.Y + 52, 220, 110), "NET CASH", Formatters.Money(weekResult.NetCashChange), Formatters.WeekLabel(weekResult.WeekNumber));

        var rows = weekResult.TournamentResult.Standings
            .Take(12)
            .Select(standing => BuildRow(standing, session.State))
            .ToArray();

        UiToolkit.DrawTable(
            ui,
            new Rectangle(bounds.X + 16, bounds.Y + 190, bounds.Width - 32, bounds.Height - 206),
            ["POS", "GOLFER", "TOTAL", "PAYOUT", "SPONSORED"],
            [90, 320, 120, 160, 160],
            rows);
    }

    private static string[] BuildRow(TournamentStanding standing, GameState state)
    {
        var sponsored = state.PlayerBrand.Contracts.Any(contract => contract.GolferId == standing.Golfer.Id);
        return
        [
            $"P{standing.Place}",
            standing.Golfer.FullName.ToUpperInvariant(),
            standing.MadeCut ? standing.TotalScore.ToString() : $"MC {standing.TotalScore}",
            standing.PrizeMoney <= 0m ? "NO PAY" : Formatters.Money(standing.PrizeMoney),
            sponsored ? "YES" : "NO"
        ];
    }
}
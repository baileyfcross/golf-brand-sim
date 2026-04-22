using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class DashboardScreen : IScreen
{
    public string TabLabel => "DASHBOARD";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        var state = session.State;
        var nextTournament = state.NextTournament;
        var lastWeek = state.LastWeekResult;

        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X, bounds.Y, 250, 120), "CASH", Formatters.Money(state.PlayerBrand.CashBalance), "WORKING CAPITAL");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 270, bounds.Y, 320, 120), "NEXT EVENT", nextTournament?.Name ?? "SEASON COMPLETE", nextTournament?.IsMajor == true ? "MAJOR WEEK" : "STANDARD WEEK");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 610, bounds.Y, 220, 120), "SPONSORED", state.PlayerBrand.Contracts.Count.ToString(), "ACTIVE CONTRACTS");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 850, bounds.Y, 220, 120), "PRODUCTS", state.PlayerBrand.Products.Count.ToString(), state.PlayerBrand.Specialization.ToString().ToUpperInvariant());
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 1090, bounds.Y, 250, 120), "LAST NET", lastWeek is null ? "NO RESULT" : Formatters.Money(lastWeek.NetCashChange), lastWeek is null ? "SIMULATE FIRST WEEK" : Formatters.WeekLabel(lastWeek.WeekNumber));

        var contractsBounds = new Rectangle(bounds.X, bounds.Y + 150, 760, bounds.Height - 150);
        UiToolkit.DrawPanel(ui, contractsBounds, "SPONSORED GOLFERS");

        var contractRows = state.PlayerBrand.Contracts
            .Select(contract => BuildContractRow(contract, state))
            .ToArray();

        UiToolkit.DrawTable(
            ui,
            new Rectangle(contractsBounds.X + 16, contractsBounds.Y + 52, contractsBounds.Width - 32, contractsBounds.Height - 68),
            ["NAME", "RATING", "SHARE", "RETAINER", "LAST FINISH"],
            [240, 90, 110, 130, 140],
            contractRows);

        var researchBounds = new Rectangle(bounds.X + 780, bounds.Y + 150, bounds.Width - 780, bounds.Height - 150);
        UiToolkit.DrawPanel(ui, researchBounds, "CATEGORY RESEARCH");

        var researchRows = state.PlayerBrand.ResearchTracks
            .Select(track => new[]
            {
                track.Category.ToString().ToUpperInvariant(),
                track.IsUnlocked ? "OPEN" : $"{track.ProgressPoints} OF {track.UnlockThreshold}",
                state.PlayerBrand.HasProductInCategory(track.Category) ? "LIVE" : "LOCKED"
            })
            .ToArray();

        UiToolkit.DrawTable(
            ui,
            new Rectangle(researchBounds.X + 16, researchBounds.Y + 52, researchBounds.Width - 32, 180),
            ["CATEGORY", "PROGRESS", "STATUS"],
            [180, 170, 120],
            researchRows);

        var notesBounds = new Rectangle(researchBounds.X + 16, researchBounds.Y + 260, researchBounds.Width - 32, researchBounds.Height - 276);
        UiToolkit.DrawPanel(ui, notesBounds, "WEEKLY SNAPSHOT");

        var winner = lastWeek?.TournamentResult.Winner;
        var lines = new[]
        {
            lastWeek is null ? "SIMULATE A WEEK TO GENERATE TOUR RESULTS" : $"LAST EVENT {lastWeek.TournamentResult.Tournament.Name}",
            lastWeek is null ? "PLAYER BRAND STARTS WITH ONE SPECIALIZATION OPEN" : $"WINNER {winner?.Golfer.FullName.ToUpperInvariant()} {winner?.TotalScore}",
            lastWeek is null ? "RESEARCH WILL UNLOCK NEW PRODUCT CATEGORIES" : $"SPONSOR INCOME {Formatters.Money(lastWeek.SponsorshipIncome)}",
            lastWeek is null ? "SPACE ADVANCES DIRECTLY INTO RESULTS SCREEN" : $"PRODUCT PROFIT {Formatters.Money(lastWeek.ProductProfit)}"
        };

        for (var index = 0; index < lines.Length; index++)
        {
            ui.DrawText(lines[index], new Vector2(notesBounds.X + 18, notesBounds.Y + 56 + index * 28), Theme.TextPrimary, 2);
        }
    }

    private static string[] BuildContractRow(SponsorshipContract contract, GameState state)
    {
        var golfer = state.Golfers.Single(entry => entry.Id == contract.GolferId);
        var standing = state.LastWeekResult?.TournamentResult.Standings.FirstOrDefault(entry => entry.Golfer.Id == golfer.Id);

        return
        [
            golfer.FullName.ToUpperInvariant(),
            golfer.SkillRating.ToString(),
            Formatters.Percent(contract.WinningsShareRate),
            Formatters.Money(contract.WeeklyRetainer),
            standing is null ? "PENDING" : $"P{standing.Place}"
        ];
    }
}
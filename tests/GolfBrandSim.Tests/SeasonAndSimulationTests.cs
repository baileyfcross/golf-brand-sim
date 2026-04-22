using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Infrastructure.Seed;

namespace GolfBrandSim.Tests;

public sealed class SeasonAndSimulationTests
{
    [Fact]
    public void SeasonGenerator_CreatesThirtySixWeekCalendar_WithFourMajors()
    {
        var generator = new SeasonGenerator();

        var schedule = generator.Generate(2026, 24);

        Assert.Equal(36, schedule.Tournaments.Count);
        Assert.Equal(new[] { 8, 16, 25, 34 }, schedule.Tournaments.Where(tournament => tournament.IsMajor).Select(tournament => tournament.WeekNumber));
    }

    [Fact]
    public void TournamentSimulator_IsDeterministicForSeed_AndAppliesCutAfterRoundTwo()
    {
        var golfers = FictionalGolferSeedData.CreateGolfers();
        var tournament = new Tournament(1, "TEST EVENT", "TEST CLUB", TournamentType.Standard, 8_200_000m, golfers.Count);

        var firstResult = new TournamentSimulator(77).Simulate(tournament, golfers);
        var secondResult = new TournamentSimulator(77).Simulate(tournament, golfers);

        Assert.Equal(firstResult.Winner.Golfer.FullName, secondResult.Winner.Golfer.FullName);
        Assert.Equal(firstResult.Standings.Select(standing => standing.TotalScore), secondResult.Standings.Select(standing => standing.TotalScore));
        Assert.Contains(firstResult.Standings, standing => !standing.MadeCut);
        Assert.All(firstResult.Standings.Where(standing => !standing.MadeCut), standing => Assert.Equal(2, standing.RoundScores.Count));
        Assert.All(firstResult.Standings.Where(standing => standing.MadeCut), standing => Assert.Equal(4, standing.RoundScores.Count));
    }

    [Fact]
    public void GameSession_AdvanceWeek_UpdatesStateAndLedger()
    {
        var session = InitialGameStateFactory.Create(20260422);

        var result = session.AdvanceWeek();

        Assert.Equal(2, session.State.CurrentWeekNumber);
        Assert.Single(session.State.CompletedWeeks);
        Assert.Same(result, session.State.LastWeekResult);
        Assert.NotEmpty(session.State.FinanceLedger.Entries.Where(entry => entry.WeekNumber == 1));
        Assert.Equal(result.EndingCashBalance, session.State.PlayerBrand.CashBalance);
    }
}
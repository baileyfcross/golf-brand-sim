using GolfBrandSim.Core.Finance;

namespace GolfBrandSim.Core.Domain;

public sealed class GameState
{
    public GameState(Brand playerBrand, IReadOnlyList<Golfer> golfers, SeasonSchedule seasonSchedule, FinanceLedger financeLedger)
    {
        PlayerBrand = playerBrand;
        Golfers = golfers;
        SeasonSchedule = seasonSchedule;
        FinanceLedger = financeLedger;
    }

    public Brand PlayerBrand { get; }

    public IReadOnlyList<Golfer> Golfers { get; }

    public SeasonSchedule SeasonSchedule { get; }

    public FinanceLedger FinanceLedger { get; }

    public IList<WeekSimulationResult> CompletedWeeks { get; } = new List<WeekSimulationResult>();

    public int CurrentWeekNumber { get; private set; } = 1;

    public WeekSimulationResult? LastWeekResult { get; private set; }

    public bool IsSeasonComplete => CurrentWeekNumber > SeasonSchedule.Tournaments.Count;

    public Tournament? NextTournament =>
        IsSeasonComplete ? null : SeasonSchedule.GetTournamentForWeek(CurrentWeekNumber);

    public void RecordWeek(WeekSimulationResult result)
    {
        CompletedWeeks.Add(result);
        LastWeekResult = result;
        CurrentWeekNumber++;
    }
}
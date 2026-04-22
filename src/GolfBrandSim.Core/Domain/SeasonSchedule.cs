namespace GolfBrandSim.Core.Domain;

public sealed class SeasonSchedule
{
    public SeasonSchedule(int year, IEnumerable<Tournament> tournaments)
    {
        Year = year;
        Tournaments = tournaments.OrderBy(tournament => tournament.WeekNumber).ToList();
    }

    public int Year { get; }

    public IReadOnlyList<Tournament> Tournaments { get; }

    public Tournament GetTournamentForWeek(int weekNumber)
    {
        return Tournaments.Single(tournament => tournament.WeekNumber == weekNumber);
    }
}
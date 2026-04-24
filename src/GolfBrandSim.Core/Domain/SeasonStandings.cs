namespace GolfBrandSim.Core.Domain;

public sealed class SeasonStandings
{
    private readonly Dictionary<Guid, GolferSeasonStats> _stats = new();

    public IReadOnlyDictionary<Guid, GolferSeasonStats> Stats => _stats;

    public void RecordResult(TournamentResult result)
    {
        foreach (var standing in result.Standings)
        {
            if (!_stats.TryGetValue(standing.Golfer.Id, out var stats))
            {
                stats = new GolferSeasonStats(standing.Golfer.Id);
                _stats[standing.Golfer.Id] = stats;
            }

            stats.RecordFinish(standing.Place, standing.PrizeMoney, result.Tournament.IsMajor, standing.MadeCut);
        }
    }

    public void RestoreStats(
        Guid golferId,
        int points,
        int wins,
        int top10s,
        decimal earnings,
        int eventsPlayed = 0,
        int cutsMade = 0,
        int majorWins = 0,
        int bestFinish = 0,
        int lastFinish = 0)
    {
        _stats[golferId] = new GolferSeasonStats(
            golferId, points, wins, top10s, earnings,
            eventsPlayed, cutsMade, majorWins, bestFinish, lastFinish);
    }

    public IReadOnlyList<(GolferSeasonStats Stats, int Rank)> GetRankedList()
    {
        return _stats.Values
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.Earnings)
            .Select((stats, index) => (stats, index + 1))
            .ToList();
    }
}

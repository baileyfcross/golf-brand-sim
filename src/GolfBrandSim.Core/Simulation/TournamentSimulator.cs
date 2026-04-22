using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

public sealed class TournamentSimulator : ITournamentSimulator
{
    private readonly Random _random;

    private static readonly decimal[] PayoutPercentages =
    [
        0.18m,
        0.108m,
        0.068m,
        0.048m,
        0.04m,
        0.036m,
        0.0335m,
        0.031m,
        0.029m,
        0.027m,
        0.025m,
        0.023m
    ];

    public TournamentSimulator(int seed)
    {
        _random = new Random(seed);
    }

    public TournamentResult Simulate(Tournament tournament, IReadOnlyList<Golfer> golfers)
    {
        var field = golfers
            .OrderByDescending(golfer => golfer.SkillRating + golfer.Consistency * 0.15 + NextDouble(0, 5))
            .Take(Math.Min(tournament.FieldSize, golfers.Count))
            .ToList();

        var roundsByGolfer = new Dictionary<Guid, List<int>>(field.Count);
        foreach (var golfer in field)
        {
            roundsByGolfer[golfer.Id] =
            [
                SimulateRound(golfer, tournament.IsMajor),
                SimulateRound(golfer, tournament.IsMajor)
            ];
        }

        var cutIndex = Math.Max(8, field.Count / 2) - 1;
        var cutOrdered = field
            .Select(golfer => new { Golfer = golfer, Total = roundsByGolfer[golfer.Id].Sum() })
            .OrderBy(entry => entry.Total)
            .ThenByDescending(entry => entry.Golfer.SkillRating)
            .ToList();

        var cutScore = cutOrdered[cutIndex].Total;
        var golfersMakingCut = cutOrdered
            .Where(entry => entry.Total <= cutScore)
            .Select(entry => entry.Golfer)
            .ToHashSet();

        foreach (var golfer in field.Where(golfer => golfersMakingCut.Contains(golfer)))
        {
            roundsByGolfer[golfer.Id].Add(SimulateRound(golfer, tournament.IsMajor));
            roundsByGolfer[golfer.Id].Add(SimulateRound(golfer, tournament.IsMajor));
        }

        var orderedStandings = field
            .Select(golfer => BuildStanding(golfer, roundsByGolfer[golfer.Id], golfersMakingCut.Contains(golfer)))
            .OrderBy(standing => standing.TotalScore)
            .ThenByDescending(standing => standing.Golfer.SkillRating)
            .ToList();

        var standings = AssignPlacesAndPayouts(orderedStandings, tournament.Purse);
        return new TournamentResult(tournament, standings, cutScore);
    }

    private TournamentStanding BuildStanding(Golfer golfer, List<int> roundScores, bool madeCut)
    {
        return new TournamentStanding(golfer, 0, roundScores.ToArray(), madeCut, 0m);
    }

    private IReadOnlyList<TournamentStanding> AssignPlacesAndPayouts(IReadOnlyList<TournamentStanding> rawStandings, decimal purse)
    {
        var standings = new List<TournamentStanding>(rawStandings.Count);
        var currentPlace = 0;
        var displayedPlace = 0;
        int? priorScore = null;

        foreach (var rawStanding in rawStandings)
        {
            currentPlace++;
            if (priorScore != rawStanding.TotalScore)
            {
                displayedPlace = currentPlace;
            }

            priorScore = rawStanding.TotalScore;
            var payout = rawStanding.MadeCut && displayedPlace <= PayoutPercentages.Length
                ? decimal.Round(purse * PayoutPercentages[displayedPlace - 1], 2, MidpointRounding.AwayFromZero)
                : 0m;

            standings.Add(new TournamentStanding(
                rawStanding.Golfer,
                displayedPlace,
                rawStanding.RoundScores,
                rawStanding.MadeCut,
                payout));
        }

        return standings;
    }

    private int SimulateRound(Golfer golfer, bool majorSetup)
    {
        var baseline = 76.2 - golfer.SkillRating * 0.095;
        var variance = 1.4 + (100 - golfer.Consistency) * 0.02;
        var courseTax = majorSetup ? 1.2 : 0.0;
        var score = baseline + courseTax + NextGaussian() * variance;
        return (int)Math.Clamp(Math.Round(score), 65, 81);
    }

    private double NextDouble(double min, double max)
    {
        return min + _random.NextDouble() * (max - min);
    }

    private double NextGaussian()
    {
        var u1 = 1.0 - _random.NextDouble();
        var u2 = 1.0 - _random.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}
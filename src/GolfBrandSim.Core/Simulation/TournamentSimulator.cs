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
            .OrderByDescending(golfer => golfer.Overall + golfer.Consistency * 0.15 + NextDouble(0, 5))
            .Take(Math.Min(tournament.FieldSize, golfers.Count))
            .ToList();

        var roundsByGolfer = new Dictionary<Guid, List<int>>(field.Count);
        foreach (var golfer in field)
        {
            roundsByGolfer[golfer.Id] =
            [
                SimulateRound(golfer, tournament),
                SimulateRound(golfer, tournament)
            ];
        }

        var cutIndex = Math.Min(Math.Max(8, Math.Min(65, field.Count)) - 1, field.Count - 1);
        var cutOrdered = field
            .Select(golfer => new { Golfer = golfer, Total = roundsByGolfer[golfer.Id].Sum() })
            .OrderBy(entry => entry.Total)
            .ThenByDescending(entry => entry.Golfer.Overall)
            .ToList();

        var cutScore = cutOrdered[cutIndex].Total;
        var golfersMakingCut = cutOrdered
            .Where(entry => entry.Total <= cutScore)
            .Select(entry => entry.Golfer)
            .ToHashSet();

        foreach (var golfer in field.Where(golfer => golfersMakingCut.Contains(golfer)))
        {
            roundsByGolfer[golfer.Id].Add(SimulateRound(golfer, tournament));
            roundsByGolfer[golfer.Id].Add(SimulateRound(golfer, tournament));
        }

        var orderedStandings = field
            .Select(golfer => BuildStanding(golfer, roundsByGolfer[golfer.Id], golfersMakingCut.Contains(golfer)))
            .OrderBy(standing => standing.TotalScore)
            .ThenByDescending(standing => standing.Golfer.Overall)
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
        var paidSpots = rawStandings.Count(s => s.MadeCut);
        var payoutTable = BuildPayoutPercentages(paidSpots);
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
            var payout = rawStanding.MadeCut && displayedPlace <= payoutTable.Count
                ? decimal.Round(purse * payoutTable[displayedPlace - 1], 2, MidpointRounding.AwayFromZero)
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

    private static IReadOnlyList<decimal> BuildPayoutPercentages(int paidSpots)
    {
        if (paidSpots <= 0)
            return [];

        var values = new List<decimal>(paidSpots);
        for (var place = 1; place <= paidSpots; place++)
        {
            if (place <= PayoutPercentages.Length)
            {
                values.Add(PayoutPercentages[place - 1]);
                continue;
            }

            var tail = PayoutPercentages[^1] * (decimal)Math.Pow(0.92, place - PayoutPercentages.Length);
            values.Add(Math.Max(0.0002m, tail));
        }

        // Normalize to a realistic purse distribution pool.
        const decimal payoutPool = 0.80m;
        var total = values.Sum();
        if (total <= 0m)
            return values;

        return values.Select(v => v * payoutPool / total).ToArray();
    }

    private int SimulateRound(Golfer golfer, Tournament tournament)
    {
        var profile = tournament.CourseProfile;

        var totalDemand = profile.DistanceDemand + profile.AccuracyDemand +
                          profile.ApproachDemand + profile.ShortGameDemand + profile.PuttingDemand;

        var weightedSkill = totalDemand > 0
            ? (golfer.DrivingDistance * profile.DistanceDemand +
               golfer.DrivingAccuracy * profile.AccuracyDemand +
               golfer.Approach * profile.ApproachDemand +
               golfer.ShortGame * profile.ShortGameDemand +
               golfer.Putting * profile.PuttingDemand) / (double)totalDemand
            : golfer.Overall;

        var difficultyMod = profile.Difficulty * 0.15;
        var baseline = 76.2 - weightedSkill * 0.095 + difficultyMod;

        var volatilityMod = profile.Volatility * 0.07;
        var variance = 1.4 + (100 - golfer.Consistency) * 0.02 + volatilityMod;

        // Majors: low mentality golfers struggle under pressure
        if (tournament.IsMajor)
        {
            var mentalityPenalty = Math.Max(0, 85 - golfer.Mentality) * 0.010;
            baseline += mentalityPenalty;
        }

        var score = baseline + NextGaussian() * variance;
        return (int)Math.Clamp(Math.Round(score), 63, 82);
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

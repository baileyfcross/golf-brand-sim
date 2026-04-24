using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

/// <summary>Computes sponsorship-driven demand multiplier for active products.</summary>
public static class ProductLaunchService
{
    public static decimal CalculateDemandMultiplier(Brand brand, TournamentResult tournamentResult, GameState state, int weekNumber)
    {
        var sponsoredStandings = tournamentResult.Standings
            .Where(s => brand.Contracts.Any(c => c.GolferId == s.Golfer.Id && c.IsActiveForWeek(weekNumber)))
            .ToList();

        var brandHeat = 1.0m;
        if (sponsoredStandings.Count == 0)
            return brandHeat;

        var bestFinish = sponsoredStandings.Min(s => s.Place);
        brandHeat += Math.Max(0m, 0.20m - (bestFinish - 1) * 0.012m);

        var bestGolfer = sponsoredStandings.First(s => s.Place == bestFinish).Golfer;
        brandHeat += bestGolfer.Marketability * 0.0008m;

        var contractedIds = brand.Contracts
            .Where(c => c.IsActiveForWeek(weekNumber))
            .Select(c => c.GolferId)
            .ToHashSet();

        var seasonWins = contractedIds
            .Where(id => state.SeasonStandings.Stats.TryGetValue(id, out var s) && s.Wins > 0)
            .Sum(id => state.SeasonStandings.Stats[id].Wins);

        brandHeat += seasonWins * 0.015m;
        return brandHeat;
    }
}

using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

/// <summary>Calculates the minimum contract terms a golfer is willing to accept.</summary>
public sealed record GolferMarketValue(
    decimal MinWeeklyRetainer,
    decimal MinSigningBonus,
    decimal MinWinningsShare);

public static class GolferMarketValueService
{
    public static GolferMarketValue Calculate(Golfer golfer, SeasonStandings standings)
    {
        // Base weekly retainer scales with overall rating
        var baseWeekly = 500m + (decimal)Math.Pow(Math.Max(0.0, golfer.Overall - 60), 1.5) * 42m;

        // Popularity premium
        baseWeekly += golfer.Popularity * 12m;

        // In-season performance bonus
        if (standings.Stats.TryGetValue(golfer.Id, out var stats))
        {
            baseWeekly += stats.Wins * 250m;
            baseWeekly += stats.Top10s * 35m;
        }

        var weeklyMin = RoundToNearest(baseWeekly, 100m); // round to $100s
        var signingMin = RoundToNearest(weeklyMin * 4m, 1000m); // round to $1000s
        var shareMin = Math.Round(Math.Clamp(0.05m + (golfer.Overall - 60) * 0.003m, 0.05m, 0.25m), 2);

        return new GolferMarketValue(weeklyMin, signingMin, shareMin);
    }

    private static decimal RoundToNearest(decimal value, decimal increment)
    {
        if (increment <= 0m)
            return value;

        return Math.Round(value / increment, 0, MidpointRounding.AwayFromZero) * increment;
    }
}

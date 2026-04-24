using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

/// <summary>
/// Simulates CPU competitor brands signing available free agents each week.
/// Uses seeded randomness for determinism.
/// </summary>
public sealed class CompetitorSponsorshipService
{
    private readonly Random _random;

    public CompetitorSponsorshipService(Random random)
    {
        _random = random;
    }

    public void ProcessWeek(GameState state)
    {
        var week = state.CurrentWeekNumber;

        var takenByPlayer = state.PlayerBrand.Contracts
            .Where(c => c.IsActiveForWeek(week))
            .Select(c => c.GolferId)
            .ToHashSet();

        var takenByCompetitors = state.CompetitorBrands
            .SelectMany(b => b.SponsoredGolferIds)
            .ToHashSet();

        // Free agents sorted best-first; competitors prefer top talent
        var available = state.Golfers
            .Where(g => !takenByPlayer.Contains(g.Id) && !takenByCompetitors.Contains(g.Id))
            .OrderByDescending(g => g.Overall)
            .ToList();

        if (available.Count == 0) return;

        foreach (var brand in state.CompetitorBrands)
        {
            // Probability of signing attempt this week scales with aggression (1–5 = 8%–40%)
            if (_random.NextDouble() > brand.AggressionLevel * 0.08)
                continue;

            // Prefer candidates from the top tier, but allow some randomness
            var candidateCount = Math.Min(6, available.Count);
            var chosenIndex = _random.Next(0, candidateCount);
            var chosen = available[chosenIndex];

            brand.SignGolfer(chosen.Id);
            available.RemoveAt(chosenIndex);
        }
    }

    /// <summary>Runs multiple pre-season rounds so competitors start with initial rosters.</summary>
    public void SeedInitialRosters(GameState state, int rounds)
    {
        for (var i = 0; i < rounds; i++)
            ProcessWeek(state);
    }
}

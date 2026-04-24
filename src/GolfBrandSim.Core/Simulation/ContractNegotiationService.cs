using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

public sealed record ContractDecisionResult(
    bool IsAccepted,
    string Message,
    GolferMarketValue MarketValue);

/// <summary>Evaluates sponsorship offers from the player against a golfer's minimum expectations.</summary>
public sealed class ContractNegotiationService
{
    private readonly Random _random;

    public ContractNegotiationService(Random random)
    {
        _random = random;
    }

    public ContractDecisionResult Evaluate(
        ContractOffer offer,
        Golfer golfer,
        SeasonStandings standings,
        Brand brand,
        int currentWeek)
    {
        var market = GolferMarketValueService.Calculate(golfer, standings);

        // Reject immediately if already contracted
        if (brand.Contracts.Any(c => c.GolferId == golfer.Id && c.IsActiveForWeek(currentWeek)))
        {
            return new ContractDecisionResult(false, "ALREADY CONTRACTED", market);
        }

        // Reject immediately if player can't afford the signing bonus
        if (brand.CashBalance < offer.SigningBonus)
        {
            return new ContractDecisionResult(false, "INSUFFICIENT FUNDS FOR SIGNING BONUS", market);
        }

        // Score each component vs minimum (1.0 = meets minimum, >1.0 = exceeds)
        var retainerScore = market.MinWeeklyRetainer > 0
            ? (double)(offer.WeeklyRetainer / market.MinWeeklyRetainer)
            : 1.0;
        var bonusScore = market.MinSigningBonus > 0
            ? (double)(offer.SigningBonus / market.MinSigningBonus)
            : 1.0;
        var shareScore = market.MinWinningsShare > 0
            ? (double)(offer.WinningsShareRate / market.MinWinningsShare)
            : 1.0;

        var weightedScore = retainerScore * 0.55 + bonusScore * 0.25 + shareScore * 0.20;

        var acceptChance = weightedScore switch
        {
            >= 1.30 => 0.97,
            >= 1.10 => 0.85,
            >= 1.00 => 0.72,
            >= 0.85 => 0.45,
            >= 0.70 => 0.20,
            _ => 0.05
        };

        var accepted = _random.NextDouble() < acceptChance;

        if (accepted)
        {
            offer.Accept();
            return new ContractDecisionResult(true, "OFFER ACCEPTED!", market);
        }

        offer.Reject();
        var hint = weightedScore < 0.70
            ? "OFFER FAR BELOW EXPECTATIONS"
            : weightedScore < 0.85
                ? "OFFER BELOW MARKET VALUE"
                : "CLOSE — TRY A SLIGHTLY BETTER OFFER";

        return new ContractDecisionResult(false, $"OFFER REJECTED — {hint}", market);
    }
}

namespace GolfBrandSim.Core.Domain;

public static class ContractTemplates
{
    public static readonly ContractTemplate CheapProspect = new(
        "PROSPECT DEAL",
        DurationWeeks: 18,
        SigningBonus: 0m,
        AnnualRetainer: 48_000m,
        WinningsShareRate: 0.08m);

    public static readonly ContractTemplate BalancedTour = new(
        "BALANCED TOUR DEAL",
        DurationWeeks: 36,
        SigningBonus: 25_000m,
        AnnualRetainer: 120_000m,
        WinningsShareRate: 0.15m);

    public static readonly ContractTemplate AggressiveStar = new(
        "STAR DEAL",
        DurationWeeks: 52,
        SigningBonus: 80_000m,
        AnnualRetainer: 250_000m,
        WinningsShareRate: 0.22m);

    public static IReadOnlyList<ContractTemplate> All => [CheapProspect, BalancedTour, AggressiveStar];
}

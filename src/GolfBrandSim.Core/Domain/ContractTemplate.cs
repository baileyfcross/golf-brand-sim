namespace GolfBrandSim.Core.Domain;

public sealed record ContractTemplate(
    string Name,
    int DurationWeeks,
    decimal SigningBonus,
    decimal AnnualRetainer,
    decimal WinningsShareRate);

namespace GolfBrandSim.Core.Domain;

public sealed class SponsorshipContract
{
    public SponsorshipContract(
        Guid golferId,
        string contractName,
        decimal winningsShareRate,
        decimal annualRetainer,
        int startWeek,
        int endWeek)
    {
        GolferId = golferId;
        ContractName = contractName;
        WinningsShareRate = winningsShareRate;
        AnnualRetainer = annualRetainer;
        StartWeek = startWeek;
        EndWeek = endWeek;
    }

    public Guid GolferId { get; }

    public string ContractName { get; }

    public decimal WinningsShareRate { get; }

    public decimal AnnualRetainer { get; }

    public int StartWeek { get; }

    public int EndWeek { get; }

    public decimal WeeklyRetainer => decimal.Round(AnnualRetainer / 52m, 2, MidpointRounding.AwayFromZero);

    public bool IsActiveForWeek(int weekNumber)
    {
        return weekNumber >= StartWeek && weekNumber <= EndWeek;
    }

    public decimal CalculateBrandShare(decimal winnings)
    {
        return decimal.Round(winnings * WinningsShareRate, 2, MidpointRounding.AwayFromZero);
    }
}
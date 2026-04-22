using GolfBrandSim.Core.Finance;

namespace GolfBrandSim.Core.Domain;

public sealed class WeekSimulationResult
{
    public WeekSimulationResult(
        int weekNumber,
        TournamentResult tournamentResult,
        decimal productProfit,
        decimal sponsorshipIncome,
        decimal operatingExpense,
        decimal netCashChange,
        decimal endingCashBalance,
        IReadOnlyList<FinanceEntry> financeEntries)
    {
        WeekNumber = weekNumber;
        TournamentResult = tournamentResult;
        ProductProfit = productProfit;
        SponsorshipIncome = sponsorshipIncome;
        OperatingExpense = operatingExpense;
        NetCashChange = netCashChange;
        EndingCashBalance = endingCashBalance;
        FinanceEntries = financeEntries;
    }

    public int WeekNumber { get; }

    public TournamentResult TournamentResult { get; }

    public decimal ProductProfit { get; }

    public decimal SponsorshipIncome { get; }

    public decimal OperatingExpense { get; }

    public decimal NetCashChange { get; }

    public decimal EndingCashBalance { get; }

    public IReadOnlyList<FinanceEntry> FinanceEntries { get; }
}
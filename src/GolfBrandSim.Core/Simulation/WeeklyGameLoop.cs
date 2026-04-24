using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Finance;

namespace GolfBrandSim.Core.Simulation;

public sealed class WeeklyGameLoop
{
    private readonly ITournamentSimulator _tournamentSimulator;
    private readonly CompetitorSponsorshipService? _competitorSponsorshipService;

    public WeeklyGameLoop(
        ITournamentSimulator tournamentSimulator,
        CompetitorSponsorshipService? competitorSponsorshipService = null)
    {
        _tournamentSimulator = tournamentSimulator;
        _competitorSponsorshipService = competitorSponsorshipService;
    }

    public WeekSimulationResult Advance(GameState state)
    {
        if (state.IsSeasonComplete)
        {
            throw new InvalidOperationException("The season has already been completed.");
        }

        var weekNumber = state.CurrentWeekNumber;

        _competitorSponsorshipService?.ProcessWeek(state);

        var tournament = state.SeasonSchedule.GetTournamentForWeek(weekNumber);
        var tournamentResult = _tournamentSimulator.Simulate(tournament, state.Golfers);
        var financeEntries = new List<FinanceEntry>();

        // Record standings before applying financials
        state.SeasonStandings.RecordResult(tournamentResult);

        var sponsorshipIncome = ApplySponsorshipIncome(state.PlayerBrand, tournamentResult, weekNumber, financeEntries);
        var productProfit = ApplyProductRevenue(state.PlayerBrand, tournamentResult, state, weekNumber, financeEntries);
        var operatingExpense = ApplyOperatingExpense(state.PlayerBrand, weekNumber, financeEntries);

        state.FinanceLedger.AddRange(financeEntries);

        var netCashChange = financeEntries.Sum(entry => entry.Amount);
        state.PlayerBrand.ApplyCashChange(netCashChange);

        var result = new WeekSimulationResult(
            weekNumber,
            tournamentResult,
            productProfit,
            sponsorshipIncome,
            operatingExpense,
            netCashChange,
            state.PlayerBrand.CashBalance,
            financeEntries);

        state.RecordWeek(result);
        return result;
    }

    private static decimal ApplySponsorshipIncome(
        Brand brand,
        TournamentResult tournamentResult,
        int weekNumber,
        ICollection<FinanceEntry> financeEntries)
    {
        decimal total = 0m;

        foreach (var contract in brand.Contracts.Where(contract => contract.IsActiveForWeek(weekNumber)))
        {
            var standing = tournamentResult.Standings.FirstOrDefault(result => result.Golfer.Id == contract.GolferId);
            if (standing is null || standing.PrizeMoney <= 0m)
            {
                continue;
            }

            var share = contract.CalculateBrandShare(standing.PrizeMoney);
            financeEntries.Add(new FinanceEntry(
                weekNumber,
                FinanceEntryType.SponsorshipIncome,
                $"{standing.Golfer.FullName} PRIZE SHARE",
                share));

            total += share;
        }

        return total;
    }

    private static decimal ApplyProductRevenue(
        Brand brand,
        TournamentResult tournamentResult,
        GameState state,
        int weekNumber,
        ICollection<FinanceEntry> financeEntries)
    {
        var brandHeat = ProductLaunchService.CalculateDemandMultiplier(brand, tournamentResult, state, weekNumber);

        decimal total = 0m;
        foreach (var product in brand.Products.Where(p => p.IsActive))
        {
            var profit = product.CalculateWeeklyProfit(brandHeat);
            financeEntries.Add(new FinanceEntry(
                weekNumber,
                FinanceEntryType.ProductRevenue,
                $"{product.Name} WEEKLY SALES",
                profit));

            total += profit;
        }

        return total;
    }

    private static decimal ApplyOperatingExpense(Brand brand, int weekNumber, ICollection<FinanceEntry> financeEntries)
    {
        decimal total = 0m;

        foreach (var contract in brand.Contracts.Where(contract => contract.IsActiveForWeek(weekNumber)))
        {
            var retainer = -contract.WeeklyRetainer;
            financeEntries.Add(new FinanceEntry(
                weekNumber,
                FinanceEntryType.OperatingExpense,
                $"{contract.ContractName} RETAINER",
                retainer));

            total += Math.Abs(retainer);
        }

        return total;
    }
}

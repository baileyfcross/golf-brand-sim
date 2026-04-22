using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Finance;

namespace GolfBrandSim.Core.Simulation;

public sealed class WeeklyGameLoop
{
    private readonly ITournamentSimulator _tournamentSimulator;

    public WeeklyGameLoop(ITournamentSimulator tournamentSimulator)
    {
        _tournamentSimulator = tournamentSimulator;
    }

    public WeekSimulationResult Advance(GameState state)
    {
        if (state.IsSeasonComplete)
        {
            throw new InvalidOperationException("The season has already been completed.");
        }

        var weekNumber = state.CurrentWeekNumber;
        var tournament = state.SeasonSchedule.GetTournamentForWeek(weekNumber);
        var tournamentResult = _tournamentSimulator.Simulate(tournament, state.Golfers);
        var financeEntries = new List<FinanceEntry>();

        var sponsorshipIncome = ApplySponsorshipIncome(state.PlayerBrand, tournamentResult, weekNumber, financeEntries);
        var productProfit = ApplyProductRevenue(state.PlayerBrand, tournamentResult, weekNumber, financeEntries);
        var operatingExpense = ApplyOperatingExpense(state.PlayerBrand, weekNumber, financeEntries);
        ApplyResearchProgress(state.PlayerBrand, weekNumber, financeEntries);

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
        int weekNumber,
        ICollection<FinanceEntry> financeEntries)
    {
        var sponsoredGolfers = tournamentResult.Standings
            .Where(standing => brand.Contracts.Any(contract => contract.GolferId == standing.Golfer.Id))
            .ToList();

        var brandHeat = 1.0m;
        if (sponsoredGolfers.Count > 0)
        {
            var bestFinish = sponsoredGolfers.Min(standing => standing.Place);
            brandHeat += Math.Max(0m, 0.18m - (bestFinish - 1) * 0.01m);
        }

        decimal total = 0m;
        foreach (var product in brand.Products.Where(product => product.IsActive))
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

    private static void ApplyResearchProgress(Brand brand, int weekNumber, ICollection<FinanceEntry> financeEntries)
    {
        foreach (var track in brand.ResearchTracks.Where(track => !track.IsUnlocked))
        {
            if (!track.Advance(6))
            {
                continue;
            }

            if (!brand.HasProductInCategory(track.Category))
            {
                brand.Products.Add(BrandProduct.CreateExpansionLine(brand.Name, track.Category));
            }

            financeEntries.Add(new FinanceEntry(
                weekNumber,
                FinanceEntryType.ResearchUnlock,
                $"{track.Category} RESEARCH COMPLETE",
                0m));
        }
    }
}
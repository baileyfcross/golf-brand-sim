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
        // Base demand multiplier from best sponsored finish
        var sponsoredStandings = tournamentResult.Standings
            .Where(s => brand.Contracts.Any(c => c.GolferId == s.Golfer.Id && c.IsActiveForWeek(weekNumber)))
            .ToList();

        var brandHeat = 1.0m;
        if (sponsoredStandings.Count > 0)
        {
            var bestFinish = sponsoredStandings.Min(s => s.Place);
            // Win is a big boost, top-5 meaningful, top-10 minor
            brandHeat += Math.Max(0m, 0.20m - (bestFinish - 1) * 0.012m);

            // Additional boost from golfer marketability (popularity × marketability effect)
            var bestGolfer = sponsoredStandings.First(s => s.Place == bestFinish).Golfer;
            var mktBoost = bestGolfer.Marketability * 0.0008m;
            brandHeat += mktBoost;

            // Seasonal wins bonus: each win this season by a contracted golfer adds a small permanent lift
            var contractedIds = brand.Contracts
                .Where(c => c.IsActiveForWeek(weekNumber))
                .Select(c => c.GolferId)
                .ToHashSet();

            var seasonWins = contractedIds
                .Where(id => state.SeasonStandings.Stats.TryGetValue(id, out var s) && s.Wins > 0)
                .Sum(id => state.SeasonStandings.Stats[id].Wins);

            brandHeat += seasonWins * 0.015m;
        }

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

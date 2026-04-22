using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Finance;
using GolfBrandSim.Core.Simulation;

namespace GolfBrandSim.Infrastructure.Seed;

public static class InitialGameStateFactory
{
    public static GameSession Create(int seed)
    {
        var golfers = FictionalGolferSeedData.CreateGolfers();
        var specialization = ProductCategory.Equipment;
        var brand = CreateBrand(golfers, specialization);
        var financeLedger = new FinanceLedger();
        financeLedger.Add(new FinanceEntry(0, FinanceEntryType.Capital, "FOUNDER CAPITAL", brand.CashBalance));

        ISeasonGenerator seasonGenerator = new SeasonGenerator();
        ITournamentSimulator tournamentSimulator = new TournamentSimulator(seed);
        var state = new GameState(brand, golfers, seasonGenerator.Generate(2026, golfers.Count), financeLedger);
        return new GameSession(state, new WeeklyGameLoop(tournamentSimulator));
    }

    private static Brand CreateBrand(IReadOnlyList<Golfer> golfers, ProductCategory specialization)
    {
        var contracts = new[]
        {
            new SponsorshipContract(golfers[0].Id, "LIAM HART SIGNATURE", 0.20m, 210_000m, 1, 36),
            new SponsorshipContract(golfers[6].Id, "RIKU SATO TOUR", 0.18m, 165_000m, 1, 36),
            new SponsorshipContract(golfers[11].Id, "GABRIEL STONE DEVELOPMENT", 0.15m, 92_000m, 1, 36),
            new SponsorshipContract(golfers[18].Id, "BENJI FORD REGIONAL", 0.12m, 68_000m, 1, 36)
        };

        var products = new[]
        {
            BrandProduct.CreateStarterLine("SUMMIT", specialization)
        };

        var researchTracks = Enum.GetValues<ProductCategory>()
            .Select(category => new ResearchState(category, 30, category == specialization))
            .ToArray();

        return new Brand(
            Guid.NewGuid(),
            "SUMMIT",
            specialization,
            1_250_000m,
            products,
            researchTracks,
            contracts);
    }
}
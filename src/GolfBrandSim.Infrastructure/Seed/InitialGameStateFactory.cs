using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Finance;
using GolfBrandSim.Core.Simulation;

namespace GolfBrandSim.Infrastructure.Seed;

public static class InitialGameStateFactory
{
    public static GameSession Create(string brandName, ProductCategory specialization, int seed)
    {
        var golfers = FictionalGolferSeedData.CreateGolfers();
        var brand = CreateBrand(brandName, specialization);
        var financeLedger = new FinanceLedger();
        financeLedger.Add(new FinanceEntry(0, FinanceEntryType.Capital, "FOUNDER CAPITAL", brand.CashBalance));

        ISeasonGenerator seasonGenerator = new SeasonGenerator();
        ITournamentSimulator tournamentSimulator = new TournamentSimulator(seed);
        var state = new GameState(brand, golfers, seasonGenerator.Generate(2026, golfers.Count), financeLedger);
        return new GameSession(state, new WeeklyGameLoop(tournamentSimulator));
    }

    private static Brand CreateBrand(string brandName, ProductCategory specialization)
    {
        var startingCash = specialization switch
        {
            ProductCategory.Apparel => 1_100_000m,
            ProductCategory.Accessories => 1_150_000m,
            _ => 1_250_000m
        };

        var products = new[]
        {
            BrandProduct.CreateStarterLine(brandName, specialization)
        };

        var researchTracks = Enum.GetValues<ProductCategory>()
            .Select(category => new ResearchState(category, 30, category == specialization))
            .ToArray();

        return new Brand(
            Guid.NewGuid(),
            brandName,
            specialization,
            startingCash,
            products,
            researchTracks,
            []);
    }
}

using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Finance;
using GolfBrandSim.Core.Simulation;

namespace GolfBrandSim.Infrastructure.Seed;

public static class InitialGameStateFactory
{
    public static GameSession Create(string brandName, ProductCategory specialization, int seed)
    {
        return Create(brandName, specialization, GameDifficulty.Normal, seed);
    }

    public static GameSession Create(string brandName, ProductCategory specialization, GameDifficulty difficulty, int seed)
    {
        var golfers = FictionalGolferSeedData.CreateGolfers();
        var brand = CreateBrand(brandName, specialization, difficulty);
        var financeLedger = new FinanceLedger();
        financeLedger.Add(new FinanceEntry(0, FinanceEntryType.Capital, "FOUNDER CAPITAL", brand.CashBalance));

        ISeasonGenerator seasonGenerator = new SeasonGenerator();
        ITournamentSimulator tournamentSimulator = new TournamentSimulator(seed);

        var state = new GameState(brand, golfers, seasonGenerator.Generate(2026, golfers.Count), financeLedger);
        foreach (var competitor in CreateCompetitorBrands())
            state.CompetitorBrands.Add(competitor);

        var competitorService = new CompetitorSponsorshipService(new Random(seed + 17));
        competitorService.SeedInitialRosters(state, 3);

        var negotiationService = new ContractNegotiationService(new Random(seed + 31));
        return new GameSession(
            state,
            new WeeklyGameLoop(tournamentSimulator, competitorService),
            negotiationService);
    }

    private static Brand CreateBrand(string brandName, ProductCategory specialization, GameDifficulty difficulty)
    {
        var startingCash = specialization switch
        {
            ProductCategory.Apparel => 1_100_000m,
            ProductCategory.Accessories => 1_150_000m,
            _ => 1_250_000m
        };

        startingCash = difficulty switch
        {
            GameDifficulty.Easy => startingCash + 200_000m,
            GameDifficulty.Hard => startingCash - 200_000m,
            _ => startingCash
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

    private static IReadOnlyList<CompetitorBrand> CreateCompetitorBrands()
    {
        return
        [
            new CompetitorBrand(Guid.NewGuid(), "NORTH RIDGE ATHLETICS", "APPAREL", 3),
            new CompetitorBrand(Guid.NewGuid(), "FAIRWAY FORGE", "EQUIPMENT", 4),
            new CompetitorBrand(Guid.NewGuid(), "PIN SEEKER CO.", "ACCESSORIES", 2),
            new CompetitorBrand(Guid.NewGuid(), "GREENLINE SPORTS", "APPAREL", 5)
        ];
    }
}

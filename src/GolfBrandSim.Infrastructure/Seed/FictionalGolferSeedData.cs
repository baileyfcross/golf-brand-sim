using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Infrastructure.Seed;

public static class FictionalGolferSeedData
{
    public static IReadOnlyList<Golfer> CreateGolfers()
    {
        return
        [
            new Golfer(Guid.NewGuid(), "Liam Hart", "USA", 89, 86, 91),
            new Golfer(Guid.NewGuid(), "Mateo Alvarez", "ESP", 87, 82, 84),
            new Golfer(Guid.NewGuid(), "Ethan Cross", "USA", 84, 80, 75),
            new Golfer(Guid.NewGuid(), "Noah Bennett", "CAN", 83, 79, 71),
            new Golfer(Guid.NewGuid(), "Theo Mercer", "ENG", 86, 77, 82),
            new Golfer(Guid.NewGuid(), "Owen Gallagher", "IRL", 81, 85, 66),
            new Golfer(Guid.NewGuid(), "Riku Sato", "JPN", 85, 83, 73),
            new Golfer(Guid.NewGuid(), "Julian Price", "USA", 80, 81, 69),
            new Golfer(Guid.NewGuid(), "Logan Frost", "AUS", 79, 78, 64),
            new Golfer(Guid.NewGuid(), "Dylan Rhodes", "USA", 82, 74, 68),
            new Golfer(Guid.NewGuid(), "Adrian Vale", "RSA", 84, 72, 72),
            new Golfer(Guid.NewGuid(), "Gabriel Stone", "USA", 78, 84, 61),
            new Golfer(Guid.NewGuid(), "Micah Cole", "USA", 76, 77, 58),
            new Golfer(Guid.NewGuid(), "Parker Shaw", "NZL", 77, 75, 57),
            new Golfer(Guid.NewGuid(), "Hugo Laurent", "FRA", 83, 70, 67),
            new Golfer(Guid.NewGuid(), "Nico Barlow", "SCO", 79, 79, 63),
            new Golfer(Guid.NewGuid(), "Carson Pike", "USA", 74, 76, 55),
            new Golfer(Guid.NewGuid(), "Felix Wagner", "GER", 81, 73, 62),
            new Golfer(Guid.NewGuid(), "Benji Ford", "AUS", 75, 81, 56),
            new Golfer(Guid.NewGuid(), "Mason Clarke", "USA", 73, 72, 52),
            new Golfer(Guid.NewGuid(), "Rafael Costa", "BRA", 78, 71, 60),
            new Golfer(Guid.NewGuid(), "Jonas Meyer", "SWE", 72, 78, 50),
            new Golfer(Guid.NewGuid(), "Kei Nakamura", "JPN", 80, 69, 65),
            new Golfer(Guid.NewGuid(), "Ty Brooks", "USA", 71, 74, 48)
        ];
    }
}
namespace GolfBrandSim.Core.Domain;

public sealed class CourseProfile
{
    public CourseProfile(
        int distanceDemand,
        int accuracyDemand,
        int approachDemand,
        int shortGameDemand,
        int puttingDemand,
        int difficulty,
        int volatility)
    {
        DistanceDemand = distanceDemand;
        AccuracyDemand = accuracyDemand;
        ApproachDemand = approachDemand;
        ShortGameDemand = shortGameDemand;
        PuttingDemand = puttingDemand;
        Difficulty = difficulty;
        Volatility = volatility;
    }

    public int DistanceDemand { get; }

    public int AccuracyDemand { get; }

    public int ApproachDemand { get; }

    public int ShortGameDemand { get; }

    public int PuttingDemand { get; }

    public int Difficulty { get; }

    public int Volatility { get; }

    public static CourseProfile Balanced => new(5, 5, 5, 5, 5, 5, 5);
}

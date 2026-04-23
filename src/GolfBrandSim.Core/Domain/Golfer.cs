namespace GolfBrandSim.Core.Domain;

public sealed class Golfer
{
    public Golfer(
        Guid id,
        string fullName,
        string countryCode,
        int age,
        int drivingDistance,
        int drivingAccuracy,
        int approach,
        int shortGame,
        int putting,
        int mentality,
        int consistency,
        int popularity,
        int marketability)
    {
        Id = id;
        FullName = fullName;
        CountryCode = countryCode;
        Age = age;
        DrivingDistance = drivingDistance;
        DrivingAccuracy = drivingAccuracy;
        Approach = approach;
        ShortGame = shortGame;
        Putting = putting;
        Mentality = mentality;
        Consistency = consistency;
        Popularity = popularity;
        Marketability = marketability;
    }

    public Guid Id { get; }

    public string FullName { get; }

    public string CountryCode { get; }

    public int Age { get; }

    public int DrivingDistance { get; }

    public int DrivingAccuracy { get; }

    public int Approach { get; }

    public int ShortGame { get; }

    public int Putting { get; }

    public int Mentality { get; }

    public int Consistency { get; }

    public int Popularity { get; }

    public int Marketability { get; }

    public int Overall => (int)Math.Round(
        DrivingDistance * 0.10 +
        DrivingAccuracy * 0.15 +
        Approach * 0.25 +
        ShortGame * 0.20 +
        Putting * 0.20 +
        Mentality * 0.05 +
        Consistency * 0.05);
}
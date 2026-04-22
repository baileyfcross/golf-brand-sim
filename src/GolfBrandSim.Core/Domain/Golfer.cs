namespace GolfBrandSim.Core.Domain;

public sealed class Golfer
{
    public Golfer(Guid id, string fullName, string countryCode, int skillRating, int consistency, int popularity)
    {
        Id = id;
        FullName = fullName;
        CountryCode = countryCode;
        SkillRating = skillRating;
        Consistency = consistency;
        Popularity = popularity;
    }

    public Guid Id { get; }

    public string FullName { get; }

    public string CountryCode { get; }

    public int SkillRating { get; }

    public int Consistency { get; }

    public int Popularity { get; }
}
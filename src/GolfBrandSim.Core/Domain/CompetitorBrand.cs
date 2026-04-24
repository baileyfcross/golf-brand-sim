namespace GolfBrandSim.Core.Domain;

/// <summary>A CPU-controlled competing golf brand that signs golfers from the free agent pool.</summary>
public sealed class CompetitorBrand
{
    private readonly HashSet<Guid> _sponsoredGolferIds = new();

    public CompetitorBrand(Guid id, string name, string specialization, int aggressionLevel)
    {
        Id = id;
        Name = name;
        Specialization = specialization;
        AggressionLevel = aggressionLevel;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Specialization { get; }

    /// <summary>1–5. Higher means more likely to sign a golfer each week.</summary>
    public int AggressionLevel { get; }

    public IReadOnlyCollection<Guid> SponsoredGolferIds => _sponsoredGolferIds;

    public bool IsSponsoring(Guid golferId) => _sponsoredGolferIds.Contains(golferId);

    public void SignGolfer(Guid golferId) => _sponsoredGolferIds.Add(golferId);

    public void ReleaseGolfer(Guid golferId) => _sponsoredGolferIds.Remove(golferId);
}

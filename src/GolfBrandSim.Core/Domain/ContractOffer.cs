namespace GolfBrandSim.Core.Domain;

public enum ContractOfferStatus
{
    Pending,
    Accepted,
    Rejected,
    Expired
}

/// <summary>Represents a sponsorship offer the player has submitted to a free agent.</summary>
public sealed class ContractOffer
{
    public ContractOffer(
        Guid id,
        Guid golferId,
        decimal signingBonus,
        decimal weeklyRetainer,
        decimal winningsShareRate,
        int durationWeeks,
        int createdWeek)
    {
        Id = id;
        GolferId = golferId;
        SigningBonus = signingBonus;
        WeeklyRetainer = weeklyRetainer;
        WinningsShareRate = winningsShareRate;
        DurationWeeks = durationWeeks;
        CreatedWeek = createdWeek;
        Status = ContractOfferStatus.Pending;
    }

    public Guid Id { get; }

    public Guid GolferId { get; }

    public decimal SigningBonus { get; }

    public decimal WeeklyRetainer { get; }

    public decimal WinningsShareRate { get; }

    public int DurationWeeks { get; }

    public int CreatedWeek { get; }

    public ContractOfferStatus Status { get; private set; }

    public void Accept() => Status = ContractOfferStatus.Accepted;

    public void Reject() => Status = ContractOfferStatus.Rejected;

    public void Expire() => Status = ContractOfferStatus.Expired;
}

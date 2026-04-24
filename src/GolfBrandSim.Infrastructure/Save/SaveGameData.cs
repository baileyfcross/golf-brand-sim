using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Infrastructure.Save;

// Version 3: full-state save with negotiations and competitor brands
public sealed class SaveGameData
{
    public int Version { get; set; } = 3;

    public BrandSaveData Brand { get; set; } = new();

    public List<GolferSaveData> Golfers { get; set; } = [];

    public List<TournamentSaveData> Tournaments { get; set; } = [];

    public int SeasonYear { get; set; }

    public StandingsSaveData SeasonStandings { get; set; } = new();

    public List<FinanceEntrySaveData> FinanceLedger { get; set; } = [];

    public int CurrentWeekNumber { get; set; }

    public List<CompetitorBrandSaveData> CompetitorBrands { get; set; } = [];

    public List<ContractOfferSaveData> RecentOffers { get; set; } = [];

    public LastWeekSaveData? LastWeekResult { get; set; }
}

public sealed class CompetitorBrandSaveData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int AggressionLevel { get; set; }
    public List<Guid> SponsoredGolferIds { get; set; } = [];
}

public sealed class ContractOfferSaveData
{
    public Guid Id { get; set; }
    public Guid GolferId { get; set; }
    public decimal SigningBonus { get; set; }
    public decimal WeeklyRetainer { get; set; }
    public decimal WinningsShareRate { get; set; }
    public int DurationWeeks { get; set; }
    public int CreatedWeek { get; set; }
    public ContractOfferStatus Status { get; set; }
}

public sealed class BrandSaveData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public ProductCategory Specialization { get; set; }
    public decimal CashBalance { get; set; }
    public List<ProductSaveData> Products { get; set; } = [];
    public List<ResearchSaveData> ResearchTracks { get; set; } = [];
    public List<ContractSaveData> Contracts { get; set; } = [];
}

public sealed class ProductSaveData
{
    public string Name { get; set; } = "";
    public ProductCategory Category { get; set; }
    public decimal WeeklyRevenueBase { get; set; }
    public decimal MarginRate { get; set; }
    public bool IsActive { get; set; }
    public ProductQualityTier QualityTier { get; set; }
}

public sealed class ResearchSaveData
{
    public ProductCategory Category { get; set; }
    public int ProgressPoints { get; set; }
    public int UnlockThreshold { get; set; }
    public bool IsUnlocked { get; set; }
}

public sealed class ContractSaveData
{
    public Guid GolferId { get; set; }
    public string ContractName { get; set; } = "";
    public decimal WinningsShareRate { get; set; }
    public decimal AnnualRetainer { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; }
}

public sealed class GolferSaveData
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public int Age { get; set; }
    public int DrivingDistance { get; set; }
    public int DrivingAccuracy { get; set; }
    public int Approach { get; set; }
    public int ShortGame { get; set; }
    public int Putting { get; set; }
    public int Mentality { get; set; }
    public int Consistency { get; set; }
    public int Popularity { get; set; }
    public int Marketability { get; set; }
}

public sealed class TournamentSaveData
{
    public int WeekNumber { get; set; }
    public string Name { get; set; } = "";
    public string VenueName { get; set; } = "";
    public TournamentType Type { get; set; }
    public decimal Purse { get; set; }
    public int FieldSize { get; set; }
    public CourseProfileSaveData CourseProfile { get; set; } = new();
}

public sealed class CourseProfileSaveData
{
    public int DistanceDemand { get; set; }
    public int AccuracyDemand { get; set; }
    public int ApproachDemand { get; set; }
    public int ShortGameDemand { get; set; }
    public int PuttingDemand { get; set; }
    public int Difficulty { get; set; }
    public int Volatility { get; set; }
}

public sealed class StandingsSaveData
{
    public Dictionary<string, GolferStatsSaveData> Stats { get; set; } = [];
}

public sealed class GolferStatsSaveData
{
    public int Points { get; set; }
    public int Wins { get; set; }
    public int Top10s { get; set; }
    public decimal Earnings { get; set; }
    public int EventsPlayed { get; set; }
    public int CutsMade { get; set; }
    public int MajorWins { get; set; }
    public int BestFinish { get; set; }
    public int LastFinish { get; set; }
}

public sealed class FinanceEntrySaveData
{
    public int WeekNumber { get; set; }
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}

public sealed class LastWeekSaveData
{
    public int WeekNumber { get; set; }
    public decimal ProductProfit { get; set; }
    public decimal SponsorshipIncome { get; set; }
    public decimal OperatingExpense { get; set; }
    public decimal NetCashChange { get; set; }
    public decimal EndingCashBalance { get; set; }
    public TournamentResultSaveData? TournamentResult { get; set; }
}

public sealed class TournamentResultSaveData
{
    public string TournamentName { get; set; } = "";
    public int CutScore { get; set; }
    public List<StandingSaveData> Standings { get; set; } = [];
}

public sealed class StandingSaveData
{
    public Guid GolferId { get; set; }
    public int Place { get; set; }
    public List<int> RoundScores { get; set; } = [];
    public bool MadeCut { get; set; }
    public decimal PrizeMoney { get; set; }
}

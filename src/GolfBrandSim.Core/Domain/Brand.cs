using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Core.Domain;

public sealed class Brand
{
    public Brand(
        Guid id,
        string name,
        ProductCategory specialization,
        decimal cashBalance,
        IEnumerable<BrandProduct> products,
        IEnumerable<ResearchState> researchTracks,
        IEnumerable<SponsorshipContract> contracts)
    {
        Id = id;
        Name = name;
        Specialization = specialization;
        CashBalance = cashBalance;
        Products = products.ToList();
        ResearchTracks = researchTracks.ToList();
        Contracts = contracts.ToList();
    }

    public Guid Id { get; }

    public string Name { get; }

    public ProductCategory Specialization { get; }

    public decimal CashBalance { get; private set; }

    public IList<BrandProduct> Products { get; }

    public IList<ResearchState> ResearchTracks { get; }

    public IList<SponsorshipContract> Contracts { get; }

    public void ApplyCashChange(decimal amount)
    {
        CashBalance += amount;
    }

    public ResearchState GetResearch(ProductCategory category)
    {
        return ResearchTracks.Single(track => track.Category == category);
    }

    public bool HasProductInCategory(ProductCategory category)
    {
        return Products.Any(product => product.Category == category && product.IsActive);
    }

    /// <summary>Signs a golfer to a contract using the given template. Returns false if already contracted or insufficient cash.</summary>
    public bool SignGolfer(Guid golferId, ContractTemplate template, int currentWeek)
    {
        if (Contracts.Any(c => c.GolferId == golferId && c.IsActiveForWeek(currentWeek)))
            return false;

        if (CashBalance < template.SigningBonus)
            return false;

        CashBalance -= template.SigningBonus;

        var endWeek = currentWeek + template.DurationWeeks - 1;
        Contracts.Add(new SponsorshipContract(
            golferId,
            template.Name,
            template.WinningsShareRate,
            template.AnnualRetainer,
            currentWeek,
            endWeek));

        return true;
    }

    /// <summary>
    /// Launches a new product. Deducts launch cost from cash. Returns false if category not unlocked or insufficient cash.
    /// </summary>
    public bool LaunchProduct(string productName, ProductCategory category, ProductQualityTier tier)
    {
        if (!GetResearch(category).IsUnlocked) return false;

        var launchCost = BrandProduct.GetLaunchCost(tier);
        if (CashBalance < launchCost) return false;

        CashBalance -= launchCost;
        Products.Add(BrandProduct.CreateByTier(productName, category, tier));
        return true;
    }

    /// <summary>
    /// Invests cash in research for a category. $5,000 per point. Returns true if the category just unlocked.
    /// </summary>
    public bool InvestInResearch(ProductCategory category, decimal cashAmount)
    {
        if (cashAmount <= 0 || CashBalance < cashAmount) return false;

        CashBalance -= cashAmount;
        var points = Math.Max(1, (int)(cashAmount / 5000m));
        var track = GetResearch(category);
        var justUnlocked = track.Advance(points);

        if (justUnlocked && !HasProductInCategory(category))
        {
            Products.Add(BrandProduct.CreateExpansionLine(Name, category));
        }

        return justUnlocked;
    }

    /// <summary>
    /// Signs a golfer using a custom negotiated offer. Deducts signing bonus from cash.
    /// Returns false if already contracted or insufficient cash.
    /// </summary>
    public bool SignGolferFromOffer(ContractOffer offer, int currentWeek)
    {
        if (Contracts.Any(c => c.GolferId == offer.GolferId && c.IsActiveForWeek(currentWeek)))
            return false;

        if (CashBalance < offer.SigningBonus)
            return false;

        CashBalance -= offer.SigningBonus;

        var annualRetainer = offer.WeeklyRetainer * 52m;
        var endWeek = currentWeek + offer.DurationWeeks - 1;

        Contracts.Add(new SponsorshipContract(
            offer.GolferId,
            "CUSTOM DEAL",
            offer.WinningsShareRate,
            annualRetainer,
            currentWeek,
            endWeek));

        return true;
    }
}

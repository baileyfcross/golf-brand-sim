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
}
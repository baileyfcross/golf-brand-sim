using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Core.Domain;

public sealed class BrandProduct
{
    public BrandProduct(string name, ProductCategory category, decimal weeklyRevenueBase, decimal marginRate, bool isActive = true, ProductQualityTier qualityTier = ProductQualityTier.Standard)
    {
        Name = name;
        Category = category;
        WeeklyRevenueBase = weeklyRevenueBase;
        MarginRate = marginRate;
        IsActive = isActive;
        QualityTier = qualityTier;
    }

    public string Name { get; }

    public ProductCategory Category { get; }

    public decimal WeeklyRevenueBase { get; }

    public decimal MarginRate { get; }

    public bool IsActive { get; }

    public ProductQualityTier QualityTier { get; }

    public decimal CalculateWeeklyProfit(decimal demandMultiplier)
    {
        var grossRevenue = WeeklyRevenueBase * demandMultiplier;
        return decimal.Round(grossRevenue * MarginRate, 2, MidpointRounding.AwayFromZero);
    }

    public static BrandProduct CreateStarterLine(string brandName, ProductCategory category)
    {
        return category switch
        {
            ProductCategory.Apparel => new BrandProduct($"{brandName} TOUR POLO", category, 38000m, 0.32m, true, ProductQualityTier.Standard),
            ProductCategory.Accessories => new BrandProduct($"{brandName} RANGE KIT", category, 30000m, 0.36m, true, ProductQualityTier.Standard),
            _ => new BrandProduct($"{brandName} TOUR DRIVER", category, 46000m, 0.28m, true, ProductQualityTier.Standard)
        };
    }

    public static BrandProduct CreateExpansionLine(string brandName, ProductCategory category)
    {
        return category switch
        {
            ProductCategory.Apparel => new BrandProduct($"{brandName} PERFORMANCE CAPSULE", category, 34000m, 0.30m, true, ProductQualityTier.Standard),
            ProductCategory.Accessories => new BrandProduct($"{brandName} PLAYER ESSENTIALS", category, 28000m, 0.35m, true, ProductQualityTier.Standard),
            _ => new BrandProduct($"{brandName} FORGED SERIES", category, 42000m, 0.26m, true, ProductQualityTier.Standard)
        };
    }

    public static BrandProduct CreateByTier(string productName, ProductCategory category, ProductQualityTier tier)
    {
        var (baseRevenue, margin, launchCost) = tier switch
        {
            ProductQualityTier.Budget => (category switch
            {
                ProductCategory.Apparel => 24000m,
                ProductCategory.Accessories => 19000m,
                _ => 29000m
            }, 0.22m, 20000m),
            ProductQualityTier.Premium => (category switch
            {
                ProductCategory.Apparel => 54000m,
                ProductCategory.Accessories => 44000m,
                _ => 65000m
            }, 0.36m, 140000m),
            _ => (category switch  // Standard
            {
                ProductCategory.Apparel => 38000m,
                ProductCategory.Accessories => 30000m,
                _ => 46000m
            }, 0.28m, 60000m)
        };

        return new BrandProduct(productName, category, baseRevenue, margin, true, tier);
    }

    public static decimal GetLaunchCost(ProductQualityTier tier) => tier switch
    {
        ProductQualityTier.Budget => 20000m,
        ProductQualityTier.Premium => 140000m,
        _ => 60000m
    };
}

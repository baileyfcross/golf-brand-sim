using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Core.Domain;

public sealed class BrandProduct
{
    public BrandProduct(string name, ProductCategory category, decimal weeklyRevenueBase, decimal marginRate, bool isActive = true)
    {
        Name = name;
        Category = category;
        WeeklyRevenueBase = weeklyRevenueBase;
        MarginRate = marginRate;
        IsActive = isActive;
    }

    public string Name { get; }

    public ProductCategory Category { get; }

    public decimal WeeklyRevenueBase { get; }

    public decimal MarginRate { get; }

    public bool IsActive { get; }

    public decimal CalculateWeeklyProfit(decimal demandMultiplier)
    {
        var grossRevenue = WeeklyRevenueBase * demandMultiplier;
        return decimal.Round(grossRevenue * MarginRate, 2, MidpointRounding.AwayFromZero);
    }

    public static BrandProduct CreateStarterLine(string brandName, ProductCategory category)
    {
        return category switch
        {
            ProductCategory.Apparel => new BrandProduct($"{brandName} TOUR POLO", category, 38000m, 0.32m),
            ProductCategory.Accessories => new BrandProduct($"{brandName} RANGE KIT", category, 30000m, 0.36m),
            _ => new BrandProduct($"{brandName} TOUR DRIVER", category, 46000m, 0.28m)
        };
    }

    public static BrandProduct CreateExpansionLine(string brandName, ProductCategory category)
    {
        return category switch
        {
            ProductCategory.Apparel => new BrandProduct($"{brandName} PERFORMANCE CAPSULE", category, 34000m, 0.30m),
            ProductCategory.Accessories => new BrandProduct($"{brandName} PLAYER ESSENTIALS", category, 28000m, 0.35m),
            _ => new BrandProduct($"{brandName} FORGED SERIES", category, 42000m, 0.26m)
        };
    }
}
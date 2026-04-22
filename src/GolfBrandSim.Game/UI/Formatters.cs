namespace GolfBrandSim.Game.UI;

public static class Formatters
{
    public static string Money(decimal amount)
    {
        var absolute = Math.Abs(amount);
        var prefix = amount < 0 ? "-" : string.Empty;

        if (absolute >= 1_000_000m && absolute % 1_000_000m == 0)
        {
            return $"{prefix}{absolute / 1_000_000m:0}M";
        }

        if (absolute >= 1_000m)
        {
            return $"{prefix}{decimal.Round(absolute / 1_000m, 0, MidpointRounding.AwayFromZero):0}K";
        }

        return $"{prefix}{decimal.Round(absolute, 0, MidpointRounding.AwayFromZero):0}";
    }

    public static string Percent(decimal value)
    {
        return $"{decimal.Round(value * 100m, 0, MidpointRounding.AwayFromZero):0} PCT";
    }

    public static string WeekLabel(int weekNumber)
    {
        return $"WK {weekNumber:00}";
    }
}
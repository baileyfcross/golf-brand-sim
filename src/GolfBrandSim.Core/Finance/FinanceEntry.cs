using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Core.Finance;

public sealed class FinanceEntry
{
    public FinanceEntry(int weekNumber, FinanceEntryType type, string description, decimal amount)
    {
        WeekNumber = weekNumber;
        Type = type;
        Description = description;
        Amount = amount;
    }

    public int WeekNumber { get; }

    public FinanceEntryType Type { get; }

    public string Description { get; }

    public decimal Amount { get; }
}
namespace GolfBrandSim.Core.Finance;

public sealed class FinanceLedger
{
    private readonly List<FinanceEntry> _entries = new();

    public IReadOnlyList<FinanceEntry> Entries => _entries;

    public void Add(FinanceEntry entry)
    {
        _entries.Add(entry);
    }

    public void AddRange(IEnumerable<FinanceEntry> entries)
    {
        _entries.AddRange(entries);
    }

    public decimal GetNetForWeek(int weekNumber)
    {
        return _entries.Where(entry => entry.WeekNumber == weekNumber).Sum(entry => entry.Amount);
    }
}
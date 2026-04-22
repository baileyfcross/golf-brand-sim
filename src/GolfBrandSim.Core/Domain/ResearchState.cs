using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Core.Domain;

public sealed class ResearchState
{
    public ResearchState(ProductCategory category, int unlockThreshold, bool isUnlocked)
    {
        Category = category;
        UnlockThreshold = unlockThreshold;
        IsUnlocked = isUnlocked;
        ProgressPoints = isUnlocked ? unlockThreshold : 0;
    }

    public ProductCategory Category { get; }

    public int ProgressPoints { get; private set; }

    public int UnlockThreshold { get; }

    public bool IsUnlocked { get; private set; }

    public bool Advance(int points)
    {
        if (IsUnlocked)
        {
            return false;
        }

        ProgressPoints = Math.Min(UnlockThreshold, ProgressPoints + points);
        if (ProgressPoints < UnlockThreshold)
        {
            return false;
        }

        IsUnlocked = true;
        return true;
    }
}
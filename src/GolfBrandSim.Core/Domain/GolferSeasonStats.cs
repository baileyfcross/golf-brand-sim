namespace GolfBrandSim.Core.Domain;

public sealed class GolferSeasonStats
{
    public GolferSeasonStats(Guid golferId)
    {
        GolferId = golferId;
    }

    /// <summary>Direct-restore constructor for save/load.</summary>
    public GolferSeasonStats(
        Guid golferId,
        int points,
        int wins,
        int top10s,
        decimal earnings,
        int eventsPlayed = 0,
        int cutsMade = 0,
        int majorWins = 0,
        int bestFinish = 0,
        int lastFinish = 0)
    {
        GolferId = golferId;
        Points = points;
        Wins = wins;
        Top10s = top10s;
        Earnings = earnings;
        EventsPlayed = eventsPlayed;
        CutsMade = cutsMade;
        MajorWins = majorWins;
        BestFinish = bestFinish;
        LastFinish = lastFinish;
    }

    public Guid GolferId { get; }

    public int Points { get; private set; }

    public int Wins { get; private set; }

    public int Top10s { get; private set; }

    public decimal Earnings { get; private set; }

    public int EventsPlayed { get; private set; }

    public int CutsMade { get; private set; }

    public int MajorWins { get; private set; }

    /// <summary>Best (lowest) finishing place this season. 0 = no finishes recorded.</summary>
    public int BestFinish { get; private set; }

    /// <summary>Finishing place in the most recent event.</summary>
    public int LastFinish { get; private set; }

    public void RecordFinish(int place, decimal prizeMoney, bool isMajor, bool madeCut)
    {
        EventsPlayed++;
        LastFinish = place;
        if (BestFinish == 0 || place < BestFinish) BestFinish = place;

        if (madeCut)
        {
            CutsMade++;
            Earnings += prizeMoney;
            if (place <= 10) Top10s++;
            if (place == 1)
            {
                Wins++;
                if (isMajor) MajorWins++;
            }
        }

        var points = madeCut ? PointsForPlace(place) : 0;
        Points += isMajor && madeCut ? (int)(points * 1.5) : points;
    }

    private static int PointsForPlace(int place) => place switch
    {
        1 => 500,
        2 => 300,
        3 => 200,
        4 => 160,
        5 => 140,
        6 => 125,
        7 => 110,
        8 => 100,
        9 => 90,
        10 => 80,
        <= 15 => 70 - (place - 11) * 5,
        <= 25 => 48 - (place - 16) * 2,
        _ => 10
    };
}

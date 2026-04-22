namespace GolfBrandSim.Core.Domain;

public sealed class TournamentStanding
{
    public TournamentStanding(Golfer golfer, int place, IReadOnlyList<int> roundScores, bool madeCut, decimal prizeMoney)
    {
        Golfer = golfer;
        Place = place;
        RoundScores = roundScores;
        MadeCut = madeCut;
        PrizeMoney = prizeMoney;
    }

    public Golfer Golfer { get; }

    public int Place { get; }

    public IReadOnlyList<int> RoundScores { get; }

    public bool MadeCut { get; }

    public decimal PrizeMoney { get; }

    public int TotalScore => RoundScores.Sum();
}
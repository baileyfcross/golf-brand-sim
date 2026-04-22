namespace GolfBrandSim.Core.Domain;

public sealed class TournamentResult
{
    public TournamentResult(Tournament tournament, IReadOnlyList<TournamentStanding> standings, int cutScore)
    {
        Tournament = tournament;
        Standings = standings;
        CutScore = cutScore;
    }

    public Tournament Tournament { get; }

    public IReadOnlyList<TournamentStanding> Standings { get; }

    public int CutScore { get; }

    public TournamentStanding Winner => Standings[0];
}
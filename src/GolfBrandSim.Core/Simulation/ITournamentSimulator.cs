using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

public interface ITournamentSimulator
{
    TournamentResult Simulate(Tournament tournament, IReadOnlyList<Golfer> golfers);
}
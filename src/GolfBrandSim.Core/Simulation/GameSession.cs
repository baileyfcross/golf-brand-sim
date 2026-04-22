using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

public sealed class GameSession
{
    private readonly WeeklyGameLoop _weeklyGameLoop;

    public GameSession(GameState state, WeeklyGameLoop weeklyGameLoop)
    {
        State = state;
        _weeklyGameLoop = weeklyGameLoop;
    }

    public GameState State { get; }

    public bool CanAdvanceWeek => !State.IsSeasonComplete;

    public WeekSimulationResult AdvanceWeek()
    {
        return _weeklyGameLoop.Advance(State);
    }
}
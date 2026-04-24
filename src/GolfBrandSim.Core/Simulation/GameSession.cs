using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

public sealed class GameSession
{
    private readonly WeeklyGameLoop _weeklyGameLoop;
    private readonly ContractNegotiationService _negotiationService;

    public GameSession(
        GameState state,
        WeeklyGameLoop weeklyGameLoop,
        ContractNegotiationService? negotiationService = null)
    {
        State = state;
        _weeklyGameLoop = weeklyGameLoop;
        _negotiationService = negotiationService ?? new ContractNegotiationService(new Random());
    }

    public GameState State { get; }

    public bool CanAdvanceWeek => !State.IsSeasonComplete;

    public WeekSimulationResult AdvanceWeek()
    {
        return _weeklyGameLoop.Advance(State);
    }

    /// <summary>
    /// Evaluates a sponsorship offer for the given golfer. If accepted, automatically signs them.
    /// </summary>
    public ContractDecisionResult EvaluateOffer(ContractOffer offer, Golfer golfer)
    {
        var result = _negotiationService.Evaluate(
            offer, golfer, State.SeasonStandings, State.PlayerBrand, State.CurrentWeekNumber);

        if (result.IsAccepted)
            State.PlayerBrand.SignGolferFromOffer(offer, State.CurrentWeekNumber);

        // Keep a rolling log of recent offers (last 20)
        State.RecentOffers.Add(offer);
        if (State.RecentOffers.Count > 20)
            State.RecentOffers.RemoveAt(0);

        return result;
    }
}
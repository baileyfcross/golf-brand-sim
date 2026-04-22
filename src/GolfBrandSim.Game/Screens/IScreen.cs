using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public interface IScreen
{
    string TabLabel { get; }

    void HandleInput(InputState input, GameSession session, Rectangle bounds);

    void Draw(UiContext ui, GameSession session, Rectangle bounds);
}
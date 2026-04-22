using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GolfBrandSim.Game.App;

public readonly record struct InputState(
    KeyboardState Current,
    KeyboardState Previous,
    MouseState CurrentMouse,
    MouseState PreviousMouse)
{
    public bool IsNewKeyPress(Keys key)
    {
        return Current.IsKeyDown(key) && Previous.IsKeyUp(key);
    }

    public Point MousePosition => new(CurrentMouse.X, CurrentMouse.Y);

    public bool IsNewLeftClick()
    {
        return CurrentMouse.LeftButton == ButtonState.Pressed && PreviousMouse.LeftButton == ButtonState.Released;
    }
}
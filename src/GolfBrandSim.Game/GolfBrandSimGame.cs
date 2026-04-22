using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using GolfBrandSim.Infrastructure.Seed;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GolfBrandSim.Game;

public sealed class GolfBrandSimGame : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly ScreenManager _screenManager;

    private SpriteBatch? _spriteBatch;
    private PrimitiveBatch? _primitiveBatch;
    private PixelFont? _pixelFont;
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    public GolfBrandSimGame(int simulationSeed)
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1440;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.SynchronizeWithVerticalRetrace = true;

        IsMouseVisible = true;
        IsFixedTimeStep = true;
        Window.Title = "Golf Brand Sim";

        _screenManager = new ScreenManager(InitialGameStateFactory.Create(simulationSeed));
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _primitiveBatch = new PrimitiveBatch(GraphicsDevice);
        _pixelFont = new PixelFont();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
            return;
        }

        var input = new InputState(keyboardState, _previousKeyboardState, mouseState, _previousMouseState);
        _screenManager.Update(input, GraphicsDevice.Viewport.Bounds);
        _previousKeyboardState = keyboardState;
        _previousMouseState = mouseState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (_spriteBatch is null || _primitiveBatch is null || _pixelFont is null)
        {
            return;
        }

        GraphicsDevice.Clear(Theme.Background);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        var ui = new UiContext(_spriteBatch, _primitiveBatch, _pixelFont, GraphicsDevice.Viewport.Bounds);
        _screenManager.Draw(ui);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
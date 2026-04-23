using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Infrastructure.Seed;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GolfBrandSim.Game;

public sealed class GolfBrandSimGame : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly SaveGameStore _saveGameStore;

    private readonly int _bootstrapSeed;

    private ScreenManager? _screenManager;
    private GameSession? _session;

    private SpriteBatch? _spriteBatch;
    private PrimitiveBatch? _primitiveBatch;
    private PixelFont? _pixelFont;
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    private AppMode _mode = AppMode.MainMenu;
    private GameOptions _options = new();
    private string _menuMessage = "";
    private int _menuHoverIndex = -1;
    private int _optionsHoverIndex = -1;

    public GolfBrandSimGame(int simulationSeed)
    {
        _bootstrapSeed = simulationSeed;
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1440;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.SynchronizeWithVerticalRetrace = true;
        _saveGameStore = new SaveGameStore();

        IsMouseVisible = true;
        IsFixedTimeStep = true;
        Window.AllowUserResizing = true;
        Window.Title = "Golf Brand Sim";
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _primitiveBatch = new PrimitiveBatch(GraphicsDevice);
        _pixelFont = new PixelFont();

        StartNewGame(_bootstrapSeed);
        _mode = AppMode.MainMenu;
        _menuMessage = _saveGameStore.HasSaveFile() ? "SAVE SLOT READY" : "NO SAVED GAME FOUND";
        ApplySoundOptions();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();
        var input = new InputState(keyboardState, _previousKeyboardState, mouseState, _previousMouseState);

        if (_mode == AppMode.InGame && input.IsNewKeyPress(Keys.Escape))
        {
            _mode = AppMode.MainMenu;
            _menuMessage = "PAUSED";
        }
        else if ((_mode == AppMode.MainMenu || _mode == AppMode.Options) && input.IsNewKeyPress(Keys.Escape))
        {
            if (_mode == AppMode.Options)
            {
                _mode = AppMode.MainMenu;
            }
            else
            {
                Exit();
                return;
            }
        }

        switch (_mode)
        {
            case AppMode.MainMenu:
                UpdateMainMenu(input);
                break;
            case AppMode.Options:
                UpdateOptionsMenu(input);
                break;
            case AppMode.InGame:
                if (_screenManager is not null)
                {
                    _screenManager.Update(input, GraphicsDevice.Viewport.Bounds);
                }

                break;
        }

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
        switch (_mode)
        {
            case AppMode.MainMenu:
                DrawMainMenu(ui);
                break;
            case AppMode.Options:
                DrawOptionsMenu(ui);
                break;
            case AppMode.InGame:
                _screenManager?.Draw(ui);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateMainMenu(InputState input)
    {
        var items = GetMainMenuItems();
        _menuHoverIndex = GetHoveredMenuIndex(input.MousePosition, items.Count, 300);

        if (input.IsNewKeyPress(Keys.Down))
        {
            _menuHoverIndex = (_menuHoverIndex + 1 + items.Count) % items.Count;
        }
        else if (input.IsNewKeyPress(Keys.Up))
        {
            _menuHoverIndex = (_menuHoverIndex - 1 + items.Count) % items.Count;
        }

        if (input.IsNewLeftClick() && _menuHoverIndex >= 0)
        {
            HandleMainMenuSelection(items[_menuHoverIndex]);
        }
        else if (input.IsNewKeyPress(Keys.Enter) && _menuHoverIndex >= 0)
        {
            HandleMainMenuSelection(items[_menuHoverIndex]);
        }
    }

    private void HandleMainMenuSelection(string selected)
    {
        switch (selected)
        {
            case "CONTINUE GAME":
                if (_session is null)
                {
                    _menuMessage = "NO ACTIVE GAME";
                    return;
                }

                _mode = AppMode.InGame;
                break;
            case "LOAD GAME":
                if (!_saveGameStore.TryLoad(out var loadedSession, out var loadedSeed))
                {
                    _menuMessage = "NO SAVE FILE TO LOAD";
                    return;
                }

                AttachSession(loadedSession, loadedSeed);
                _mode = AppMode.InGame;
                break;
            case "NEW GAME":
                StartNewGame(Environment.TickCount);
                _mode = AppMode.InGame;
                break;
            case "OPTIONS":
                _mode = AppMode.Options;
                _menuMessage = "";
                break;
            case "EXIT":
                Exit();
                break;
        }
    }

    private void UpdateOptionsMenu(InputState input)
    {
        var rows = GetOptionsRows();
        _optionsHoverIndex = GetHoveredMenuIndex(input.MousePosition, rows.Count, 250);

        if (input.IsNewKeyPress(Keys.Down))
        {
            _optionsHoverIndex = (_optionsHoverIndex + 1 + rows.Count) % rows.Count;
        }
        else if (input.IsNewKeyPress(Keys.Up))
        {
            _optionsHoverIndex = (_optionsHoverIndex - 1 + rows.Count) % rows.Count;
        }

        if (_optionsHoverIndex >= 0)
        {
            if (input.IsNewKeyPress(Keys.Left))
            {
                ApplyOptionsChange(_optionsHoverIndex, -1);
            }
            else if (input.IsNewKeyPress(Keys.Right))
            {
                ApplyOptionsChange(_optionsHoverIndex, 1);
            }
            else if (input.IsNewKeyPress(Keys.Enter))
            {
                ApplyOptionsChange(_optionsHoverIndex, 1);
            }
        }

        if (!input.IsNewLeftClick() || _optionsHoverIndex < 0)
        {
            return;
        }

        var rowBounds = GetMenuRowBounds(_optionsHoverIndex, 250);
        var leftArrowBounds = new Rectangle(rowBounds.Right - 112, rowBounds.Y + 5, 44, rowBounds.Height - 10);
        var rightArrowBounds = new Rectangle(rowBounds.Right - 56, rowBounds.Y + 5, 44, rowBounds.Height - 10);

        if (leftArrowBounds.Contains(input.MousePosition))
        {
            ApplyOptionsChange(_optionsHoverIndex, -1);
        }
        else if (rightArrowBounds.Contains(input.MousePosition))
        {
            ApplyOptionsChange(_optionsHoverIndex, 1);
        }
        else
        {
            ApplyOptionsChange(_optionsHoverIndex, 1);
        }
    }

    private void ApplyOptionsChange(int rowIndex, int direction)
    {
        switch (rowIndex)
        {
            case 0:
                if (direction < 0)
                {
                    _options.PreviousResolution();
                }
                else
                {
                    _options.NextResolution();
                }

                ApplyDisplayOptions();
                break;
            case 1:
                _options.WindowMode = direction < 0
                    ? (WindowMode)(((int)_options.WindowMode - 1 + 3) % 3)
                    : (WindowMode)(((int)_options.WindowMode + 1) % 3);
                ApplyDisplayOptions();
                break;
            case 2:
                AdjustVolume(value => _options.MasterVolume = value, _options.MasterVolume, direction);
                break;
            case 3:
                AdjustVolume(value => _options.MusicVolume = value, _options.MusicVolume, direction);
                break;
            case 4:
                AdjustVolume(value => _options.SfxVolume = value, _options.SfxVolume, direction);
                break;
            case 5:
                _mode = AppMode.MainMenu;
                _menuMessage = "OPTIONS SAVED";
                break;
        }
    }

    private void AdjustVolume(Action<float> setter, float current, int direction)
    {
        var updated = MathHelper.Clamp(current + 0.05f * direction, 0f, 1f);
        setter(updated);
        ApplySoundOptions();
    }

    private void ApplyDisplayOptions()
    {
        var resolution = _options.SelectedResolution;
        _graphics.PreferredBackBufferWidth = resolution.X;
        _graphics.PreferredBackBufferHeight = resolution.Y;

        switch (_options.WindowMode)
        {
            case WindowMode.Windowed:
                _graphics.IsFullScreen = false;
                Window.IsBorderless = false;
                Window.AllowUserResizing = true;
                break;
            case WindowMode.Borderless:
                _graphics.IsFullScreen = false;
                Window.IsBorderless = true;
                Window.AllowUserResizing = false;
                break;
            case WindowMode.Fullscreen:
                Window.IsBorderless = false;
                _graphics.IsFullScreen = true;
                Window.AllowUserResizing = false;
                break;
        }

        _graphics.ApplyChanges();
    }

    private void ApplySoundOptions()
    {
        var master = MathHelper.Clamp(_options.MasterVolume, 0f, 1f);
        SoundEffect.MasterVolume = MathHelper.Clamp(master * _options.SfxVolume, 0f, 1f);
        MediaPlayer.Volume = MathHelper.Clamp(master * _options.MusicVolume, 0f, 1f);
    }

    private void DrawMainMenu(UiContext ui)
    {
        var frame = ui.ViewportBounds;
        UiToolkit.DrawPanel(ui, new Rectangle(frame.Width / 2 - 360, 90, 720, frame.Height - 180), "MAIN MENU");
        ui.DrawText("GOLF BRAND SIM", new Vector2(frame.Width / 2 - 182, 140), Theme.TextPrimary, 4);
        ui.DrawText("MANAGE YOUR BRAND, ROSTER, AND WEEKLY TOUR SEASON", new Vector2(frame.Width / 2 - 286, 190), Theme.TextMuted, 2);

        var items = GetMainMenuItems();
        for (var index = 0; index < items.Count; index++)
        {
            var rowBounds = GetMenuRowBounds(index, 300);
            var hovered = index == _menuHoverIndex;
            ui.FillRectangle(rowBounds, hovered ? Theme.Accent : Theme.PanelRaised);
            ui.DrawBorder(rowBounds, hovered ? Theme.AccentHighlight : Theme.PanelBorder, 2);
            ui.DrawCenteredText(items[index], rowBounds, hovered ? Theme.Header : Theme.TextPrimary, 2);
        }

        if (!string.IsNullOrWhiteSpace(_menuMessage))
        {
            ui.DrawCenteredText(_menuMessage, new Rectangle(frame.Width / 2 - 250, frame.Height - 120, 500, 40), Theme.TextMuted, 2);
        }

        ui.DrawCenteredText("CLICK TO SELECT. ENTER AND ESC REMAIN AVAILABLE.", new Rectangle(frame.Width / 2 - 300, frame.Height - 80, 600, 36), Theme.TextMuted, 2);
    }

    private void DrawOptionsMenu(UiContext ui)
    {
        var frame = ui.ViewportBounds;
        UiToolkit.DrawPanel(ui, new Rectangle(frame.Width / 2 - 440, 90, 880, frame.Height - 180), "OPTIONS");
        ui.DrawText("DISPLAY AND AUDIO", new Vector2(frame.Width / 2 - 180, 140), Theme.TextPrimary, 3);

        var rows = GetOptionsRows();
        for (var index = 0; index < rows.Count; index++)
        {
            var rowBounds = GetMenuRowBounds(index, 250);
            var hovered = index == _optionsHoverIndex;
            ui.FillRectangle(rowBounds, hovered ? Theme.HighlightRow : Theme.PanelRaised);
            ui.DrawBorder(rowBounds, Theme.PanelBorder, 2);

            ui.DrawText(rows[index].Label, new Vector2(rowBounds.X + 18, rowBounds.Y + 13), Theme.TextPrimary, 2);
            ui.DrawText(rows[index].Value, new Vector2(rowBounds.X + 330, rowBounds.Y + 13), Theme.TextMuted, 2);

            if (!rows[index].IsBack)
            {
                var leftArrowBounds = new Rectangle(rowBounds.Right - 112, rowBounds.Y + 5, 44, rowBounds.Height - 10);
                var rightArrowBounds = new Rectangle(rowBounds.Right - 56, rowBounds.Y + 5, 44, rowBounds.Height - 10);

                ui.FillRectangle(leftArrowBounds, Theme.Panel);
                ui.DrawBorder(leftArrowBounds, Theme.PanelBorder, 1);
                ui.DrawCenteredText("<", leftArrowBounds, Theme.TextPrimary, 2);

                ui.FillRectangle(rightArrowBounds, Theme.Panel);
                ui.DrawBorder(rightArrowBounds, Theme.PanelBorder, 1);
                ui.DrawCenteredText(">", rightArrowBounds, Theme.TextPrimary, 2);
            }
        }

        ui.DrawCenteredText("CLICK ARROWS TO CHANGE SETTINGS. ESC RETURNS TO MENU.", new Rectangle(frame.Width / 2 - 330, frame.Height - 80, 660, 36), Theme.TextMuted, 2);
    }

    private int GetHoveredMenuIndex(Point mousePosition, int rowCount, int top)
    {
        for (var index = 0; index < rowCount; index++)
        {
            if (GetMenuRowBounds(index, top).Contains(mousePosition))
            {
                return index;
            }
        }

        return -1;
    }

    private Rectangle GetMenuRowBounds(int index, int top)
    {
        var frame = GraphicsDevice.Viewport.Bounds;
        return new Rectangle(frame.Width / 2 - 260, top + index * 62, 520, 50);
    }

    private IReadOnlyList<string> GetMainMenuItems()
    {
        return
        [
            "CONTINUE GAME",
            "LOAD GAME",
            "NEW GAME",
            "OPTIONS",
            "EXIT"
        ];
    }

    private IReadOnlyList<(string Label, string Value, bool IsBack)> GetOptionsRows()
    {
        var resolution = _options.SelectedResolution;
        return
        [
            ("RESOLUTION", $"{resolution.X} x {resolution.Y}", false),
            ("WINDOW MODE", _options.WindowMode.ToString().ToUpperInvariant(), false),
            ("MASTER VOLUME", ToPercentText(_options.MasterVolume), false),
            ("MUSIC VOLUME", ToPercentText(_options.MusicVolume), false),
            ("SFX VOLUME", ToPercentText(_options.SfxVolume), false),
            ("BACK", "RETURN TO MAIN MENU", true)
        ];
    }

    private static string ToPercentText(float value)
    {
        return $"{Math.Round(value * 100f):0}%";
    }

    private void StartNewGame(int seed)
    {
        AttachSession(InitialGameStateFactory.Create(seed), seed);
    }

    private void AttachSession(GameSession session, int seed)
    {
        _session = session;
        _screenManager = new ScreenManager(
            session,
            () => _saveGameStore.Save(session, seed),
            () =>
            {
                _mode = AppMode.MainMenu;
                _menuMessage = "PAUSED";
                _saveGameStore.Save(session, seed);
            });
        _saveGameStore.Save(session, seed);
    }
}
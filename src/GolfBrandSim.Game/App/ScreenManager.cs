using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.Screens;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.App;

public sealed class ScreenManager
{
    private readonly IReadOnlyList<IScreen> _screens;
    private readonly Action? _onWeekAdvanced;
    private readonly Action? _onReturnToMainMenu;
    private int _selectedIndex;
    private int _hoveredTabIndex = -1;
    private bool _advanceButtonHovered;
    private bool _mainMenuButtonHovered;

    public ScreenManager(GameSession session, Action? onWeekAdvanced = null, Action? onReturnToMainMenu = null)
    {
        Session = session;
        _onWeekAdvanced = onWeekAdvanced;
        _onReturnToMainMenu = onReturnToMainMenu;
        _screens =
        [
            new DashboardScreen(),
            new GolfersScreen(),
            new ContractsScreen(),
            new StandingsScreen(),
            new CalendarScreen(),
            new TournamentResultsScreen(),
            new FinanceScreen(),
            new ProductManagementScreen(),
            new ResearchScreen(),
            new SponsorshipMarketScreen()
        ];
    }

    public GameSession Session { get; }

    private IScreen ActiveScreen => _screens[_selectedIndex];

    public void Update(InputState input, Rectangle viewportBounds)
    {
        var tabBounds = GetTabStripBounds(viewportBounds);
        var contentBounds = GetContentBounds(viewportBounds);
        var advanceButtonBounds = GetAdvanceButtonBounds(viewportBounds);
        var mainMenuButtonBounds = GetMainMenuButtonBounds(viewportBounds);

        _hoveredTabIndex = GetTabIndexAtPoint(input.MousePosition, tabBounds);
        _advanceButtonHovered = advanceButtonBounds.Contains(input.MousePosition);
        _mainMenuButtonHovered = mainMenuButtonBounds.Contains(input.MousePosition);

        if (input.IsNewLeftClick() && _hoveredTabIndex >= 0)
        {
            _selectedIndex = _hoveredTabIndex;
        }

        if (input.IsNewLeftClick() && _advanceButtonHovered && Session.CanAdvanceWeek)
        {
            Session.AdvanceWeek();
            _selectedIndex = 3;
            _onWeekAdvanced?.Invoke();
        }

        if (input.IsNewLeftClick() && _mainMenuButtonHovered)
        {
            _onReturnToMainMenu?.Invoke();
            return;
        }

        ActiveScreen.HandleInput(input, Session, contentBounds);
    }

    public void Draw(UiContext ui)
    {
        var frame = ui.ViewportBounds;
        var contentBounds = GetContentBounds(frame);

        DrawHeader(ui, frame);
        DrawTabs(ui, GetTabStripBounds(frame));
        ActiveScreen.Draw(ui, Session, contentBounds);
        DrawFooter(ui, frame);
    }

    private void DrawHeader(UiContext ui, Rectangle frame)
    {
        ui.FillRectangle(new Rectangle(0, 0, frame.Width, 74), Theme.Header);
        ui.DrawText("GOLF BRAND SIM", new Vector2(40, 14), Theme.TextPrimary, 3);

        var weekText = Session.State.IsSeasonComplete
            ? "SEASON COMPLETE"
            : $"{Formatters.WeekLabel(Session.State.CurrentWeekNumber)}  {Session.State.NextTournament?.Name}";

        ui.DrawText(weekText, new Vector2(frame.Width - ui.MeasureText(weekText, 2).X - 40, 25), Theme.TextMuted, 2);
    }

    private void DrawTabs(UiContext ui, Rectangle bounds)
    {
        var tabWidth = bounds.Width / _screens.Count;
        for (var index = 0; index < _screens.Count; index++)
        {
            var tabBounds = new Rectangle(bounds.X + index * tabWidth, bounds.Y, tabWidth - 8, bounds.Height);
            var active = index == _selectedIndex;
            var hovered = index == _hoveredTabIndex;
            var tabColor = active ? Theme.Accent : hovered ? Theme.HighlightRow : Theme.PanelRaised;
            ui.FillRectangle(tabBounds, tabColor);
            ui.DrawBorder(tabBounds, active ? Theme.AccentHighlight : Theme.PanelBorder, 2);
            ui.DrawCenteredText(_screens[index].TabLabel, tabBounds, active ? Theme.Header : Theme.TextPrimary, 2);
        }
    }

    private void DrawFooter(UiContext ui, Rectangle frame)
    {
        var footerBounds = new Rectangle(40, frame.Height - 46, frame.Width - 80, 28);
        ui.DrawText("CLICK TABS TO NAVIGATE. USE ADVANCE WEEK OR MAIN MENU BUTTONS.", new Vector2(footerBounds.X, footerBounds.Y), Theme.TextMuted, 2);

        var menuBounds = GetMainMenuButtonBounds(frame);
        ui.FillRectangle(menuBounds, _mainMenuButtonHovered ? Theme.HighlightRow : Theme.PanelRaised);
        ui.DrawBorder(menuBounds, Theme.PanelBorder, 2);
        ui.DrawCenteredText("MAIN MENU", menuBounds, Theme.TextPrimary, 2);

        var advanceBounds = GetAdvanceButtonBounds(frame);
        ui.FillRectangle(advanceBounds, _advanceButtonHovered ? Theme.AccentHighlight : Theme.Accent);
        ui.DrawBorder(advanceBounds, Theme.PanelBorder, 2);
        ui.DrawCenteredText(Session.CanAdvanceWeek ? "ADVANCE WEEK" : "SEASON COMPLETE", advanceBounds, Theme.Header, 2);
    }

    private static Rectangle GetTabStripBounds(Rectangle frame)
    {
        return new Rectangle(40, 86, frame.Width - 80, 36);
    }

    private static Rectangle GetContentBounds(Rectangle frame)
    {
        return new Rectangle(40, 130, frame.Width - 80, frame.Height - 180);
    }

    private static Rectangle GetAdvanceButtonBounds(Rectangle frame)
    {
        return new Rectangle(frame.Width - 290, frame.Height - 54, 250, 34);
    }

    private static Rectangle GetMainMenuButtonBounds(Rectangle frame)
    {
        return new Rectangle(frame.Width - 480, frame.Height - 54, 180, 34);
    }

    private int GetTabIndexAtPoint(Point point, Rectangle bounds)
    {
        if (!bounds.Contains(point))
        {
            return -1;
        }

        var tabWidth = bounds.Width / _screens.Count;
        var relativeX = point.X - bounds.X;
        var index = Math.Clamp(relativeX / tabWidth, 0, _screens.Count - 1);
        return index;
    }
}
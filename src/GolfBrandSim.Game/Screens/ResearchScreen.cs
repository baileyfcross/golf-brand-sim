using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class ResearchScreen : IScreen
{
    private string _statusMessage = string.Empty;

    public string TabLabel => "RESEARCH";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
        if (!input.IsNewLeftClick()) return;

        var frame = GetFrameBounds(bounds);
        var tracks = session.State.PlayerBrand.ResearchTracks.ToList();
        for (var i = 0; i < tracks.Count; i++)
        {
            var panelBounds = GetTrackPanelBounds(frame, i);
            var smallBtn = GetInvestButtonBounds(panelBounds, 0);
            var bigBtn = GetInvestButtonBounds(panelBounds, 1);

            if (smallBtn.Contains(input.MousePosition))
            {
                if (session.State.PlayerBrand.InvestInResearch(tracks[i].Category, 5_000m))
                    _statusMessage = $"+5K TO {tracks[i].Category.ToString().ToUpperInvariant()}";
                else
                    _statusMessage = "NOT ENOUGH CASH";
                return;
            }

            if (bigBtn.Contains(input.MousePosition))
            {
                if (session.State.PlayerBrand.InvestInResearch(tracks[i].Category, 25_000m))
                    _statusMessage = $"+25K TO {tracks[i].Category.ToString().ToUpperInvariant()}";
                else
                    _statusMessage = "NOT ENOUGH CASH";
                return;
            }
        }
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "RESEARCH & DEVELOPMENT");

        var frame = GetFrameBounds(bounds);
        var brand = session.State.PlayerBrand;

        ui.DrawText("INVEST CASH TO UNLOCK PRODUCT CATEGORIES  ($5K = +1 POINT, THRESHOLD = 30)",
            new Vector2(frame.X, frame.Y), Theme.TextMuted, 2);

        var tracks = brand.ResearchTracks.ToList();
        for (var i = 0; i < tracks.Count; i++)
        {
            var track = tracks[i];
            var panelBounds = GetTrackPanelBounds(frame, i);

            ui.FillRectangle(panelBounds, Theme.PanelRaised);
            ui.DrawBorder(panelBounds, Theme.PanelBorder, 2);

            // Category label
            var labelColor = track.IsUnlocked ? Theme.AccentHighlight : Theme.Header;
            ui.DrawText(track.Category.ToString().ToUpperInvariant(), new Vector2(panelBounds.X + 16, panelBounds.Y + 16), labelColor, 3);

            // Status
            var status = track.IsUnlocked ? "UNLOCKED" : $"{track.ProgressPoints} / {track.UnlockThreshold} PTS";
            ui.DrawText(status, new Vector2(panelBounds.X + 16, panelBounds.Y + 50), Theme.TextPrimary, 2);

            // Progress bar background
            var barBg = new Rectangle(panelBounds.X + 16, panelBounds.Y + 78, panelBounds.Width - 32, 18);
            ui.FillRectangle(barBg, Theme.PanelBorder);

            // Progress bar fill
            var pct = track.IsUnlocked ? 1f : Math.Min(1f, (float)track.ProgressPoints / track.UnlockThreshold);
            var fillWidth = (int)(barBg.Width * pct);
            if (fillWidth > 0)
            {
                var barFill = new Rectangle(barBg.X, barBg.Y, fillWidth, barBg.Height);
                ui.FillRectangle(barFill, track.IsUnlocked ? Theme.AccentHighlight : Theme.Accent);
            }

            // Invest buttons (hidden if already unlocked)
            if (!track.IsUnlocked)
            {
                var btn5 = GetInvestButtonBounds(panelBounds, 0);
                var btn25 = GetInvestButtonBounds(panelBounds, 1);
                ui.FillRectangle(btn5, Theme.Accent);
                ui.DrawBorder(btn5, Theme.AccentHighlight, 1);
                ui.DrawCenteredText("+$5K", btn5, Theme.Header, 2);
                ui.FillRectangle(btn25, Theme.Accent);
                ui.DrawBorder(btn25, Theme.AccentHighlight, 1);
                ui.DrawCenteredText("+$25K", btn25, Theme.Header, 2);
            }
        }

        if (_statusMessage.Length > 0)
        {
            var msgY = frame.Y + tracks.Count * (TrackPanelHeight + 20) + 10;
            ui.DrawText(_statusMessage, new Vector2(frame.X, msgY), Theme.TextPrimary, 2);
        }
    }

    private const int TrackPanelHeight = 130;

    private static Rectangle GetFrameBounds(Rectangle bounds) =>
        new(bounds.X + 16, bounds.Y + 52, bounds.Width - 32, bounds.Height - 68);

    private static Rectangle GetTrackPanelBounds(Rectangle frame, int index) =>
        new(frame.X, frame.Y + 30 + index * (TrackPanelHeight + 20), 500, TrackPanelHeight);

    private static Rectangle GetInvestButtonBounds(Rectangle panel, int index) =>
        new(panel.X + 16 + index * 100, panel.Y + 100, 86, 26);
}

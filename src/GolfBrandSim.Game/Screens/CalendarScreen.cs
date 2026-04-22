using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class CalendarScreen : IScreen
{
    public string TabLabel => "CALENDAR";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "SEASON SCHEDULE");

        var rows = session.State.SeasonSchedule.Tournaments
            .Select(tournament => new[]
            {
                Formatters.WeekLabel(tournament.WeekNumber),
                tournament.IsMajor ? "MAJOR" : "STANDARD",
                tournament.Name,
                tournament.VenueName,
                Formatters.Money(tournament.Purse),
                GetStatusLabel(tournament.WeekNumber, session)
            })
            .ToArray();

        var highlightedIndex = Math.Clamp(session.State.CurrentWeekNumber - 1, 0, rows.Length - 1);

        UiToolkit.DrawTable(
            ui,
            new Rectangle(bounds.X + 16, bounds.Y + 52, bounds.Width - 32, bounds.Height - 68),
            ["WEEK", "TYPE", "TOURNAMENT", "VENUE", "PURSE", "STATUS"],
            [90, 120, 280, 260, 120, 120],
            rows,
            highlightedIndex);
    }

    private static string GetStatusLabel(int weekNumber, GameSession session)
    {
        if (session.State.IsSeasonComplete)
        {
            return "DONE";
        }

        if (weekNumber < session.State.CurrentWeekNumber)
        {
            return "DONE";
        }

        return weekNumber == session.State.CurrentWeekNumber ? "NEXT" : "UPCOMING";
    }
}
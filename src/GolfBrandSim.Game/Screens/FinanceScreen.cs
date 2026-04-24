using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class FinanceScreen : IScreen
{
    public string TabLabel => "FINANCE";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        var ledger = session.State.FinanceLedger.Entries;
        var productRevenue = ledger.Where(entry => entry.Type == FinanceEntryType.ProductRevenue).Sum(entry => entry.Amount);
        var sponsorshipRevenue = ledger.Where(entry => entry.Type == FinanceEntryType.SponsorshipIncome).Sum(entry => entry.Amount);
        var operatingCost = ledger.Where(entry => entry.Type == FinanceEntryType.OperatingExpense).Sum(entry => entry.Amount);

        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X, bounds.Y, 250, 100), "CASH", Formatters.Money(session.State.PlayerBrand.CashBalance), "CURRENT BALANCE");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 270, bounds.Y, 250, 100), "PRODUCTS", Formatters.Money(productRevenue), "SEASON TO DATE");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 540, bounds.Y, 250, 100), "SPONSORSHIP", Formatters.Money(sponsorshipRevenue), "SEASON TO DATE");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 810, bounds.Y, 250, 100), "RETAINERS", Formatters.Money(operatingCost), "CASH OUT");
        UiToolkit.DrawSummaryCard(ui, new Rectangle(bounds.X + 1080, bounds.Y, 250, 100), "LAST WEEK", session.State.LastWeekResult is null ? "NO DATA" : Formatters.Money(session.State.LastWeekResult.NetCashChange), "NET CHANGE");

        UiToolkit.DrawPanel(ui, new Rectangle(bounds.X, bounds.Y + 130, bounds.Width, bounds.Height - 130), "LEDGER");

        var rows = ledger
            .OrderByDescending(entry => entry.WeekNumber)
            .Take(16)
            .Select(entry => new[]
            {
                entry.WeekNumber == 0 ? "START" : Formatters.WeekLabel(entry.WeekNumber),
                entry.Type.ToString().ToUpperInvariant(),
                entry.Description.ToUpperInvariant(),
                Formatters.Money(entry.Amount)
            })
            .ToArray();

        UiToolkit.DrawTable(
            ui,
            new Rectangle(bounds.X + 16, bounds.Y + 182, bounds.Width - 32, bounds.Height - 198),
            ["WEEK", "TYPE", "DETAIL", "AMOUNT"],
            [100, 180, 540, 160],
            rows);
    }
}
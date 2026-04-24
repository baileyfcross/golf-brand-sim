using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.UI;

public static class UiToolkit
{
    public static void DrawPanel(UiContext ui, Rectangle bounds, string title)
    {
        ui.FillRectangle(bounds, Theme.Panel);
        ui.DrawBorder(bounds, Theme.PanelBorder, 2);

        var titleBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, 44);
        ui.FillRectangle(titleBounds, Theme.PanelRaised);
        ui.DrawBorder(titleBounds, Theme.PanelBorder, 2);
        ui.DrawText(title, new Vector2(bounds.X + 14, bounds.Y + 9), Theme.TextPrimary, 2);
    }

    public static void DrawSummaryCard(UiContext ui, Rectangle bounds, string label, string value, string footer)
    {
        ui.FillRectangle(bounds, Theme.PanelRaised);
        ui.DrawBorder(bounds, Theme.PanelBorder, 2);

        ui.DrawText(label, new Vector2(bounds.X + 14, bounds.Y + 10), Theme.TextMuted, 1);
        ui.DrawText(value, new Vector2(bounds.X + 14, bounds.Y + 34), Theme.TextPrimary, 2);
        ui.DrawText(footer, new Vector2(bounds.X + 14, bounds.Bottom - 18), Theme.TextMuted, 1);
    }

    public static void DrawTable(
        UiContext ui,
        Rectangle bounds,
        IReadOnlyList<string> headers,
        IReadOnlyList<int> columnWidths,
        IReadOnlyList<string[]> rows,
        int highlightedRowIndex = -1)
    {
        ui.FillRectangle(bounds, Theme.Panel);
        ui.DrawBorder(bounds, Theme.PanelBorder, 1);

        var headerHeight = 32;
        var rowHeight = 28;
        var maxRows = Math.Max(0, (bounds.Height - headerHeight) / rowHeight);

        var headerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, headerHeight);
        ui.FillRectangle(headerBounds, Theme.PanelRaised);

        var cursorX = bounds.X;
        for (var index = 0; index < headers.Count; index++)
        {
            var width = columnWidths[index];
            var cellBounds = new Rectangle(cursorX, bounds.Y, width, headerHeight);
            ui.DrawBorder(cellBounds, Theme.PanelBorder, 1);
            ui.DrawText(headers[index], new Vector2(cellBounds.X + 8, cellBounds.Y + 10), Theme.TextPrimary, 1);
            cursorX += width;
        }

        for (var rowIndex = 0; rowIndex < Math.Min(rows.Count, maxRows); rowIndex++)
        {
            var rowBounds = new Rectangle(bounds.X, bounds.Y + headerHeight + rowIndex * rowHeight, bounds.Width, rowHeight);
            if (rowIndex == highlightedRowIndex)
            {
                ui.FillRectangle(rowBounds, Theme.HighlightRow);
            }

            cursorX = bounds.X;
            for (var columnIndex = 0; columnIndex < headers.Count; columnIndex++)
            {
                var cellBounds = new Rectangle(cursorX, rowBounds.Y, columnWidths[columnIndex], rowHeight);
                ui.DrawBorder(cellBounds, Theme.PanelBorder, 1);
                var value = columnIndex < rows[rowIndex].Length ? rows[rowIndex][columnIndex] : string.Empty;
                ui.DrawText(value, new Vector2(cellBounds.X + 8, cellBounds.Y + 8), Theme.TextPrimary, 1);
                cursorX += columnWidths[columnIndex];
            }
        }
    }
}
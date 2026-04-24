using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Game.App;
using GolfBrandSim.Game.UI;
using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.Screens;

public sealed class ProductManagementScreen : IScreen
{
    private ProductCategory _selectedCategory = ProductCategory.Equipment;
    private ProductQualityTier _selectedTier = ProductQualityTier.Standard;
    private bool _confirmHovered;
    private string _statusMessage = string.Empty;

    public string TabLabel => "PRODUCTS";

    public void HandleInput(InputState input, GameSession session, Rectangle bounds)
    {
        var frame = GetFrameBounds(bounds);
        _confirmHovered = GetConfirmBounds(frame).Contains(input.MousePosition);

        if (input.IsNewLeftClick())
        {
            TryCategoryClick(input.MousePosition, frame);
            TryTierClick(input.MousePosition, frame);

            if (_confirmHovered)
                TryLaunchProduct(session);
        }
    }

    public void Draw(UiContext ui, GameSession session, Rectangle bounds)
    {
        UiToolkit.DrawPanel(ui, bounds, "PRODUCT MANAGEMENT");

        var state = session.State;
        var brand = state.PlayerBrand;
        var frame = GetFrameBounds(bounds);

        // Current products table
        var productRows = brand.Products
            .Select(p => new[]
            {
                p.Name.ToUpperInvariant(),
                p.Category.ToString().ToUpperInvariant(),
                p.QualityTier.ToString().ToUpperInvariant(),
                Formatters.Money(p.WeeklyRevenueBase),
                p.IsActive ? "ACTIVE" : "INACTIVE"
            })
            .ToArray();

        var tableRect = new Rectangle(frame.X, frame.Y, frame.Width, 200);
        UiToolkit.DrawTable(ui, tableRect, ["PRODUCT", "CATEGORY", "TIER", "BASE REVENUE", "STATUS"],
            [260, 130, 110, 140, 100], productRows);

        // Launch new product panel
        var launchTop = frame.Y + 220;
        ui.DrawText("LAUNCH NEW PRODUCT", new Vector2(frame.X, launchTop), Theme.Header, 2);

        // Category selector
        ui.DrawText("CATEGORY:", new Vector2(frame.X, launchTop + 34), Theme.TextMuted, 2);
        var categories = Enum.GetValues<ProductCategory>();
        for (var i = 0; i < categories.Length; i++)
        {
            var cat = categories[i];
            var catBounds = GetCategoryBounds(frame, i, launchTop);
            var track = brand.ResearchTracks.FirstOrDefault(t => t.Category == cat);
            var unlocked = track?.IsUnlocked == true;
            var selected = _selectedCategory == cat;
            var fillColor = selected ? Theme.Accent : (unlocked ? Theme.PanelRaised : Theme.PanelBorder);
            var textColor = selected ? Theme.Header : (unlocked ? Theme.TextPrimary : Theme.TextMuted);
            ui.FillRectangle(catBounds, fillColor);
            ui.DrawBorder(catBounds, selected ? Theme.AccentHighlight : Theme.PanelBorder, 1);
            ui.DrawCenteredText(cat.ToString().ToUpperInvariant() + (unlocked ? "" : " (LOCKED)"), catBounds, textColor, 2);
        }

        // Tier selector
        ui.DrawText("QUALITY TIER:", new Vector2(frame.X, launchTop + 106), Theme.TextMuted, 2);
        var tiers = Enum.GetValues<ProductQualityTier>();
        for (var i = 0; i < tiers.Length; i++)
        {
            var tier = tiers[i];
            var tierBounds = GetTierBounds(frame, i, launchTop);
            var selected = _selectedTier == tier;
            var fill = selected ? Theme.Accent : Theme.PanelRaised;
            var text = selected ? Theme.Header : Theme.TextPrimary;
            var cost = BrandProduct.GetLaunchCost(tier);
            ui.FillRectangle(tierBounds, fill);
            ui.DrawBorder(tierBounds, selected ? Theme.AccentHighlight : Theme.PanelBorder, 1);
            ui.DrawCenteredText($"{tier.ToString().ToUpperInvariant()} {Formatters.Money(cost)}", tierBounds, text, 2);
        }

        // Confirm button
        var confirmBounds = GetConfirmBounds(frame);
        var confirmFill = _confirmHovered ? Theme.AccentHighlight : Theme.Accent;
        ui.FillRectangle(confirmBounds, confirmFill);
        ui.DrawBorder(confirmBounds, Theme.Header, 2);
        ui.DrawCenteredText("LAUNCH PRODUCT", confirmBounds, Theme.Header, 2);

        // Status
        if (_statusMessage.Length > 0)
            ui.DrawText(_statusMessage, new Vector2(frame.X, launchTop + 210), Theme.TextPrimary, 2);
    }

    private void TryCategoryClick(Point point, Rectangle frame)
    {
        var categories = Enum.GetValues<ProductCategory>();
        for (var i = 0; i < categories.Length; i++)
        {
            if (GetCategoryBounds(frame, i, frame.Y + 220).Contains(point))
            {
                _selectedCategory = categories[i];
                return;
            }
        }
    }

    private void TryTierClick(Point point, Rectangle frame)
    {
        var tiers = Enum.GetValues<ProductQualityTier>();
        for (var i = 0; i < tiers.Length; i++)
        {
            if (GetTierBounds(frame, i, frame.Y + 220).Contains(point))
            {
                _selectedTier = tiers[i];
                return;
            }
        }
    }

    private void TryLaunchProduct(GameSession session)
    {
        var brand = session.State.PlayerBrand;
        var productName = $"{brand.Name} {_selectedTier} {_selectedCategory}".ToUpperInvariant();
        var success = brand.LaunchProduct(productName, _selectedCategory, _selectedTier);
        _statusMessage = success ? $"LAUNCHED: {productName}" : "CANNOT LAUNCH: CHECK CASH OR RESEARCH STATUS";
    }

    private static Rectangle GetFrameBounds(Rectangle bounds) =>
        new(bounds.X + 16, bounds.Y + 52, bounds.Width - 32, bounds.Height - 68);

    private static Rectangle GetCategoryBounds(Rectangle frame, int index, int top) =>
        new(frame.X + index * 310, top + 58, 300, 44);

    private static Rectangle GetTierBounds(Rectangle frame, int index, int top) =>
        new(frame.X + index * 270, top + 130, 260, 44);

    private static Rectangle GetConfirmBounds(Rectangle frame) =>
        new(frame.X, frame.Y + 220 + 194, 280, 44);
}

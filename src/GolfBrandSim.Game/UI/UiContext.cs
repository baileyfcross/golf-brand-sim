using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GolfBrandSim.Game.UI;

public sealed class UiContext
{
    public UiContext(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, PixelFont pixelFont, Rectangle viewportBounds)
    {
        SpriteBatch = spriteBatch;
        PrimitiveBatch = primitiveBatch;
        PixelFont = pixelFont;
        ViewportBounds = viewportBounds;
    }

    public SpriteBatch SpriteBatch { get; }

    public PrimitiveBatch PrimitiveBatch { get; }

    public PixelFont PixelFont { get; }

    public Rectangle ViewportBounds { get; }

    public void FillRectangle(Rectangle rectangle, Color color)
    {
        PrimitiveBatch.FillRectangle(SpriteBatch, rectangle, color);
    }

    public void DrawBorder(Rectangle rectangle, Color color, int thickness = 1)
    {
        PrimitiveBatch.DrawBorder(SpriteBatch, rectangle, color, thickness);
    }

    public void DrawText(string text, Vector2 position, Color color, int scale = 2)
    {
        PixelFont.DrawString(SpriteBatch, PrimitiveBatch, text, position, color, scale);
    }

    public Point MeasureText(string text, int scale = 2)
    {
        return PixelFont.MeasureString(text, scale);
    }

    public void DrawCenteredText(string text, Rectangle bounds, Color color, int scale = 2)
    {
        var size = MeasureText(text, scale);
        var x = bounds.X + (bounds.Width - size.X) / 2;
        var y = bounds.Y + (bounds.Height - size.Y) / 2;
        DrawText(text, new Vector2(x, y), color, scale);
    }
}
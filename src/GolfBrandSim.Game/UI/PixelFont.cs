using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GolfBrandSim.Game.UI;

public sealed class PixelFont
{
    private readonly SpriteFont _font;

    public PixelFont(SpriteFont font)
    {
        _font = font;
    }

    public void DrawString(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, string text, Vector2 position, Color color, int scale)
    {
        spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, (float)scale, SpriteEffects.None, 0f);
    }

    public Point MeasureString(string text, int scale)
    {
        var size = _font.MeasureString(text) * scale;
        return new Point((int)size.X, (int)size.Y);
    }
}
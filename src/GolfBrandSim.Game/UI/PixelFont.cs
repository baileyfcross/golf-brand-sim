using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GolfBrandSim.Game.UI;

public sealed class PixelFont
{
    private static readonly IReadOnlyDictionary<char, string[]> Glyphs = new Dictionary<char, string[]>
    {
        ['A'] = [".X.", "X.X", "XXX", "X.X", "X.X"],
        ['B'] = ["XX.", "X.X", "XX.", "X.X", "XX."],
        ['C'] = ["XX.", "X..", "X..", "X..", "XX."],
        ['D'] = ["XX.", "X.X", "X.X", "X.X", "XX."],
        ['E'] = ["XXX", "X..", "XX.", "X..", "XXX"],
        ['F'] = ["XXX", "X..", "XX.", "X..", "X.."],
        ['G'] = ["XX.", "X..", "X.X", "X.X", "XXX"],
        ['H'] = ["X.X", "X.X", "XXX", "X.X", "X.X"],
        ['I'] = ["XXX", ".X.", ".X.", ".X.", "XXX"],
        ['J'] = ["..X", "..X", "..X", "X.X", ".X."],
        ['K'] = ["X.X", "X.X", "XX.", "X.X", "X.X"],
        ['L'] = ["X..", "X..", "X..", "X..", "XXX"],
        ['M'] = ["X.X", "XXX", "XXX", "X.X", "X.X"],
        ['N'] = ["X.X", "XXX", "XXX", "XXX", "X.X"],
        ['O'] = ["XXX", "X.X", "X.X", "X.X", "XXX"],
        ['P'] = ["XX.", "X.X", "XX.", "X..", "X.."],
        ['Q'] = ["XXX", "X.X", "X.X", "XXX", "..X"],
        ['R'] = ["XX.", "X.X", "XX.", "X.X", "X.X"],
        ['S'] = ["XX.", "X..", "XXX", "..X", "XX."],
        ['T'] = ["XXX", ".X.", ".X.", ".X.", ".X."],
        ['U'] = ["X.X", "X.X", "X.X", "X.X", "XXX"],
        ['V'] = ["X.X", "X.X", "X.X", "X.X", ".X."],
        ['W'] = ["X.X", "X.X", "XXX", "XXX", "X.X"],
        ['X'] = ["X.X", "X.X", ".X.", "X.X", "X.X"],
        ['Y'] = ["X.X", "X.X", ".X.", ".X.", ".X."],
        ['Z'] = ["XXX", "..X", ".X.", "X..", "XXX"],
        ['0'] = ["XXX", "X.X", "X.X", "X.X", "XXX"],
        ['1'] = [".X.", "XX.", ".X.", ".X.", "XXX"],
        ['2'] = ["XXX", "..X", "XXX", "X..", "XXX"],
        ['3'] = ["XXX", "..X", "XXX", "..X", "XXX"],
        ['4'] = ["X.X", "X.X", "XXX", "..X", "..X"],
        ['5'] = ["XXX", "X..", "XXX", "..X", "XXX"],
        ['6'] = ["XXX", "X..", "XXX", "X.X", "XXX"],
        ['7'] = ["XXX", "..X", "..X", "..X", "..X"],
        ['8'] = ["XXX", "X.X", "XXX", "X.X", "XXX"],
        ['9'] = ["XXX", "X.X", "XXX", "..X", "XXX"],
        ['-'] = ["...", "...", "XXX", "...", "..."],
        [' '] = ["...", "...", "...", "...", "..."]
    };

    public void DrawString(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, string text, Vector2 position, Color color, int scale)
    {
        var cursorX = (int)position.X;
        var cursorY = (int)position.Y;

        foreach (var character in text.ToUpperInvariant())
        {
            if (character == '\n')
            {
                cursorX = (int)position.X;
                cursorY += 6 * scale;
                continue;
            }

            var glyph = Glyphs.TryGetValue(character, out var pattern) ? pattern : Glyphs[' '];
            DrawGlyph(spriteBatch, primitiveBatch, glyph, cursorX, cursorY, color, scale);
            cursorX += 4 * scale;
        }
    }

    public Point MeasureString(string text, int scale)
    {
        var lines = text.ToUpperInvariant().Split('\n');
        var width = lines.Max(line => line.Length) * 4 * scale;
        var height = lines.Length * 6 * scale;
        return new Point(width, height);
    }

    private static void DrawGlyph(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, string[] pattern, int x, int y, Color color, int scale)
    {
        for (var row = 0; row < pattern.Length; row++)
        {
            var rowText = pattern[row];
            for (var column = 0; column < rowText.Length; column++)
            {
                if (rowText[column] != 'X')
                {
                    continue;
                }

                primitiveBatch.FillRectangle(spriteBatch, new Rectangle(x + column * scale, y + row * scale, scale, scale), color);
            }
        }
    }
}
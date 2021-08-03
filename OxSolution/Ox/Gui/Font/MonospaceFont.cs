using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Font
{
    /// <summary>
    /// A monospace font used to draw text.
    /// </summary>
    public class MonospaceFont : Disposable, IFont
    {
        /// <summary>
        /// Create a MonospaceFont.
        /// </summary>
        /// <param name="fontName">See property FontName.</param>
        /// <param name="glyphSheetSize">See property GlyphSheetSize.</param>
        /// <param name="glyphSheetFileName">See property GlyphSheetFileName.</param>
        /// <param name="glyphSize">See property GlyphSize.</param>
        public MonospaceFont(string fontName, Point glyphSheetSize, string glyphSheetFileName, Vector2 glyphSize)
        {
            OxHelper.ArgumentNullCheck(fontName, glyphSheetFileName);
            this.fontName = fontName;
            this.glyphSheetSize = glyphSheetSize;
            this.glyphSheetFileName = glyphSheetFileName;
            this.glyphSize = glyphSize;
        }

        /// <summary>
        /// The size of each individual glyph character on the glyph sheet.
        /// </summary>
        public Vector2 GlyphSize { get { return glyphSize; } }

        /// <summary>
        /// The size of the glyph sheet.
        /// </summary>
        public Point GlyphSheetSize { get { return glyphSheetSize; } }

        /// <summary>
        /// The file name of the glyph sheet.
        /// </summary>
        public string GlyphSheetFileName { get { return glyphSheetFileName; } }

        /// <inheritdoc />
        public string FontName { get { return fontName; } }

        /// <inheritdoc />
        public IText CreateText()
        {
            return new MonospaceText(this);
        }
        
        /// <summary>
        /// Calculate the position of a glyph on the glyph sheet.
        /// </summary>
        public Vector2 CalculateGlyphPositionOnSheet(char characterCode)
        {
            Vector2 result = Vector2.Zero;
            characterCode -= (char)33;
            int usableSpriteSheetWidth = (int)glyphSheetSize.X - ((int)glyphSheetSize.X % (int)glyphSize.X);
            result.X = (characterCode * glyphSize.X) % usableSpriteSheetWidth;
            result.Y = (int)((characterCode * glyphSize.X) / usableSpriteSheetWidth) * glyphSize.Y;
            result /= glyphSheetSize.X;
            return result;
        }

        /// <summary>
        /// Calculate the size of a glyph on the glyph sheet.
        /// </summary>
        public Vector2 CalculateGlyphSizeOnSheet()
        {
            return new Vector2(
                glyphSize.X / glyphSheetSize.X,
                glyphSize.Y / glyphSheetSize.Y);
        }

        private readonly string fontName;
        private readonly Vector2 glyphSize;
        private readonly Point glyphSheetSize;
        private readonly string glyphSheetFileName;
    }
}

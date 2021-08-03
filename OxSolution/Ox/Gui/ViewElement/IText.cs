using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.MathNamespace;
using Ox.Gui.Font;

namespace Ox.Gui.ViewElement
{
    /// <summary>
    /// Represents graphical text.
    /// </summary>
    public interface IText : IViewElement
    {
        /// <summary>
        /// The scale of the text.
        /// </summary>
        Vector2 Measure { get; }
        /// <summary>
        /// The font used by this text.
        /// </summary>
        IFont Font { get; }
        /// <summary>
        /// The color of the text.
        /// </summary>
        Color Color { get; set; }
        /// <summary>
        /// A rectangle composed of the position and scale of the text.
        /// </summary>
        Rect Rect { get; }
        /// <summary>
        /// The text string.
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// Get the position of a glyph at an index.
        /// </summary>
        Vector2 GetGlyphPosition(int index);
    }
}

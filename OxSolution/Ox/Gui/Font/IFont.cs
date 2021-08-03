using System;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Font
{
    /// <summary>
    /// Represents a font used to draw text.
    /// </summary>
    public interface IFont : IDisposable
    {
        /// <summary>
        /// The standard name of the font.
        /// </summary>
        string FontName { get; }
        /// <summary>
        /// Factory method for creating compatible IText objects
        /// </summary>
        IText CreateText();
    }
}

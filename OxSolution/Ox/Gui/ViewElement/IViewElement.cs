using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.MathNamespace;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.ViewElement
{
    /// <summary>
    /// Represent an individual element of a gui view.
    /// </summary>
    public interface IViewElement
    {
        /// <summary>
        /// The position of the element.
        /// </summary>
        Vector2 Position { get; set; }
        /// <summary>
        /// The scale of the element.
        /// </summary>
        Vector2 Scale { get; set; }
        /// <summary>
        /// The effect color applied to the element.
        /// </summary>
        Color EffectColor { get; set; }
        /// <summary>
        /// The clipping rectangle of the element.
        /// </summary>
        Rect Bounds { get; set; }
        /// <summary>
        /// The drawing inset for each sprite (used to prevent texture bleeding).
        /// </summary>
        float Inset { get; set; }
        /// <summary>
        /// The drawing Z of the element.
        /// </summary>
        float Z { get; set; }
        /// <summary>
        /// The visibility of the element.
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// Collect all the sprites in the element.
        /// </summary>
        IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result);
    }
}

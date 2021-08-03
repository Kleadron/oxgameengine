using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.MathNamespace;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// Represents the drawing strategy of a gui component.
    /// </summary>
    public interface IGuiView : IDisposable
    {
        /// <summary>
        /// The position of the view.
        /// </summary>
        Vector2 Position { get; set; }
        /// <summary>
        /// The scale of the view.
        /// </summary>
        Vector2 Scale { get; set; }
        /// <summary>
        /// The color of the component.
        /// </summary>
        Color Color { get; set; }
        /// <summary>
        /// The bounds of the view.
        /// </summary>
        Rect Bounds { get; set; }
        /// <summary>
        /// The Z of the view.
        /// </summary>
        float Z { get; set; }
        /// <summary>
        /// The state of the view's activity.
        /// </summary>
        bool Active { get; set; }
        /// <summary>
        /// The visbility of the view.
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// Is the view focused?
        /// </summary>
        bool Focused { get; set; }
        /// <summary>
        /// Is the view picked?
        /// </summary>
        bool Picked { get; set; }
        /// <summary>
        /// Update the view. Must be called once per game cycle.
        /// </summary>
        void Update(GameTime gameTime);
        /// <summary>
        /// Collect all the view's sprites.
        /// </summary>
        IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result);
    }
}

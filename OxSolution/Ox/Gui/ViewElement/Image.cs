using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.MathNamespace;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.ViewElement
{
    /// <summary>
    /// A graphical image.
    /// </summary>
    public class Image : IViewElement
    {
        /// <summary>
        /// The image's position on the texture sheet.
        /// </summary>
        public Vector2 SourcePosition
        {
            get { return sprite.SourcePosition; }
            set { sprite.SourcePosition = value; }
        }

        /// <summary>
        /// The image's size on the texture sheet.
        /// </summary>
        public Vector2 SourceSize
        {
            get { return sprite.SourceSize; }
            set { sprite.SourceSize = value; }
        }

        /// <inheritdoc />
        public Vector2 Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        /// <inheritdoc />
        public Vector2 Scale
        {
            get { return sprite.Scale; }
            set { sprite.Scale = value; }
        }

        /// <summary>
        /// The color of the image.
        /// </summary>
        public Color Color
        {
            get { return sprite.Color; }
            set { sprite.Color = value; }
        }

        /// <inheritdoc />
        public Color EffectColor
        {
            get { return sprite.EffectColor; }
            set { sprite.EffectColor = value; }
        }

        /// <inheritdoc />
        public Rect Bounds
        {
            get { return sprite.Bounds; }
            set { sprite.Bounds = value; }
        }

        /// <summary>
        /// The file name of the texture that covers the image.
        /// </summary>
        public string TextureFileName
        {
            get { return sprite.TextureFileName; }
            set { sprite.TextureFileName = value; }
        }

        /// <inheritdoc />
        public float Z
        {
            get { return sprite.Z; }
            set { sprite.Z = value; }
        }

        /// <inheritdoc />
        public float Inset
        {
            get { return sprite.Inset; }
            set { sprite.Inset = value; }
        }

        /// <inheritdoc />
        public bool Visible
        {
            get { return sprite.Visible; }
            set { sprite.Visible = value; }
        }

        /// <inheritdoc />
        public IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result)
        {
            result.Add(sprite);
            return result;
        }

        private readonly QuickSprite sprite = new QuickSprite();
    }
}

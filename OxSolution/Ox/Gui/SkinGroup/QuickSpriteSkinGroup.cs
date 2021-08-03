using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.SkinGroup
{
    /// <summary>
    /// Controls the visual properties of a group of quick sprites.
    /// </summary>
    public class QuickSpriteSkinGroup : IReadIndexable<QuickSprite>
    {
        /// <inheritdoc />
        public QuickSprite this[int index] { get { return sprites[index]; } }

        /// <summary>
        /// The texture's position on the texture sheet.
        /// </summary>
        public Vector2 SourcePosition
        {
            get { return sourcePosition; }
            set { sourcePosition = value; }
        }

        /// <summary>
        /// The texture's size on the texture sheet.
        /// </summary>
        public Vector2 SourceSize
        {
            get { return sourceSize; }
            set { sourceSize = value; }
        }

        /// <summary>
        /// The file name of the texture used to draw the sprites.
        /// </summary>
        public string TextureFileName
        {
            get { return textureFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                textureFileName = value;
            }
        }

        /// <summary>
        /// The drawing inset for the sprites (prevents texture bleeding).
        /// </summary>
        public float Inset
        {
            get { return inset; }
            set { inset = value; }
        }

        /// <inheritdoc />
        public int Count { get { return sprites.Count; } }

        /// <summary>
        /// Add a sprite to this group.
        /// </summary>
        public void AddSprite(QuickSprite sprite)
        {
            sprites.Add(sprite);
            SynchronizeSprite(sprite);
        }

        /// <summary>
        /// Remove a sprite from this group.
        /// </summary>
        public bool RemoveSprite(QuickSprite sprite)
        {
            return sprites.Remove(sprite);
        }

        /// <summary>
        /// Synchronize the properties of the sprites with those defined by this group.
        /// </summary>
        public void SynchronizeSprites()
        {
            for (int i = 0; i < sprites.Count; ++i) SynchronizeSprite(sprites[i]);
        }

        private void SynchronizeSprite(QuickSprite sprite)
        {
            sprite.SourcePosition = sourcePosition;
            sprite.SourceSize = sourceSize;
            sprite.TextureFileName = textureFileName;
            sprite.Inset = inset;
        }

        private readonly IList<QuickSprite> sprites = new List<QuickSprite>();
        private Vector2 sourcePosition = Vector2.Zero;
        private Vector2 sourceSize = Vector2.One;
        private string textureFileName = "Ox/Textures/blue";
        private float inset;
    }
}

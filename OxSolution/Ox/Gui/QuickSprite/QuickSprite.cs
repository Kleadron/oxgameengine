using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.MathNamespace;

namespace Ox.Gui.QuickSpriteNamespace
{
    /// <summary>
    /// A functionally-thin sprite that can't draw itself.
    /// </summary>
    public class QuickSprite
    {
        /// <summary>
        /// Create a QuickSprite.
        /// </summary>
        /// <param name="textureFileName">See property TextureFileName.</param>
        public QuickSprite(string textureFileName)
        {
            OxHelper.ArgumentNullCheck(textureFileName);
            this.TextureFileName = textureFileName;
        }

        /// <summary>
        /// Create a QuickSprite.
        /// </summary>
        public QuickSprite() { }

        /// <summary>
        /// The alpha blending mode in which the sprite is rendered.
        /// </summary>
        public SpriteBlendMode BlendMode
        {
            get { return blendMode; }
            set { blendMode = value; }
        }

        /// <summary>
        /// The image's position on the texture sheet.
        /// </summary>
        public Vector2 SourcePosition
        {
            get { return sourcePosition; }
            set { sourcePosition = value; }
        }

        /// <summary>
        /// The image's size on the texture sheet.
        /// </summary>
        public Vector2 SourceSize
        {
            get { return sourceSize; }
            set { sourceSize = value; }
        }

        /// <summary>
        /// The position of the sprite.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// The scale of the sprite.
        /// </summary>
        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// The color of the sprite.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// The effect color against which the normal color is multiplied.
        /// </summary>
        public Color EffectColor
        {
            get { return effectColor; }
            set { effectColor = value; }
        }

        /// <summary>
        /// The clipping rectangle of the sprite.
        /// </summary>
        public Rect Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }
        
        /// <summary>
        /// The file name of the texture used to draw the sprite.
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
        /// The drawing Z of the sprite.
        /// </summary>
        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        /// <summary>
        /// The drawing inset for each sprite (prevents texture bleeding).
        /// </summary>
        public float Inset
        {
            get { return inset; }
            set { inset = value; }
        }

        /// <summary>
        /// The visibility of the sprite.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        private SpriteBlendMode blendMode = SpriteBlendMode.AlphaBlend;
        private Vector2 position;
        private Vector2 scale = new Vector2(8, 8);
        private Vector2 sourcePosition;
        private Vector2 sourceSize = Vector2.One;
        private Color color = Color.White;
        private Color effectColor = Color.White;
        private Rect bounds = OxConfiguration.VirtualScreen;
        private string textureFileName = "Ox/Textures/blue";
        private float z;
        private float inset;
        private bool visible = true;
    }
}

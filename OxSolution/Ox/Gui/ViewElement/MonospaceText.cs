using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Gui.Font;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.ViewElement
{
    /// <summary>
    /// Graphical text that uses a monospace font.
    /// </summary>
    public class MonospaceText : IText
    {
        /// <summary>
        /// Create a MonospaceText.
        /// </summary>
        /// <param name="font">The monospace font used to draw the text.</param>
        public MonospaceText(MonospaceFont font)
        {
            OxHelper.ArgumentNullCheck(font);
            MonospaceFont = font;
        }

        /// <summary>
        /// The monospace font used by this text.
        /// </summary>
        public MonospaceFont MonospaceFont
        {
            get { return _font; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_font == value) return; // OPTIMIZATION
                _font = value;
                Update();
            }
        }

        /// <inheritdoc />
        public Color EffectColor
        {
            get { return _effectColor; }
            set
            {
                if (_effectColor == value) return; // OPTIMIZATION
                _effectColor = value;
                Update();
            }
        }

        /// <inheritdoc />
        public IFont Font { get { return _font; } }

        /// <inheritdoc />
        public Rect Rect { get { return new Rect(Position.X, Position.Y, measure.X, measure.Y); } }

        /// <inheritdoc />
        public Rect Bounds
        {
            get { return _bounds; }
            set
            {
                if (_bounds.Equals(value)) return; // OPTIMIZATION
                _bounds = value;
                Update();
            }
        }

        /// <inheritdoc />
        public float Inset
        {
            get { return _inset; }
            set
            {
                if (_inset == value) return; // OPTIMIZATION
                _inset = value;
                Update();
            }
        }

        /// <inheritdoc />
        public float Z
        {
            get { return _z; }
            set
            {
                if (_z == value) return; // OPTIMIZATION
                _z = value;
                Update();
            }
        }

        /// <inheritdoc />
        public string Text
        {
            get { return _text; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_text == value) return; // OPTIMIZATION
                _text = value;
                Update();
            }
        }

        /// <inheritdoc />
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return; // OPTIMIZATION
                _position = value;
                Update();
            }
        }

        /// <inheritdoc />
        public Vector2 Measure
        {
            get { return measure; }
        }

        /// <inheritdoc />
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return; // OPTIMIZATION
                _scale = value;
                Update();
            }
        }

        /// <inheritdoc />
        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value) return; // OPTIMIZATION
                _color = value;
                Update();
            }
        }

        /// <inheritdoc />
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return; // OPTIMIZATION
                _visible = value;
                Update();
            }
        }

        /// <inheritdoc />
        public IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result)
        {
            System.Diagnostics.Debug.Assert(Text.Length <= glyphs.Count,
                "There should never be more text characters than glyphs during CollectGlyphs()!");
            for (int i = 0; i < Text.Length; ++i) result.Add(glyphs[i]);
            return result;
        }

        /// <inheritdoc />
        public Vector2 GetGlyphPosition(int index)
        {
            Vector2 characterPosition = Vector2.Zero;
            int currentIndex = 0;
            for (int i = 0; i < Text.Length; ++i)
            {
                if (index == currentIndex) break;                
                char character = Text[i];
                if (character == '\n')
                {
                    characterPosition.X = 0;
                    characterPosition.Y += Scale.Y;
                }
                else characterPosition.X += Scale.X;
                ++currentIndex;
            }
            if (index != currentIndex) characterPosition.X += Scale.X;
            return characterPosition;
        }

        private void Update()
        {
            UpdateGlyphs();
            UpdateScale();
        }

        private void UpdateGlyphs()
        {
            Point characterOffset = new Point();
            for (int i = 0; i < Text.Length; ++i)
            {
                char character = Text[i];
                if (i == glyphs.Count) glyphs.Add(new QuickSprite());
                ConfigureGlyph(glyphs[i], character, characterOffset);
                if (character == '\n')
                {
                    characterOffset.X = 0;
                    ++characterOffset.Y;
                }
                else ++characterOffset.X;
            }
            RemoveUnusedGlyphs();
        }

        private void RemoveUnusedGlyphs()
        {
            // OPTIMIZATION: if we have more than twice as many glyphs as we need, remove all the
            // unecessary ones. This helps to avoid thrashing the GC when changing the length of
            // text strings.
            if (glyphs.Count / 2 > Text.Length)
                while (glyphs.Count > Text.Length)
                    glyphs.RemoveAt(glyphs.Count - 1);
        }

        private void UpdateScale()
        {
            float currentWidth = 0;
            float maxWidth = 0;
            float height = Scale.Y;
            for (int i = 0; i < Text.Length; ++i)
            {
                char character = Text[i];
                if (character == '\n')
                {
                    height += Scale.Y;
                    currentWidth = 0;
                }
                else
                {
                    currentWidth += Scale.X;
                    if (maxWidth < currentWidth) maxWidth = currentWidth;
                }
            }
            measure = new Vector2(maxWidth, height);
        }

        private void ConfigureGlyph(QuickSprite sprite, char characterCode, Point characterOffset)
        {
            sprite.TextureFileName = MonospaceFont.GlyphSheetFileName;
            sprite.Position = new Vector2(
                Position.X + characterOffset.X * Scale.X,
                Position.Y + characterOffset.Y * Scale.Y);
            sprite.Scale = Scale;
            sprite.Color = Color;
            sprite.EffectColor = EffectColor;
            sprite.Bounds = Bounds;
            sprite.Z = Z;
            sprite.Inset = Inset;
            sprite.SourcePosition = MonospaceFont.CalculateGlyphPositionOnSheet(characterCode);
            sprite.SourceSize = MonospaceFont.CalculateGlyphSizeOnSheet();
        }

        private readonly IList<QuickSprite> glyphs = new List<QuickSprite>();
        private Vector2 measure;
        private MonospaceFont _font;
        private Vector2 _position;
        private Vector2 _scale = new Vector2(16, 32);
        private Color _color = Color.White;
        private Color _effectColor = Color.White;
        private Rect _bounds = OxConfiguration.VirtualScreen;
        private string _text = string.Empty;
        private float _z;
        private float _inset = 1.0f / 256.0f;
        private bool _visible = true;
    }
}

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.ViewElement
{
    /// <summary>
    /// The corner or side on which a border piece resides.
    /// </summary>
    public enum BorderPiece
    {
        North = 0,
        Northeast,
        East,
        Southeast,
        South,
        Southwest,
        West,
        Northwest,
        Count
    }

    /// <summary>
    /// A graphical border decoration.
    /// </summary>
    public class Border : IViewElement
    {
        /// <summary>
        /// Create a Border.
        /// </summary>
        public Border()
        {
            pieces = CreatePieces();
            UpdateDimensions();
        }

        /// <inheritdoc />
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                UpdateDimensions();
            }
        }

        /// <inheritdoc />
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                UpdateDimensions();
            }
        }

        /// <inheritdoc />
        public Color EffectColor
        {
            get { return _effectColor; }
            set
            {
                _effectColor = value;
                UpdateColor();
            }
        }

        /// <inheritdoc />
        public Rect Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                UpdateBounds();
            }
        }

        /// <summary>
        /// The thickness of each border sprite.
        /// </summary>
        public float Thickness
        {
            get { return _thickness; }
            set
            {
                _thickness = value;
                UpdateDimensions();
            }
        }

        /// <inheritdoc />
        public float Z
        {
            get { return _z; }
            set
            {
                _z = value;
                UpdateZ();
            }
        }

        /// <inheritdoc />
        public float Inset
        {
            get { return _inset; }
            set
            {
                _inset = value;
                UpdateInset();
            }
        }

        /// <inheritdoc />
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                UpdateVisible();
            }
        }

        /// <summary>
        /// Get a sprite on the border.
        /// </summary>
        public QuickSprite GetPiece(BorderPiece piece)
        {
            return pieces[(int)piece];
        }

        /// <inheritdoc />
        public IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result)
        {            
            result.AddRange(pieces);
            return result;
        }

        private QuickSprite[] CreatePieces()
        {
            QuickSprite[] result = new QuickSprite[(int)BorderPiece.Count];
            for (int i = 0; i < (int)BorderPiece.Count; ++i) result[i] = new QuickSprite();
            return result;
        }

        private void UpdateDimensions()
        {
            float maxThickness = Thickness;
            if (Scale.X < maxThickness * 2) maxThickness = Scale.X * 0.5f;
            if (Scale.Y < maxThickness * 2) maxThickness = Scale.Y * 0.5f;

            pieces[(int)BorderPiece.North].Position = Position + new Vector2(maxThickness, 0);
            pieces[(int)BorderPiece.North].Scale = new Vector2(Scale.X - maxThickness * 2, maxThickness);

            pieces[(int)BorderPiece.Northeast].Position = pieces[(int)BorderPiece.North].Position + new Vector2(Scale.X - maxThickness * 2, 0);
            pieces[(int)BorderPiece.Northeast].Scale = new Vector2(maxThickness, maxThickness);

            pieces[(int)BorderPiece.East].Position = pieces[(int)BorderPiece.Northeast].Position + new Vector2(0, maxThickness);
            pieces[(int)BorderPiece.East].Scale = new Vector2(maxThickness, Scale.Y - maxThickness * 2);

            pieces[(int)BorderPiece.Southeast].Position = pieces[(int)BorderPiece.East].Position + new Vector2(0, Scale.Y - maxThickness * 2);
            pieces[(int)BorderPiece.Southeast].Scale = new Vector2(maxThickness, maxThickness);

            pieces[(int)BorderPiece.South].Position = pieces[(int)BorderPiece.Southeast].Position - new Vector2(Scale.X - maxThickness * 2, 0);
            pieces[(int)BorderPiece.South].Scale = new Vector2(Scale.X - maxThickness * 2, maxThickness);

            pieces[(int)BorderPiece.Southwest].Position = pieces[(int)BorderPiece.South].Position - new Vector2(maxThickness, 0);
            pieces[(int)BorderPiece.Southwest].Scale = new Vector2(maxThickness, maxThickness);

            pieces[(int)BorderPiece.West].Position = pieces[(int)BorderPiece.Southwest].Position - new Vector2(0, Scale.Y - maxThickness * 2);
            pieces[(int)BorderPiece.West].Scale = new Vector2(maxThickness, Scale.Y - maxThickness * 2);

            pieces[(int)BorderPiece.Northwest].Position = pieces[(int)BorderPiece.West].Position - new Vector2(0, maxThickness);
            pieces[(int)BorderPiece.Northwest].Scale = new Vector2(maxThickness, maxThickness);
        }

        private void UpdateColor()
        {
            for (int i = 0; i < (int)BorderPiece.Count; ++i) pieces[i].EffectColor = EffectColor;
        }

        private void UpdateBounds()
        {
            for (int i = 0; i < (int)BorderPiece.Count; ++i) pieces[i].Bounds = Bounds;
        }

        private void UpdateZ()
        {
            for (int i = 0; i < (int)BorderPiece.Count; ++i) pieces[i].Z = Z;
        }

        private void UpdateInset()
        {
            for (int i = 0; i < (int)BorderPiece.Count; ++i) pieces[i].Inset = Inset;
        }

        private void UpdateVisible()
        {
            for (int i = 0; i < (int)BorderPiece.Count; ++i) pieces[i].Visible = Visible;
        }

        private readonly QuickSprite[] pieces = new QuickSprite[(int)BorderPiece.Count];
        private Vector2 _position;
        private Vector2 _scale = new Vector2(32, 32);
        private Color _effectColor = Color.White;
        private Rect _bounds = OxConfiguration.VirtualScreen;
        private float _thickness = 1;
        private float _z = GuiConfiguration.ZGap;
        private float _inset;
        private bool _visible = true;
    }
}

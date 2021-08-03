using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical view for a fill bar.
    /// </summary>
    public class FillBarView : GuiView
    {
        /// <summary>
        /// Create a FillBarView.
        /// </summary>
        public FillBarView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();

            skinGroups.LitSurface.AddImage(background);
            RegisterElement(background);

            skinGroups.Filling.AddImage(filling);
            RegisterElement(filling);

            skinGroups.SunkenBorder.AddBorder(border);
            RegisterElement(border);
        }

        /// <summary>
        /// The direction from which the bar is filled.
        /// </summary>
        public Direction2D FillMode
        {
            get { return _fillMode; }
            set
            {
                _fillMode = value;
                UpdateFilling();
            }
        }

        /// <summary>
        /// The [0..1] amount that the bar is filled.
        /// </summary>
        public float Fill
        {
            get { return _fill; }
            set
            {
                _fill = value;
                UpdateFilling();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
                skinGroups.LitSurface.RemoveImage(background);
                skinGroups.Filling.RemoveImage(filling);
                skinGroups.SunkenBorder.RemoveBorder(border);
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void UpdateZHook()
        {
            base.UpdateZHook();
            filling.Z = Z + GuiConfiguration.ZGap * 0.25f;
            border.Z = Z + GuiConfiguration.ZGap * 0.5f;
        }

        /// <inheritdoc />
        protected override void UpdatePositionHook()
        {
            base.UpdatePositionHook();
            UpdateFilling();
        }

        /// <inheritdoc />
        protected override void UpdateScaleHook()
        {
            base.UpdateScaleHook();
            UpdateFilling();
        }

        private void UpdateFilling()
        {
            switch (FillMode)
            {
                case Direction2D.Up: FillUpward(Position, Scale, Fill); break;
                case Direction2D.Down: FillDownward(Position, Scale, Fill); break;
                case Direction2D.Left: FillLeftward(Position, Scale, Fill); break;
                case Direction2D.Right: FillRightward(Position, Scale, Fill); break;
            }
        }

        private void FillRightward(Vector2 position, Vector2 scale, float fill)
        {
            filling.Scale = new Vector2(scale.X * fill, scale.Y);
            filling.Position = position;
        }

        private void FillLeftward(Vector2 position, Vector2 scale, float fill)
        {
            filling.Scale = new Vector2(scale.X * fill, scale.Y);
            filling.Position = new Vector2(scale.X - filling.Scale.X, 0) + position;
        }

        private void FillDownward(Vector2 position, Vector2 scale, float fill)
        {
            filling.Scale = new Vector2(scale.X, scale.Y * fill);
            filling.Position = position;
        }

        private void FillUpward(Vector2 position, Vector2 scale, float fill)
        {
            filling.Scale = new Vector2(scale.X, scale.Y * fill);
            filling.Position = new Vector2(0, scale.Y - filling.Scale.Y) + position;
        }

        private readonly Image background = new Image();
        private readonly Image filling = new Image();
        private readonly Border border = new Border();
        private Direction2D _fillMode;
        private float _fill;
    }
}

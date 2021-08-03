using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui.QuickSpriteNamespace;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A drawing strategy of a gui component.
    /// </summary>
    public class GuiView : Disposable
    {
        /// <summary>
        /// Create a GuiView.
        /// </summary>
        public GuiView(OxEngine engine, BaseGuiComponent parent)
        {
            OxHelper.ArgumentNullCheck(engine, parent);
            this.engine = engine;
            this.parent = parent;
            parent.TraitSet += parent_TraitSet;
        }

        /// <summary>
        /// The position of the view.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// The scale of the view.
        /// </summary>
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                UpdateScale();
            }
        }

        /// <summary>
        /// The color of the component.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateColor();
            }
        }

        /// <summary>
        /// The bounds of the view.
        /// </summary>
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
        /// The Z of the view.
        /// </summary>
        public float Z
        {
            get { return _z; }
            set
            {
                _z = value;
                UpdateZ();
            }
        }

        /// <summary>
        /// The state of the view's activity.
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                UpdateActive();
            }
        }

        /// <summary>
        /// The visbility of the view.
        /// </summary>
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
        /// Is the view focused?
        /// </summary>
        public bool Focused
        {
            get { return _focused; }
            set
            {
                _focused = value;
                UpdateFocused();
            }
        }

        /// <summary>
        /// Is the view picked?
        /// </summary>
        public bool Picked
        {
            get { return _picked; }
            set
            {
                _picked = value;
                UpdatePicked();
            }
        }

        /// <summary>
        /// Update the view. Must be called once per game cycle.
        /// </summary>
        public IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result)
        {
            OxHelper.ArgumentNullCheck(result);
            for (int i = 0; i < elements.Count; ++i) elements[i].CollectQuickSprites(result);
            return result;
        }

        /// <summary>
        /// Collect all the view's sprites.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            UpdateHook(gameTime);
        }

        /// <summary>
        /// The parent component.
        /// </summary>
        protected BaseGuiComponent Parent { get { return parent; } }

        /// <summary>
        /// The engine.
        /// </summary>
        protected OxEngine Engine { get { return engine; } }

        /// <summary>
        /// The engine as T.
        /// </summary>
        protected T GetEngine<T>() where T : class
        {
            return OxHelper.Cast<T>(engine);
        }

        /// <summary>
        /// Resgister an view element to be automatically transformed by the view.
        /// </summary>
        protected void RegisterElement(IViewElement element)
        {
            elements.Add(element);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) parent.TraitSet -= parent_TraitSet;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handle updating the view.
        /// </summary>
        protected virtual void UpdateHook(GameTime gameTime) { }

        /// <summary>
        /// Handle updating the visibility after any automatic transformation.
        /// </summary>
        protected virtual void UpdateVisibleHook() { }

        /// <summary>
        /// Handle updating the z position after any automatic transformation.
        /// </summary>
        protected virtual void UpdateZHook() { }

        /// <summary>
        /// Handle updating the x,y position after any automatic transformation.
        /// </summary>
        protected virtual void UpdatePositionHook() { }

        /// <summary>
        /// Handle updating the scale after any automatic transformation.
        /// </summary>
        protected virtual void UpdateScaleHook() { }

        /// <summary>
        /// Handle updating the color after any automatic transformation.
        /// </summary>
        protected virtual void UpdateColorHook() { }

        /// <summary>
        /// Handle updating the bounds after any automatic transformation.
        /// </summary>
        protected virtual void UpdateBoundsHook() { }

        /// <summary>
        /// Handle updating the active-ness.
        /// </summary>
        protected virtual void UpdateActiveHook() { }

        /// <summary>
        /// Handle updating the focus.
        /// </summary>
        protected virtual void UpdateFocusedHook() { }

        /// <summary>
        /// Handle updating the picked-ness.
        /// </summary>
        protected virtual void UpdatePickedHook() { }

        /// <summary>
        /// Handle updating the a trait of the specified name.
        /// </summary>
        protected virtual void TraitSetHook(string name) { }

        private void parent_TraitSet(OxComponent sender, string name)
        {
            OxHelper.ArgumentNullCheck(sender, name);
            TraitSetHook(name);
        }

        private void UpdateVisible()
        {
            for (int i = 0; i < elements.Count; ++i) elements[i].Visible = Visible;
            UpdateVisibleHook();
        }

        private void UpdateZ()
        {
            for (int i = 0; i < elements.Count; ++i) elements[i].Z = Z;
            UpdateZHook();
        }

        private void UpdatePosition()
        {
            for (int i = 0; i < elements.Count; ++i) elements[i].Position = Position;
            UpdatePositionHook();
        }

        private void UpdateScale()
        {
            for (int i = 0; i < elements.Count; ++i) elements[i].Scale = Scale;
            UpdateScaleHook();
        }

        private void UpdateColor()
        {
            for (int i = 0; i < elements.Count; ++i) elements[i].EffectColor = Color;
            UpdateColorHook();
        }

        private void UpdateBounds()
        {
            for (int i = 0; i < elements.Count; ++i) elements[i].Bounds = Bounds;
            UpdateBoundsHook();
        }

        private void UpdateActive()
        {
            UpdateActiveHook();
        }

        private void UpdateFocused()
        {
            UpdateFocusedHook();
        }

        private void UpdatePicked()
        {
            UpdatePickedHook();
        }

        private readonly IList<IViewElement> elements = new List<IViewElement>();
        private readonly BaseGuiComponent parent;
        private readonly OxEngine engine;
        private Vector2 _position;
        private Vector2 _scale;
        private Color _color = Color.White;
        private Rect _bounds;
        private float _z;
        private bool _active;
        private bool _visible;
        private bool _focused;
        private bool _picked;
    }
}

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Gui.QuickSpriteNamespace;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// Represents the state of a mouse cursor.
    /// </summary>
    public enum MouseMode
    {
        Normal = 0,
        Wait
    }

    /// <summary>
    /// A mouse cursor for a user interface.
    /// </summary>
    public class MouseComponent : InterleavedComponent
    {
        /// <summary>
        /// Create a MouseComponent.
        /// </summary>
        public MouseComponent(OxEngine engine, string domainName, bool isOwnedByDomain) :
            base(engine, domainName, isOwnedByDomain)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();

            Image normalMouse = new Image();
            skinGroups.NormalMouse.AddImage(normalMouse);
            cursors.Add(normalMouse);

            Image waitMouse = new Image();
            skinGroups.WaitMouse.AddImage(waitMouse);
            cursors.Add(waitMouse);

            Mode = MouseMode.Normal; // put object into a valid state
            Scale = new Vector2(32, 32); // put object into a valid state
        }

        /// <summary>
        /// The state.
        /// </summary>
        public MouseMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                for (int i = 0; i < cursors.Count; ++i) cursors[i].Visible = i == (int)_mode;
            }
        }

        /// <summary>
        /// The position.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                for (int i = 0; i < cursors.Count; ++i)
                {
                    // NOTE: since there is inaccuracy when translating from OS space to
                    // application space, the visual representation of the mouse must be offset.
                    cursors[i].Position = value - new Vector2(3);
                }
            }
        }

        /// <summary>
        /// The scale.
        /// </summary>
        public Vector2 Scale
        {
            get { return Cursor.Scale; }
            set { for (int i = 0; i < cursors.Count; ++i) cursors[i].Scale = value; }
        }

        /// <summary>
        /// The rectangle.
        /// </summary>
        public Rect Rect { get { return new Rect(Position, Scale); } }

        /// <summary>
        /// The drawing Z.
        /// </summary>
        public float Z
        {
            get { return Cursor.Z; }
            set { for (int i = 0; i < cursors.Count; ++i) cursors[i].Z = value; }
        }

        /// <inheritdoc />
        protected override bool VisibleHook
        {
            get { return base.VisibleHook; }
            set
            {
                base.VisibleHook = value;
                for (int i = 0; i < cursors.Count; ++i) cursors[i].Visible = value;
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
                skinGroups.WaitMouse.RemoveImage(cursors[(int)MouseMode.Wait]);
                skinGroups.NormalMouse.RemoveImage(cursors[(int)MouseMode.Normal]);
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            Position = Engine.GetService<GuiSystem>().AppMousePosition;
        }

        /// <inheritdoc />
        protected override IList<QuickSprite> CollectQuickSpritesHook(IList<QuickSprite> result)
        {
            base.CollectQuickSpritesHook(result);
            if (Visible) Cursor.CollectQuickSprites(result);
            return result;
        }

        private Image Cursor { get { return cursors[(int)Mode]; } }

        private readonly IList<Image> cursors = new List<Image>();
        private MouseMode _mode;
        private Vector2 _position;
    }
}

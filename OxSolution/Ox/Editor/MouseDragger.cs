using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Utility;
using Ox.Gui.Event;

namespace Ox.Editor
{
    public abstract class MouseDragger<T> : UpdateableSubcomponent<T> where T : EditorController
    {
        public MouseDragger(OxEngine engine, T component, double beginDragDelay) : base(engine, component)
        {
            this.beginDragDelay = (double)MathHelper.Max(0, (float)beginDragDelay); // VALIDATION
        }

        public void HandleButton(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickDown)
            {
                modifier = Engine.KeyboardState.GetModifier();
                ButtonDown = true;
                PrepareDrag(mousePosition);
            }
            else if (inputType == InputType.Down)
            {
                if (ReadyToDrag)
                {
                    if (!dragging)
                    {
                        dragging = true;
                        BeginDrag(mousePosition);
                    }

                    if (modifierChanged)
                    {
                        modifierChanged = false;
                        EndDrag(mousePosition);
                        PrepareDrag(mousePosition);
                        BeginDrag(mousePosition);
                    }

                    UpdateDrag(mousePosition);
                }
                else if (modifierChanged)
                {
                    modifierChanged = false;
                    PrepareDrag(mousePosition);
                }
            }
            else if (inputType == InputType.ClickUp)
            {
                if (ReadyToDrag && dragging)
                {
                    dragging = false;
                    EndDrag(mousePosition);
                }

                ButtonDown = false;
            }
        }

        protected KeyboardModifier Modifier { get { return modifier; } }

        protected bool ButtonDown
        {
            get { return _buttonDown; }
            private set
            {
                _buttonDown = value;
                buttonDownTime = 0;
            }
        }

        protected bool ReadyToDrag
        {
            get { return ButtonDown && buttonDownTime >= beginDragDelay; }
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            buttonDownTime += gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardModifier modifier = Engine.KeyboardState.GetModifier();
            if (this.modifier == modifier) return;
            this.modifier = modifier;
            modifierChanged = true;
        }

        protected abstract void PrepareDragHook(Vector2 mousePosition);
        protected abstract void BeginDragHook(Vector2 mousePosition);
        protected abstract void UpdateDragHook(Vector2 mousePosition);
        protected abstract void EndDragHook(Vector2 mousePosition);

        private void PrepareDrag(Vector2 mousePosition)
        {
            PrepareDragHook(mousePosition);
        }

        private void BeginDrag(Vector2 mousePosition)
        {
            BeginDragHook(mousePosition);
        }

        private void UpdateDrag(Vector2 mousePosition)
        {
            UpdateDragHook(mousePosition);
        }

        private void EndDrag(Vector2 mousePosition)
        {
            UpdateDragHook(mousePosition); // update one time before ending the drag operation!
            EndDragHook(mousePosition);
        }

        private readonly double beginDragDelay;
        private KeyboardModifier modifier;
        private double buttonDownTime;
        private bool modifierChanged;
        private bool dragging;
        private bool _buttonDown;
    }
}

using System;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical check box for a user interface.
    /// </summary>
    public class CheckBox : GuiComponent<CheckBoxView>
    {
        /// <summary>
        /// Create a CheckBoxView.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public CheckBox(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            View.Checked = Checked;
            Scale = defaultScale;
            GuiAction += this_GuiAction;
            MouseButtonAction += this_MouseButtonAction;
            AbstractAction += this_AbstractAction;
        }
        
        /// <summary>
        /// Is there a check in the check box?
        /// </summary>
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value) return;
                View.Checked = _checked = value;
                if (CheckedChanged != null) CheckedChanged(this);
            }
        }
        
        /// <summary>
        /// Raised when the check box is checked or unchecked.
        /// </summary>
        public event Action<CheckBox> CheckedChanged;

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new CheckBoxToken();
        }

        /// <inheritdoc />
        protected override bool SinkAbstractEventHook(InputType inputType, AbstractEventType eventType)
        {
            return eventType == AbstractEventType.Affirm;
        }

        private void this_GuiAction(BaseGuiComponent sender, GuiEventType type)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (type == GuiEventType.Defocused) HandleDefocused();
        }

        private void this_MouseButtonAction(BaseGuiComponent sender, InputType type, MouseButton button, Vector2 mousePosition)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (button == MouseButton.Left) HandleLeftMouse(type);
        }

        private void this_AbstractAction(BaseGuiComponent sender, InputType inputType, AbstractEventType eventType)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (eventType == AbstractEventType.Affirm) HandleAffirmButton(inputType);
        }

        private void HandleDefocused()
        {
            pressed = false;
        }

        private void HandleLeftMouse(InputType type)
        {
            if (type == InputType.ClickDown)
            {
                RaiseAbstractEvent(InputType.ClickDown, AbstractEventType.Affirm);
            }
            else if (type == InputType.ClickUp)
            {
                if (Picked && pressed)
                {
                    RaiseAbstractEvent(InputType.ClickUp, AbstractEventType.Affirm);
                }
                else
                {
                    pressed = false;
                }
            }
        }

        private void HandleAffirmButton(InputType inputType)
        {
            if (inputType == InputType.ClickDown)
            {
                pressed = true;
            }
            else if (inputType == InputType.ClickUp)
            {
                if (pressed)
                {
                    Checked = !Checked;
                    pressed = false;
                }
            }
        }

        private static readonly Vector2 defaultScale = new Vector2(32);

        private bool pressed;
        private bool _checked;
    }
}

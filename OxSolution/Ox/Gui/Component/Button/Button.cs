using System;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical button for a user interface.
    /// </summary>
    public class Button : GuiComponent<ButtonView>
    {
        /// <summary>
        /// Create a Button.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public Button(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            View.Pressed = Pressed;
            View.Text = Text;
            GuiAction += this_GuiAction;
            MouseButtonAction += this_MouseButtonAction;
            AbstractAction += this_AbstractAction;
        }

        /// <summary>
        /// The text string on the button.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                View.Text = _text = value;
            }
        }

        /// <summary>
        /// Is the button pressed?
        /// </summary>
        public bool Pressed
        {
            get { return _pressed; }
            private set { View.Pressed = _pressed = value; }
        }

        /// <summary>
        /// Raise when the button has been clicked.
        /// </summary>
        public event Action<Button> Clicked;

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new ButtonToken();
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
            Pressed = false;
        }

        private void HandleLeftMouse(InputType type)
        {
            if (type == InputType.ClickDown)
            {
                RaiseAbstractEvent(InputType.ClickDown, AbstractEventType.Affirm);
            }
            else if (type == InputType.ClickUp)
            {
                if (Picked && Pressed)
                {
                    RaiseAbstractEvent(InputType.ClickUp, AbstractEventType.Affirm);
                }
                else
                {
                    Pressed = false;
                }
            }
        }

        private void HandleAffirmButton(InputType inputType)
        {
            if (inputType == InputType.ClickDown)
            {
                Pressed = true;
            }
            else if (inputType == InputType.ClickUp)
            {
                if (Pressed)
                {
                    if (Clicked != null) Clicked(this);
                    Pressed = false;
                }
            }
        }

        private string _text = string.Empty;
        private bool _pressed;
    }
}

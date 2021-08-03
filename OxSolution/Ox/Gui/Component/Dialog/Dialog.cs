using System;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical dialog window for a user interface.
    /// </summary>
    public class Dialog : GuiComponent<DialogView>
    {
        /// <summary>
        /// Create a Dialog.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public Dialog(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            GuiAction += this_GuiAction;

            View.Text = Text;

            AddGarbage(closeButton = new Button(engine, domainName, false));
            closeButton.Position = closeButtonPosition;
            closeButton.Scale = closeButtonScale;
            closeButton.SetTrait("Style", "Close");
            closeButton.Clicked += closeButton_Clicked;
            AddChild(closeButton);

            FocusableByMouseInput = false;
            FocusableByOtherInput = false;
        }

        /// <summary>
        /// The header text of the dialog.
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
        /// Is the close button active?
        /// </summary>
        public bool CloseButtonActive
        {
            get { return closeButton.Active; }
            set { closeButton.Active = value; }
        }

        /// <summary>
        /// Raised when the close button on the dialog is clicked.
        /// </summary>
        public event Action<Dialog> CloseClicked;

        /// <summary>
        /// Close the dialog.
        /// </summary>
        public void Close()
        {
            closeButton.RaiseAbstractEvent(InputType.ClickDown, AbstractEventType.Affirm);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) GuiAction -= this_GuiAction;
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new DialogToken();
        }

        /// <inheritdoc />
        protected override bool SinkDirectionEventHook(InputType inputType, Direction2D direction)
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool SinkAbstractEventHook(InputType inputType, AbstractEventType eventType)
        {
            return true;
        }

        private void this_GuiAction(BaseGuiComponent sender, GuiEventType e)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (e == GuiEventType.ColorChanged) closeButton.Color = Color;
        }

        private void closeButton_Clicked(Button sender)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (CloseClicked != null) CloseClicked(this);
        }

        private static readonly Vector2 closeButtonPosition = new Vector2(8);
        private static readonly Vector2 closeButtonScale = new Vector2(32);

        private readonly Button closeButton;
        private string _text = string.Empty;
    }
}

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// Represent the type of a message box.
    /// </summary>
    public enum MessageBoxType
    {
        OkOnly = 0,
        ConfirmCancel,
        YesNo
    }

    /// <summary>
    /// Represents the result of interaction with a message box.
    /// </summary>
    public enum MessageBoxAnswer
    {
        Negative = 0,
        Positive
    }

    /// <summary>
    /// Raised when the message box has obtained its result.
    /// </summary>
    public delegate void MessageBoxAnswered(MessageBox sender, MessageBoxAnswer answer);

    /// <summary>
    /// A graphical message box.
    /// </summary>
    public class MessageBox : Dialog
    {
        public MessageBox(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            GuiAction += this_GuiAction;
            CloseClicked += this_CloseClicked;

            AddGarbage(message = new Label(engine, domainName, false));
            message.Position = Vector2.Zero;
            message.Scale = scale;
            message.SetTrait("TextJustification", Justification2D.Center);
            AddChild(message);

            AddGarbage(button1 = new Button(engine, domainName, false));
            button1.Scale = buttonScale;
            button1.Clicked += button1_Clicked;
            AddChild(button1);

            AddGarbage(button2 = new Button(engine, domainName, false));
            button2.Scale = buttonScale;
            button2.Clicked += button2_Clicked;
            AddChild(button2);

            Scale = scale;
            Modal = true;
        }

        /// <summary>
        /// The type of the message box.
        /// </summary>
        public MessageBoxType Type
        {
            get { return _type; }
            private set
            {
                _type = value;
                UpdateType();
            }
        }

        /// <summary>
        /// Raised when the message box has obtained its result.
        /// </summary>
        public event MessageBoxAnswered Answered;

        /// <summary>
        /// Execute the message box.
        /// </summary>
        /// <param name="type">The type of message box to use.</param>
        /// <param name="headerText">The header text of the message box.</param>
        /// <param name="messageText">The message text of the message box.</param>
        public void Execute(MessageBoxType type, string headerText, string messageText)
        {
            OxHelper.ArgumentNullCheck(headerText, messageText);
            Type = type;
            Text = headerText;
            message.Text = messageText;
            button1.FocusByOtherInput();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) GuiAction -= this_GuiAction;
            base.Dispose(disposing);
        }

        private void this_GuiAction(BaseGuiComponent sender, GuiEventType type)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (type == GuiEventType.ColorChanged) CascadeColor();
        }

        private void this_CloseClicked(Dialog sender)
        {
            OxHelper.ArgumentNullCheck(sender);
            MessageBoxAnswer answer;
            switch (Type)
            {
                case MessageBoxType.ConfirmCancel: answer = MessageBoxAnswer.Negative; break;
                case MessageBoxType.OkOnly: answer = MessageBoxAnswer.Positive; break;
                case MessageBoxType.YesNo: answer = MessageBoxAnswer.Negative; break;
                default: answer = MessageBoxAnswer.Positive; break;
            }

            RaiseAnswered(answer);
        }

        private void button1_Clicked(Button sender)
        {
            OxHelper.ArgumentNullCheck(sender);
            RaiseAnswered(MessageBoxAnswer.Positive);
        }

        private void button2_Clicked(Button sender)
        {
            OxHelper.ArgumentNullCheck(sender);
            RaiseAnswered(MessageBoxAnswer.Negative);
        }

        private void RaiseAnswered(MessageBoxAnswer answer)
        {
            GuiSystem guiSystem = Engine.GetService<GuiSystem>();
            guiSystem.HideMessageBox();
            if (Answered != null) Answered(this, answer);
        }

        private void UpdateType()
        {
            switch (Type)
            {
                case MessageBoxType.OkOnly: ConfigureOkOnly(); break;
                case MessageBoxType.ConfirmCancel: ConfigureConfirmCancel(); break;
                case MessageBoxType.YesNo: ConfigureYesNo(); break;
            }
        }

        private void ConfigureOkOnly()
        {
            button1.Text = "OK";
            button1.Visible = true;
            button1.Position = centerButtonPosition;

            button2.Visible = false;
        }

        private void ConfigureConfirmCancel()
        {
            button1.Text = "Confirm";
            button1.Visible = true;
            button1.Position = leftButtonPosition;

            button2.Text = "Cancel";
            button2.Visible = true;
            button2.Position = rightButtonPosition;
        }

        private void ConfigureYesNo()
        {
            button1.Text = "Yes";
            button1.Visible = true;
            button1.Position = leftButtonPosition;

            button2.Text = "No";
            button2.Visible = true;
            button2.Position = rightButtonPosition;
        }

        private void CascadeColor()
        {
            IList<BaseGuiComponent> children = CollectChildren(CollectionAlgorithm.Descending, new List<BaseGuiComponent>()); // MEMORYCHURN
            for (int i = 0; i < children.Count; ++i) children[i].Color = Color;
        }

        private readonly Vector2 centerButtonPosition = new Vector2(288, 276);
        private readonly Vector2 leftButtonPosition = new Vector2(186, 279);
        private readonly Vector2 rightButtonPosition = new Vector2(390, 276);
        private readonly Vector2 buttonScale = new Vector2(192, 48);
        private readonly Vector2 scale = new Vector2(768, 336);
        private readonly Button button1;
        private readonly Button button2;
        private readonly Label message;
        private MessageBoxType _type;
    }
}

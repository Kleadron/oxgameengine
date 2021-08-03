using System;
using Microsoft.Xna.Framework.Input;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Engine.ServicesNamespace;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical text-entry box for a user interface.
    /// </summary>
    public class TextBox : GuiComponent<TextBoxView>
    {
        /// <summary>
        /// Create a TextBox.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public TextBox(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            View.CurrentCharacter = CurrentCharacter;
            View.CursorVisible = CursorVisible;
            View.Text = Text;
            GuiAction += this_GuiAction;
            AbstractAction += this_AbstractAction;
            DirectionAction += this_DirectionAction;
            KeyAction += this_KeyAction;
        }

        /// <summary>
        /// The current text string in the text box.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                value = value.Substring(0, Math.Min(value.Length, CharacterLimit)); // VALIDATION: keep text within character limit
                CurrentCharacter = Math.Min(value.Length, CurrentCharacter); // VALIDATION: keep CurrentCharacter valid.
                if (_text.Equals(value)) return;
                View.Text = _text = value;
                if (TextChanged != null) TextChanged(this);
            }
        }

        /// <summary>
        /// The visibility of the text-entry cursor.
        /// </summary>
        public bool CursorVisible
        {
            get { return _cursorVisible; }
            set
            {
                if (_cursorVisible == value) return;
                View.CursorVisible = _cursorVisible = value;
                if (CursorVisibleChanged != null) CursorVisibleChanged(this);
            }
        }

        /// <summary>
        /// The current point of text-entry.
        /// </summary>
        public int CurrentCharacter
        {
            get { return _currentCharacter; }
            set
            {
                // VALIDATION: if out of range, "wrap" the value around to opposite side of the text box
                if (value > Text.Length) value = 0;
                else if (value < 0) value = Text.Length;
                if (_currentCharacter == value) return;
                View.CurrentCharacter = _currentCharacter = value;
                if (CurrentCharacterChanged != null) CurrentCharacterChanged(this);
            }
        }

        /// <summary>
        /// The maximum number of characters that can be entered in the text box.
        /// </summary>
        public int CharacterLimit
        {
            get { return _characterLimit; }
            set
            {
                value = Math.Max(0, value); // VALIDATION
                if (_characterLimit == value) return;
                _text = _text.Substring(0, Math.Min(_text.Length, value));
                _characterLimit = value;
                if (TextChanged != null) TextChanged(this);
                if (CharacterLimitChanged != null) CharacterLimitChanged(this);
            }
        }

        /// <summary>
        /// Raised when the text string is changed.
        /// </summary>
        public event Action<TextBox> TextChanged;

        /// <summary>
        /// Raised when the point of text-entry is changed.
        /// </summary>
        public event Action<TextBox> CurrentCharacterChanged;

        /// <summary>
        /// Raised when the cursor's visibility is changed.
        /// </summary>
        public event Action<TextBox> CursorVisibleChanged;

        /// <summary>
        /// Raised when the character limit is changed.
        /// </summary>
        public event Action<TextBox> CharacterLimitChanged;

        /// <summary>
        /// Enter text using a keyboard key.
        /// </summary>
        public void TextEntry(Keys key)
        {
            switch (key)
            {
                case Keys.Home: Home(); break;
                case Keys.End: End(); break;
                case Keys.Back: Backspace(); break;
                case Keys.Delete: Delete(); break;
                default:
                    {
                        KeyboardContext keyboardContext = Engine.GetService<KeyboardContext>();
                        char character;
                        if (keyboardContext.KeyToChar(key, out character)) TextEntry(character);
                        break;
                    }
            }
        }

        /// <summary>
        /// Enter text using a character.
        /// </summary>
        public void TextEntry(char character)
        {
            if (Text.Length >= CharacterLimit) return;
            Text = Text.Insert(CurrentCharacter, character.ToString());
            CurrentCharacter++;
        }

        /// <summary>
        /// Position the text-entry cursor at the beginning of the text string.
        /// </summary>
        public void Home()
        {
            CurrentCharacter = 0;
        }

        /// <summary>
        /// Position the text-entry cursor at the end of the text string.
        /// </summary>
        public void End()
        {
            CurrentCharacter = Text.Length;
        }

        /// <summary>
        /// Delete a character at the current point of text-entry.
        /// </summary>
        public void Delete()
        {
            if (CurrentCharacter >= Text.Length) return;
            Text = Text.Remove(CurrentCharacter, 1);
        }

        /// <summary>
        /// Delete a character before the current point of text-entry.
        /// </summary>
        public void Backspace()
        {
            if (CurrentCharacter <= 0) return;
            CurrentCharacter--;
            Text = Text.Remove(CurrentCharacter, 1);
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new TextBoxToken();
        }

        /// <inheritdoc />
        protected override bool SinkDirectionEventHook(InputType inputType, Direction2D direction)
        {
            return direction == Direction2D.Left || direction == Direction2D.Right;
        }

        /// <inheritdoc />
        protected override bool SinkAbstractEventHook(InputType inputType, AbstractEventType eventType)
        {
            return eventType == AbstractEventType.Affirm;
        }

        private void this_GuiAction(BaseGuiComponent sender, GuiEventType type)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (type == GuiEventType.FocusedByMouseInput ||
                type == GuiEventType.FocusedByOtherInput ||
                type == GuiEventType.FocusedByNonInput)
                CursorVisible = true;
            else if (type == GuiEventType.Defocused) CursorVisible = false;
        }

        private void this_AbstractAction(BaseGuiComponent sender, InputType inputType, AbstractEventType eventType)
        {
            OxHelper.ArgumentNullCheck(sender);
            GuiSystem guiSystem = Engine.GetService<GuiSystem>();
            if (inputType == InputType.ClickDown) guiSystem.ShowVirtualKeyboard(this);
        }

        private void this_DirectionAction(BaseGuiComponent sender, InputType type, Direction2D direction)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (type == InputType.ClickDown || type == InputType.Repeat)
            {
                if (direction == Direction2D.Left) --CurrentCharacter;
                else if (direction == Direction2D.Right) ++CurrentCharacter;
            }
        }

        private void this_KeyAction(BaseGuiComponent sender, InputType type, Keys key)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (type == InputType.Repeat || type == InputType.ClickDown) TextEntry(key);
        }

        private string _text = string.Empty;
        private bool _cursorVisible;
        private int _currentCharacter;
        private int _characterLimit = 32;
    }
}

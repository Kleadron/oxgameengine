using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical view for a text box.
    /// </summary>
    public class TextBoxView : GuiView
    {
        /// <summary>
        /// Create a TextBoxView.
        /// </summary>
        public TextBoxView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();

            skinGroups.LitSurface.AddImage(background);
            RegisterElement(background);

            skinGroups.NormalTextCursor.AddImage(cursor);
            RegisterElement(cursor);

            skinGroups.TextBorder.AddBorder(border);
            RegisterElement(border);

            text = skinGroups.NormalText.Font.CreateText();
            text.Color = Color.Black;
            skinGroups.NormalText.AddText(text);
            RegisterElement(text);
        }

        /// <summary>
        /// The text string in the text box.
        /// </summary>
        public string Text
        {
            get { return _textString; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                _textString = value;
                UpdateText();
            }
        }

        /// <summary>
        /// The visibility of the text box's cursor.
        /// </summary>
        public bool CursorVisible
        {
            get { return _cursorVisible; }
            set
            {
                _cursorVisible = value;
                UpdateCursorVisible();
            }
        }

        /// <summary>
        /// The current character on before which the cursor is positioned.
        /// </summary>
        public int CurrentCharacter
        {
            get { return _currentCharacter; }
            set
            {
                _currentCharacter = value;
                UpdateCursorTransform();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
                skinGroups.NormalText.RemoveText(text);
                skinGroups.TextBorder.RemoveBorder(border);
                skinGroups.NormalTextCursor.RemoveImage(background);
                skinGroups.LitSurface.RemoveImage(background);
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void TraitSetHook(string name)
        {
            base.TraitSetHook(name);
            switch(name)
            {
                case "TextScale": UpdateTextTransform(); break;
                case "TextInset": text.Inset = Parent.GetTrait<float>("TextInset"); break;
                case "TextColor": TextColor = Parent.GetTrait<Color>("TextColor"); break;
                case "TextInactiveColor": TextInactiveColor = Parent.GetTrait<Color>("TextInactiveColor"); break;
            }
        }

        /// <inheritdoc />
        protected override void UpdateVisibleHook()
        {
            base.UpdateVisibleHook();
            UpdateCursorVisible();
        }

        /// <inheritdoc />
        protected override void UpdateFocusedHook()
        {
            base.UpdateFocusedHook();
            UpdateCursorVisible();
        }

        /// <inheritdoc />
        protected override void UpdateActiveHook()
        {
            base.UpdateActiveHook();
            UpdateTextColor();
        }

        /// <inheritdoc />
        protected override void UpdateZHook()
        {
            base.UpdateZHook();
            cursor.Z = Z + GuiConfiguration.ZGap * 0.25f;
            text.Z = Z + GuiConfiguration.ZGap * 0.5f;
            border.Z = Z + GuiConfiguration.ZGap * 0.75f;
        }

        /// <inheritdoc />
        protected override void UpdatePositionHook()
        {
            base.UpdatePositionHook();
            UpdateTextTransform();
            UpdateCursorTransform();
        }

        /// <inheritdoc />
        protected override void UpdateScaleHook()
        {
            base.UpdateScaleHook();
            UpdateTextTransform();
            UpdateCursorTransform();
        }

        private Color TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                UpdateTextColor();
            }
        }

        private Color TextInactiveColor
        {
            get { return _textInactiveColor; }
            set
            {
                _textInactiveColor = value;
                UpdateTextColor();
            }
        }

        private void UpdateText()
        {
            text.Text = Text;
            UpdateTextTransform();
            UpdateCursorTransform();
        }

        private void UpdateTextTransform()
        {
            // MUST set scale first since the code after depends on it
            text.Scale = Parent.TryGetTrait<Vector2>("TextScale", new Vector2(16, 32));
            Vector2 textOffset = new Vector2(text.Scale.X, (Scale.Y - text.Scale.Y) * 0.5f);
            text.Position = Position + textOffset;
        }

        private void UpdateTextColor()
        {
            text.Color = Active ? TextColor : TextInactiveColor;
        }

        /// <summary>
        /// Updates the cursor's position.
        /// Note that this method depends on text's position being up to date.
        /// </summary>
        private void UpdateCursorTransform()
        {
            // MUST set scale first since the code after depends on it
            cursor.Scale = text.Scale;
            // position the cursor at the proper offset from the text's position
            cursor.Position = text.Position + text.GetGlyphPosition((int)CurrentCharacter);
            // center the cursor horizontally
            cursor.Position -= new Vector2(cursor.Scale.X * 0.5f, 0);
        }

        private void UpdateCursorVisible()
        {
            cursor.Visible = Visible && CursorVisible;
        }

        private readonly Image background = new Image();
        private readonly Image cursor = new Image();
        private readonly Border border = new Border();
        private readonly IText text;
        private Color _textColor = Color.Black;
        private Color _textInactiveColor = Color.Gray;
        private string _textString = string.Empty;
        private bool _cursorVisible;
        private int _currentCharacter;
    }
}

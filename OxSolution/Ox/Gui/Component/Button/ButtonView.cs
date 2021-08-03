using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical view for a button.
    /// </summary>
    public class ButtonView : GuiView
    {
        /// <summary>
        /// Create a ButtonView.
        /// </summary>
        public ButtonView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();            

            DecorateImages();
            RegisterElement(pressedImage);
            RegisterElement(releasedImage);

            text = skinGroups.NormalText.Font.CreateText();
            text.Color = Color.Black;
            skinGroups.NormalText.AddText(text);
            RegisterElement(text);

            skinGroups.SunkenBorder.AddBorder(pressedBorder);
            RegisterElement(pressedBorder);

            skinGroups.SunkenFocusedBorder.AddBorder(pressedFocusedBorder);
            RegisterElement(pressedFocusedBorder);

            skinGroups.RaisedBorder.AddBorder(releasedBorder);
            RegisterElement(releasedBorder);

            skinGroups.RaisedFocusedBorder.AddBorder(releasedFocusedBorder);
            RegisterElement(releasedFocusedBorder);
        }

        /// <summary>
        /// The text string on the button.
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
        /// The pressed state of the button.
        /// </summary>
        public bool Pressed
        {
            get { return _pressed; }
            set
            {
                _pressed = value;
                UpdateVisible();
                UpdateTextTransform();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();                
                skinGroups.RaisedFocusedBorder.RemoveBorder(releasedFocusedBorder);
                skinGroups.RaisedBorder.RemoveBorder(releasedBorder);
                skinGroups.SunkenFocusedBorder.RemoveBorder(pressedFocusedBorder);
                skinGroups.SunkenBorder.RemoveBorder(pressedBorder);
                skinGroups.NormalText.RemoveText(text);
                UndecorateImages();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void TraitSetHook(string name)
        {
            base.TraitSetHook(name);
            switch (name)
            {
                case "Style": Style = Parent.GetTrait<string>("Style"); break;
                case "TextJustification": TextJustification = Parent.GetTrait<Justification2D>("TextJustification"); break;
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
            UpdateVisible();
        }

        /// <inheritdoc />
        protected override void UpdateFocusedHook()
        {
            base.UpdateFocusedHook();
            UpdateVisible();
        }

        /// <inheritdoc />
        protected override void UpdatePositionHook()
        {
            base.UpdatePositionHook();
            UpdateTextTransform();
        }

        /// <inheritdoc />
        protected override void UpdateScaleHook()
        {
            base.UpdateScaleHook();
            UpdateTextTransform();
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

            releasedBorder.Z =
                releasedFocusedBorder.Z =
                pressedBorder.Z =
                pressedFocusedBorder.Z =
                Z + GuiConfiguration.ZGap * 0.25f;

            text.Z = Z + GuiConfiguration.ZGap * 0.5f;
        }
        
        private Justification2D TextJustification
        {
            get { return _textJustification; }
            set
            {
                _textJustification = value;
                UpdateTextTransform();
            }
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
        
        private string Style
        {
            get { return _style; }
            set
            {
                _style = value;
                UndecorateImages();
                DecorateImages();
            }
        }

        private void DecorateImages()
        {
            GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
            if (Style == "Close")
            {
                skinGroups.RaisedX.AddImage(releasedImage);
                skinGroups.SunkenX.AddImage(pressedImage);
            }
            else
            {
                skinGroups.RaisedSurface.AddImage(releasedImage);
                skinGroups.SunkenSurface.AddImage(pressedImage);
            }
        }

        private void UndecorateImages()
        {
            GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
            if (Style == "Close")
            {
                skinGroups.RaisedX.RemoveImage(releasedImage);
                skinGroups.SunkenX.RemoveImage(pressedImage);
            }
            else
            {
                skinGroups.RaisedSurface.RemoveImage(releasedImage);
                skinGroups.SunkenSurface.RemoveImage(pressedImage);
            }
        }

        private void UpdateVisible()
        {
            if (!Visible) return;

            releasedBorder.Visible = !Pressed && !Focused;
            releasedFocusedBorder.Visible = !Pressed && Focused;
            releasedImage.Visible = !Pressed;

            pressedBorder.Visible = Pressed && !Focused;
            pressedFocusedBorder.Visible = Pressed && Focused;
            pressedImage.Visible = Pressed;
        }

        private void UpdateText()
        {
            text.Text = Text;
            UpdateTextTransform();
        }

        private void UpdateTextColor()
        {
            text.Color = Active ? TextColor : TextInactiveColor;
        }

        /// <summary>
        /// "Sink" the text slightly when the button is pressed.
        /// </summary>
        private void UpdateTextTransform()
        {
            // MUST set scale first since code after depends on it.
            text.Scale = Parent.TryGetTrait<Vector2>("TextScale", new Vector2(16, 32));
            Rect justifiedRect = new Rect(Position, Scale).Justify(TextJustification, text.Rect);
            text.Position = new Vector2(justifiedRect.X, justifiedRect.Y);
            if (Pressed) text.Position += Vector2.One;
        }

        private readonly Image pressedImage = new Image();
        private readonly Border pressedBorder = new Border();
        private readonly Border pressedFocusedBorder = new Border();
        private readonly Image releasedImage = new Image();
        private readonly Border releasedBorder = new Border();
        private readonly Border releasedFocusedBorder = new Border();
        private readonly IText text;
        private Justification2D _textJustification = Justification2D.Center;
        private Color _textColor = Color.Black;
        private Color _textInactiveColor = Color.Gray;
        private string _style;
        private string _textString = string.Empty;
        private bool _pressed;
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical view for a label.
    /// </summary>
    public class LabelView : GuiView
    {
        /// <summary>
        /// Create a LabelView.
        /// </summary>
        public LabelView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();
            text = skinGroups.NormalText.Font.CreateText();
            text.Color = Color.Black;
            skinGroups.NormalText.AddText(text);
            RegisterElement(text);
        }

        /// <summary>
        /// The text string on the label.
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

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
                skinGroups.NormalText.RemoveText(text);
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void TraitSetHook(string name)
        {
            base.TraitSetHook(name);
            switch (name)
            {
                case "TextJustification": TextJustification = Parent.GetTrait<Justification2D>("TextJustification"); break;
                case "TextScale": UpdateTextTransform(); break;
                case "TextInset": text.Inset = Parent.GetTrait<float>("TextInset"); break;
                case "TextColor": text.Color = Parent.GetTrait<Color>("TextColor"); break;
            }
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

        private Justification2D TextJustification
        {
            get { return _textJustification; }
            set
            {
                _textJustification = value;
                UpdateTextTransform();
            }
        }

        private void UpdateText()
        {
            text.Text = Text;
            UpdateTextTransform();
        }

        private void UpdateTextTransform()
        {
            // MUST set scale first since code after depends on it.
            text.Scale = Parent.TryGetTrait<Vector2>("TextScale", new Vector2(16, 32));
            Rect justifiedRect = new Rect(Position, Scale).Justify(TextJustification, text.Rect);
            text.Position = new Vector2(justifiedRect.X, justifiedRect.Y);
        }

        private readonly IText text;
        private Justification2D _textJustification = Justification2D.Left;
        private string _textString = string.Empty;
    }
}

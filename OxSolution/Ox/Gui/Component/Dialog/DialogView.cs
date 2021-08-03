using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical view for a dialog window.
    /// </summary>
    public class DialogView : GuiView
    {
        /// <summary>
        /// Create DialogView.
        /// </summary>
        public DialogView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();

            skinGroups.NormalSurface.AddImage(background);
            RegisterElement(background);

            skinGroups.NormalBorder.AddBorder(border);
            RegisterElement(border);

            text = skinGroups.NormalText.Font.CreateText();
            text.Color = Color.Black;
            engine.GetService<GuiSkinGroups>().NormalText.AddText(text);
            RegisterElement(text);
        }

        /// <summary>
        /// The header text.
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
                skinGroups.NormalBorder.RemoveBorder(border);
                skinGroups.NormalSurface.RemoveImage(background);
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void TraitSetHook(string name)
        {
            base.TraitSetHook(name);
            switch (name)
            {
                case "TextScale": UpdateTextTransform(); break;
                case "TextInset": text.Inset = Parent.GetTrait<float>("TextInset"); break;
            }
        }

        /// <inheritdoc />
        protected override void UpdateZHook()
        {
            base.UpdateZHook();
            border.Z = Z + GuiConfiguration.ZGap * 0.25f;
            text.Z = Z + GuiConfiguration.ZGap * 0.5f;
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

        private void UpdateText()
        {
            text.Text = Text;
            UpdateTextTransform();
        }

        private void UpdateTextTransform()
        {
            text.Position = Position + textPositionOffset;
            text.Scale = Parent.TryGetTrait<Vector2>("TextScale", new Vector2(16, 32));
        }

        private static readonly Vector2 textPositionOffset = new Vector2(48, 8);

        private readonly Image background = new Image();
        private readonly Border border = new Border();
        private readonly IText text;
        private string _textString = string.Empty;
    }
}

using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical view for a radio button.
    /// </summary>
    public class RadioButtonView : GuiView
    {
        /// <summary>
        /// Create a RadioButtonView.
        /// </summary>
        public RadioButtonView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();

            skinGroups.RadioOn.AddImage(checkedImage);
            RegisterElement(checkedImage);

            skinGroups.RadioOnFocused.AddImage(checkedFocusedImage);
            RegisterElement(checkedFocusedImage);

            skinGroups.RadioOff.AddImage(uncheckedImage);
            RegisterElement(uncheckedImage);

            skinGroups.RadioOffFocused.AddImage(uncheckedFocusedImage);
            RegisterElement(uncheckedFocusedImage);
        }

        /// <summary>
        /// The checked state of the radio button.
        /// </summary>
        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                UpdateVisible();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
                skinGroups.RadioOffFocused.RemoveImage(uncheckedFocusedImage);
                skinGroups.RadioOff.RemoveImage(uncheckedImage);
                skinGroups.RadioOnFocused.RemoveImage(checkedFocusedImage);
                skinGroups.RadioOn.RemoveImage(checkedImage);
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void UpdateFocusedHook()
        {
            base.UpdateFocusedHook();
            UpdateVisible();
        }

        /// <inheritdoc />
        protected override void UpdateVisibleHook()
        {
            base.UpdateVisibleHook();
            UpdateVisible();
        }

        /// <inheritdoc />
        protected override void UpdateActiveHook()
        {
            base.UpdateActiveHook();
            checkedImage.Color =
                checkedFocusedImage.Color =
                uncheckedImage.Color =
                uncheckedFocusedImage.Color =
                Active ? Color.White : Color.Gray;
        }

        private void UpdateVisible()
        {
            if (!Visible) return;
            checkedImage.Visible = Checked && !Focused;
            checkedFocusedImage.Visible = Checked && Focused;
            uncheckedImage.Visible = !Checked && !Focused;
            uncheckedFocusedImage.Visible = !Checked && Focused;
        }

        private readonly Image checkedImage = new Image();
        private readonly Image checkedFocusedImage = new Image();
        private readonly Image uncheckedImage = new Image();
        private readonly Image uncheckedFocusedImage = new Image();
        private bool _checked;
    }
}

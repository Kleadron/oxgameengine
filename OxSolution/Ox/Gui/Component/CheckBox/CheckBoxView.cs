using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical view for a check box.
    /// </summary>
    public class CheckBoxView : GuiView
    {
        /// <summary>
        /// Create a CheckBoxView.
        /// </summary>
        public CheckBoxView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();

            skinGroups.Checked.AddImage(checkedImage);
            RegisterElement(checkedImage);

            skinGroups.CheckedFocused.AddImage(checkedFocusedImage);
            RegisterElement(checkedFocusedImage);

            skinGroups.Unchecked.AddImage(uncheckedImage);
            RegisterElement(uncheckedImage);

            skinGroups.UncheckedFocused.AddImage(uncheckedFocusedImage);
            RegisterElement(uncheckedFocusedImage);
        }

        /// <summary>
        /// The checked state of the check box.
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
                skinGroups.UncheckedFocused.RemoveImage(uncheckedFocusedImage);
                skinGroups.Unchecked.RemoveImage(uncheckedImage);
                skinGroups.CheckedFocused.RemoveImage(checkedFocusedImage);
                skinGroups.Checked.RemoveImage(checkedImage);
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

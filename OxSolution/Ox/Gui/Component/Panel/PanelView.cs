using Ox.Engine;
using Ox.Gui.ViewElement;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A plain looking view for panels and other non-descript gui components.
    /// </summary>
    public class PanelView : GuiView
    {
        /// <summary>
        /// Create a PanelView.
        /// </summary>
        public PanelView(OxEngine engine, BaseGuiComponent parent) : base(engine, parent)
        {
            GuiSkinGroups skinGroups = engine.GetService<GuiSkinGroups>();

            skinGroups.NormalSurface.AddImage(background);
            RegisterElement(background);

            skinGroups.NormalBorder.AddBorder(border);
            RegisterElement(border);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
                skinGroups.NormalSurface.RemoveImage(background);
                skinGroups.NormalBorder.RemoveBorder(border);
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void UpdateZHook()
        {
            base.UpdateZHook();
            border.Z = Z + GuiConfiguration.ZGap * 0.5f;
        }

        private readonly Image background = new Image();
        private readonly Border border = new Border();
    }
}

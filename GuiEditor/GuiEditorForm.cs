using System;
using System.Windows.Forms;
using Ox.Engine;
using SysDrawing = System.Drawing;

namespace GuiEditorNamespace
{
    public partial class GuiEditorForm : Form
    {
        public GuiEditorForm(OxEngine engine, GuiEditorController controller)
        {
            OxHelper.ArgumentNullCheck(engine, controller);
            InitializeComponent();
            this.engine = engine;
            this.controller = controller;
            buttonLimitFPS.Checked = engine.Game.IsFixedTimeStep;
            wrapper = new GuiEditorFormWrapper(engine, controller, this, propertyGrid,
                buttonUndo, buttonRedo, comboBoxComponentType, openFileDialog, saveFileDialog,
                richTextBoxConsole, treeView, canvas);
        }

        public PropertyGrid PropertyGrid { get { return propertyGrid; } }

        public SysDrawing.Rectangle PanelTransform
        {
            get { return new SysDrawing.Rectangle(canvas.PointToScreen(new SysDrawing.Point()), canvas.Size); }
        }

        public IntPtr PanelHandle
        {
            get { return canvas.IsHandleCreated ? canvas.Handle : IntPtr.Zero; }
        }

        private void menuItemExit_Click(object sender, EventArgs e) { wrapper.ActionExit(); }
        private void menuItemNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void menuItemSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void menuItemSaveAs_Click(object sender, EventArgs e) { wrapper.ActionSaveAs(); }
        private void menuItemOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void menuItemUndo_Click(object sender, EventArgs e) { wrapper.ActionUndo(); }
        private void menuItemRedo_Click(object sender, EventArgs e) { wrapper.ActionRedo(); }
        private void menuItemClone_Click(object sender, EventArgs e) { wrapper.ActionClone(); }
        private void menuItemDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void menuItemCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void menuItemPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void menuItemCreateComponent_Click(object sender, EventArgs e) { wrapper.ActionCreateComponent(); }
        private void menuItemToggleFreezing_Click(object sender, EventArgs e) { wrapper.ActionToggleFreezing(); }
        private void menuItemFreeze_Click(object sender, EventArgs e) { wrapper.ActionFreeze(); }
        private void menuItemRename_Click(object sender, EventArgs e) { wrapper.ActionRename(); }
        private void menuItemChangeType_Click(object sender, EventArgs e) { wrapper.ActionChangeType(); }
        private void menuItemCreateComponentInCanvas_Click(object sender, EventArgs e) { wrapper.ActionCreateComponentInCanvas(); }
        private void menuItemPasteInCanvas_Click(object sender, EventArgs e) { wrapper.ActionPasteInCanvas(); }
        private void buttonNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void buttonOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void buttonSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void buttonClone_Click(object sender, EventArgs e) { wrapper.ActionClone(); }
        private void buttonDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void buttonCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void buttonPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void buttonCreateComponent_Click(object sender, EventArgs e) { wrapper.ActionCreateComponent(); }
        private void buttonUndo_ButtonClick(object sender, EventArgs e) { wrapper.ActionUndo(); }
        private void buttonRedo_ButtonClick(object sender, EventArgs e) { wrapper.ActionRedo(); }
        private void buttonUndo_DropDownOpening(object sender, EventArgs e) { wrapper.ActionPopulateUndoDropDown(); }
        private void buttonRedo_DropDownOpening(object sender, EventArgs e) { wrapper.ActionPopulateRedoDropDown(); }
        private void buttonUndo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { wrapper.ActionUndo(new Guid(e.ClickedItem.Name)); }
        private void buttonRedo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { wrapper.ActionRedo(new Guid(e.ClickedItem.Name)); }
        private void buttonLimitFPS_Click(object sender, EventArgs e) { ActionLimitFPS(); }

        private void GuiEditorForm_Load(object sender, EventArgs e)
        {
            textBoxSnap.Text = controller.Snap.ToString();
        }

        private void GuiEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !wrapper.ActionPromptSave();
            if (!e.Cancel) wrapper.Dispose();
        }

        private void canvas_Enter(object sender, EventArgs e)
        {
            controller.Focused = true;
        }

        private void canvas_Leave(object sender, EventArgs e)
        {
            controller.Focused = false;
        }

        private void textBoxCreate_TextChanged(object sender, EventArgs e)
        {
            int snap;
            if (int.TryParse(textBoxSnap.Text, out snap)) controller.Snap = snap;
        }

        public void ActionLimitFPS()
        {
            engine.Game.IsFixedTimeStep = buttonLimitFPS.Checked;
        }

        private readonly GuiEditorFormWrapper wrapper;
        private readonly GuiEditorController controller;
        private readonly OxEngine engine;
    }
}

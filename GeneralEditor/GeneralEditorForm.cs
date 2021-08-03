using System;
using System.Windows.Forms;
using Ox.Engine;

namespace GeneralEditorNamespace
{
    public partial class GeneralEditorForm : Form
    {
        public GeneralEditorForm(OxEngine engine, GeneralEditorController controller)
        {
            OxHelper.ArgumentNullCheck(engine, controller);
            InitializeComponent();
            wrapper = new GeneralEditorFormWrapper(engine, controller, this, propertyGrid,
                buttonUndo, buttonRedo, comboBoxComponentType, openFileDialog, saveFileDialog,
                richTextBoxConsole, treeView);
        }

        public PropertyGrid PropertyGrid { get { return propertyGrid; } }

        private void menuItemExit_Click(object sender, EventArgs e) { wrapper.ActionExit(); }
        private void menuItemNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void menuItemSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void menuItemSaveAs_Click(object sender, EventArgs e) { wrapper.ActionSaveAs(); }
        private void menuItemOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void menuItemUndo_Click(object sender, EventArgs e) { wrapper.ActionUndo(); }
        private void menuItemRedo_Click(object sender, EventArgs e) { wrapper.ActionRedo(); }
        private void menuItemDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void menuItemCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void menuItemPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void menuItemClone_Click(object sender, EventArgs e) { wrapper.ActionClone(); }
        private void menuItemCreateComponent_Click(object sender, EventArgs e) { wrapper.ActionCreateComponent(); }
        private void menuItemCreateGroup_Click(object sender, EventArgs e) { wrapper.ActionCreateGroup(); }
        private void menuItemRename_Click(object sender, EventArgs e) { wrapper.ActionRename(); }
        private void menuItemChangeType_Click(object sender, EventArgs e) { wrapper.ActionChangeType(); }
        private void buttonNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void buttonOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void buttonSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void buttonCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void buttonPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void buttonClone_Click(object sender, EventArgs e) { wrapper.ActionClone(); }
        private void buttonDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void buttonCreateComponent_Click(object sender, EventArgs e) { wrapper.ActionCreateComponent(); }
        private void buttonCreateGroup_Click(object sender, EventArgs e) { wrapper.ActionCreateGroup(); }
        private void buttonUndo_ButtonClick(object sender, EventArgs e) { wrapper.ActionUndo(); }
        private void buttonRedo_ButtonClick(object sender, EventArgs e) { wrapper.ActionRedo(); }
        private void buttonUndo_DropDownOpening(object sender, EventArgs e) { wrapper.ActionPopulateUndoDropDown(); }
        private void buttonRedo_DropDownOpening(object sender, EventArgs e) { wrapper.ActionPopulateRedoDropDown(); }
        private void buttonUndo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { wrapper.ActionUndo(new Guid(e.ClickedItem.Name)); }
        private void buttonRedo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { wrapper.ActionRedo(new Guid(e.ClickedItem.Name)); }

        private void GeneralEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !wrapper.ActionPromptSave();
            if (!e.Cancel) wrapper.Dispose();
        }

        private readonly GeneralEditorFormWrapper wrapper;
    }
}

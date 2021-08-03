using System.Windows.Forms;
using Ox.Engine;

namespace Ox.Editor
{
    public abstract class GroupedEditorFormWrapper : EditorFormWrapper
    {
        public GroupedEditorFormWrapper(OxEngine engine, GroupedEditorController controller,
            Form form, PropertyGrid propertyGrid, ToolStripSplitButton undoButton,
            ToolStripSplitButton redoButton, ToolStripComboBox comboBoxComponentType,
            OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog,
            RichTextBox richTextBoxConsole, TreeView treeView)
            : base(engine, controller, form, propertyGrid, undoButton, redoButton,
            comboBoxComponentType, openFileDialog, saveFileDialog, richTextBoxConsole,
            treeView)
        {
            this.controller = controller;
        }

        public void ActionCreateGroup()
        {
            controller.CreateGroup();
        }
    
        private GroupedEditorController controller;
    }
}

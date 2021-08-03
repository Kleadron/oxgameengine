using System;
using System.Windows.Forms;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;

namespace GeneralEditorNamespace
{
    public class GeneralEditorFormWrapper : GroupedEditorFormWrapper
    {
        public GeneralEditorFormWrapper(OxEngine engine, GeneralEditorController controller,
            Form form, PropertyGrid propertyGrid, ToolStripSplitButton undoButton,
            ToolStripSplitButton redoButton, ToolStripComboBox comboBoxComponentType,
            OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog,
            RichTextBox richTextBoxConsole, TreeView treeView)
            : base(engine, controller, form, propertyGrid, undoButton, redoButton,
            comboBoxComponentType, openFileDialog, saveFileDialog, richTextBoxConsole, treeView)
        {
            treeViewWrapper = new GroupedEditorTreeViewWrapper(Engine, controller, treeView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) treeViewWrapper.Dispose();
            base.Dispose(disposing);
        }

        protected override void SaveDocumentHook() { }

        protected override void LoadDocumentHook() { }

        protected override void NewDocumentHook() { }

        protected override Type[] ComponentTokenTypesHook
        {
            get { return OxConfiguration.GeneralConstructionDictionary.GetConstructedTypes(); }
        }

        private readonly GroupedEditorTreeViewWrapper treeViewWrapper;
    }
}

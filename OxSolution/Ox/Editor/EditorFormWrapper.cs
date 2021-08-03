using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;
using SysWinForms = System.Windows.Forms;

namespace Ox.Editor
{
    public abstract class EditorFormWrapper : Disposable
    {
        public EditorFormWrapper(OxEngine engine, EditorController controller,
            Form form, PropertyGrid propertyGrid, ToolStripSplitButton undoButton,
            ToolStripSplitButton redoButton, ToolStripComboBox comboBoxComponentType,
            OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog,
            RichTextBox richTextBoxConsole, TreeView treeView)
        {
            this.engine = engine;
            this.controller = controller;
            this.form = form;
            this.propertyGrid = propertyGrid;
            this.undoButton = undoButton;
            this.redoButton = redoButton;
            this.comboBoxComponentType = comboBoxComponentType;
            this.openFileDialog = openFileDialog;
            this.saveFileDialog = saveFileDialog;
            this.richTextBoxConsole = richTextBoxConsole;
            this.treeView = treeView;

            openFileDialog.RestoreDirectory = true;
            saveFileDialog.RestoreDirectory = true;

            engine.ComponentNameConflicted += engine_ComponentNameConflicted;
            controller.Selection.SelectionChanged += selection_SelectionChanged;
            controller.Selection.SelectionPropertyChanged += selection_SelectionPropertyChanged;
            propertyGrid.Paint += new PaintEventHandler(propertyGrid_Paint);

            PopulateComponentTypes();
        }

        public void ActionExit()
        {
            form.Close();
        }

        public void ActionOpen()
        {
            treeView.BeginUpdate();
            {
                if (ActionPromptSave() &&
                    openFileDialog.ShowDialog() == DialogResult.OK)
                    Open();
            }
            treeView.EndUpdate();
        }

        public bool ActionSave()
        {
            if (saveFileDialog.FileName.IsEmpty()) saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName.IsNotEmpty())
            {
                Save();
                return true;
            }
            return false;
        }

        public void ActionSaveAs()
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK &&
                saveFileDialog.FileName.IsNotEmpty())
                Save();
        }

        public void ActionNew()
        {
            if (!ActionPromptSave()) return;
            controller.NewDocument();
            NewDocumentHook();
            openFileDialog.FileName = string.Empty;
            saveFileDialog.FileName = string.Empty;
            richTextBoxConsole.WriteLine("New... Success.");
        }

        public void ActionCreateComponent()
        {
            string componentType = OxHelper.AffixToken(comboBoxComponentType.Text);
            controller.CreateComponent(componentType);
        }

        public void ActionFreeze()
        {
            controller.Freeze();
        }

        public void ActionToggleFreezing()
        {
            controller.ToggleFreezing();
        }

        public void ActionRename()
        {
            // TODO: move a portion of this into the controller?
            if (controller.Selection.Count != 1) return;
            var item = controller.Selection.FirstOrNull as ItemToken;
            if (item == null) return;
            var itemNameForm = new ItemNameForm(item);
            itemNameForm.ShowDialog(form);
            // winforms automatically focuses the tree view after the item name dialog is
            // closed that in turn causes the first item in the tree view to be selected (I
            // think). To remedy this, this line re-selects the previously selected item.
            controller.Selection.Set(item);
        }

        public void ActionChangeType()
        {
            treeView.BeginUpdate();
            {
                var itemTypeForm = new ItemTypeForm(ComponentTokenTypesHook);
                itemTypeForm.ShowDialog(form);
                if (itemTypeForm.NewType.IsNotEmpty())
                {
                    // TODO: move a portion of this into the controller?
                    controller.OperationRecorder.PushGroup();
                    {
                        var components = controller.Selection.OfType<ComponentToken>();
                        var componentsCopy = components.ToArray(); // avoid iterator invalidation
                        componentsCopy.ForEach(x => controller.ChangeType(x, itemTypeForm.NewType));
                    }
                    controller.OperationRecorder.PopGroup();
                }
            }
            treeView.EndUpdate();
        }

        public void ActionCopy()
        {
            controller.Copy();
        }

        public void ActionPaste()
        {
            treeView.BeginUpdate();
            {
                controller.Paste();
            }
            treeView.EndUpdate();
        }

        public void ActionSelectAll()
        {
            controller.SelectAll();
        }

        public void ActionSelectSiblings()
        {
            controller.SelectSiblings();
        }

        public void ActionSelectFamily()
        {
            controller.SelectFamily();
        }

        public void ActionSelectFamilyAcross()
        {
            controller.SelectFamilyAcross();
        }

        public void ActionSelectSameType()
        {
            controller.SelectSameType();
        }

        public void ActionSelectSameScript()
        {
            controller.SelectSameScript();
        }

        public void ActionClone()
        {
            controller.Clone();
        }

        public void ActionDelete()
        {
            treeView.BeginUpdate();
            {
                if (controller.Delete() == DeleteResult.FailHasChildren) MessageBox.Show(
                    "Cannot delete an item that has children.", form.Text, MessageBoxButtons.OK);
            }
            treeView.EndUpdate();
        }

        public bool ActionPromptSave()
        {
            DialogResult answer = MessageBox.Show("Save the current document?", form.Text, MessageBoxButtons.YesNoCancel);
            if (answer == DialogResult.Yes) return ActionSave();
            if (answer == DialogResult.No) return true;
            return false;
        }

        public void ActionUndo()
        {
            treeView.BeginUpdate();
            {
                controller.OperationRecorder.Undo();
            }
            treeView.EndUpdate();
        }

        public void ActionUndo(Guid operation)
        {
            treeView.BeginUpdate();
            {
                controller.OperationRecorder.Undo(operation);
            }
            treeView.EndUpdate();
        }

        public void ActionPopulateUndoDropDown()
        {
            var dropDownItems = undoButton.DropDown.Items;
            dropDownItems.Clear();            
            var operationRecorder = controller.OperationRecorder;
            var undoables = new List<Guid>();
            operationRecorder.CollectUndoables(undoables);
            foreach (var undoable in undoables)
            {
                var operationName = controller.OperationRecorder.GetUndoOperationName(undoable);
                var item = dropDownItems.Add(operationName);
                item.Name = undoable.ToString();
            }
        }

        public void ActionRedo()
        {
            treeView.BeginUpdate();
            {
                controller.OperationRecorder.Redo();
            }
            treeView.EndUpdate();
        }

        public void ActionRedo(Guid operation)
        {
            treeView.BeginUpdate();
            {
                controller.OperationRecorder.Redo(operation);
            }
            treeView.EndUpdate();
        }

        public void ActionPopulateRedoDropDown()
        {
            var dropDownItems = redoButton.DropDown.Items;
            var operationRecorder = controller.OperationRecorder;
            dropDownItems.Clear();
            var redoables = operationRecorder.CollectRedoables(new List<Guid>());
            foreach (var redoable in redoables)
            {
                var operationName = controller.OperationRecorder.GetRedoOperationName(redoable);
                var item = dropDownItems.Add(operationName);
                item.Name = redoable.ToString();
            }
        }

        protected OxEngine Engine { get { return engine; } }

        protected abstract Type[] ComponentTokenTypesHook { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                controller.Selection.SelectionPropertyChanged -= selection_SelectionPropertyChanged;
                controller.Selection.SelectionChanged -= selection_SelectionChanged;
            }
            base.Dispose(disposing);
        }

        protected T GetEngine<T>() where T : class
        {
            return OxHelper.Cast<T>(engine);
        }

        protected abstract void SaveDocumentHook();

        protected abstract void LoadDocumentHook();

        protected abstract void NewDocumentHook();

        private void engine_ComponentNameConflicted(OxEngine sender)
        {
            SysWinForms.MessageBox.Show(
                "You should not have multiple components with the same name.\n\n" +
                "Components with duplicate names may have their names set to their guids at play-time",
                form.Text, MessageBoxButtons.OK);
        }

        private void selection_SelectionPropertyChanged(
            object sender, object target, string propertyName, object oldValue)
        {
            controller.OperationRecorder.Record("Change " + propertyName, target, propertyName, oldValue);
            // HACK: Calling propertyGrid.Refresh here every time will slow down the application
            // to a crawl. Therefore I set a dirty flag, invalidate propertyGrid, then call Refresh
            // when the propertyGrid is redrawn (and the dirty flag is true). Horrible, eh?
            RefreshPropertyGrid();
        }

        private void selection_SelectionChanged(object sender, IEnumerable oldSelection)
        {
            if (controller.Selection.Count == 0) propertyGrid.SelectedObject = null;
            else if (controller.Selection.Count == 1) propertyGrid.SelectedObject = controller.Selection.FirstOrNull;
            else propertyGrid.SelectedObjects = controller.Selection.ToArray();
        }

        private void propertyGrid_Paint(object sender, PaintEventArgs e)
        {
            if (!propertyGridDirty) return; // OPTIMIZATION
            propertyGridDirty = false;
            propertyGrid.Refresh();
        }

        private void PopulateComponentTypes()
        {
            comboBoxComponentType.Items.Clear();
            var componentTypes = ComponentTokenTypesHook;
            var componentTypeNames = componentTypes.Select(x => OxHelper.StripToken(x.Name));
            componentTypeNames.ForEach(x => comboBoxComponentType.Items.Add(x));
            if (componentTypes.Length != 0) comboBoxComponentType.SelectedIndex = 0;
        }

        private void Open()
        {
            try
            {
                controller.LoadDocument(openFileDialog.FileName);
                LoadDocumentHook();
                saveFileDialog.FileName = openFileDialog.FileName;
                richTextBoxConsole.WriteLine("Open... Success.");
            }
            catch (LoadDocumentException e)
            {
                MessageBox.Show(e.Message, form.Text, MessageBoxButtons.OK);
            }
        }

        private void Save()
        {
            try
            {
                SaveDocumentHook();
                controller.SaveDocument(saveFileDialog.FileName);
                openFileDialog.FileName = saveFileDialog.FileName;
                richTextBoxConsole.WriteLine("Save... Success.");
            }
            catch (SaveDocumentException e)
            {
                MessageBox.Show(e.Message, form.Text, MessageBoxButtons.OK);
            }
        }

        private void RefreshPropertyGrid()
        {
            propertyGridDirty = true;
            propertyGrid.Invalidate();
        }

        private readonly ToolStripSplitButton undoButton;
        private readonly ToolStripSplitButton redoButton;
        private readonly ToolStripComboBox comboBoxComponentType;
        private readonly EditorController controller;
        private readonly OpenFileDialog openFileDialog;
        private readonly SaveFileDialog saveFileDialog;
        private readonly PropertyGrid propertyGrid;
        private readonly RichTextBox richTextBoxConsole;
        private readonly TreeView treeView;
        private readonly OxEngine engine;
        private readonly Form form;
        private bool propertyGridDirty;
    }
}

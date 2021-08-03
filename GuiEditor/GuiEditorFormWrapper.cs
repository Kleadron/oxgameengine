using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Gui;
using Ox.Gui.Component;

namespace GuiEditorNamespace
{
    public class GuiEditorFormWrapper : EditorFormWrapper
    {
        public GuiEditorFormWrapper(OxEngine engine, GuiEditorController controller,
            Form form, PropertyGrid propertyGrid, ToolStripSplitButton undoButton,
            ToolStripSplitButton redoButton, ToolStripComboBox comboBoxComponentType,
            OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog,
            RichTextBox richTextBoxConsole, TreeView treeView, EditorCanvas canvas)
            : base(engine, controller, form, propertyGrid, undoButton, redoButton,
            comboBoxComponentType, openFileDialog, saveFileDialog, richTextBoxConsole,
            treeView)
        {
            this.controller = controller;
            this.comboBoxComponentType = comboBoxComponentType;
            this.canvas = canvas;

            treeViewWrapper = new RootedEditorTreeViewWrapper(Engine, controller, treeView);            

            canvas.MouseEnter += delegate { controller.Picked = true; };
            canvas.MouseLeave += delegate { controller.Picked = false; };
            canvas.Leave += delegate { rightMouseUpPosition = null; };
            canvas.MouseUp += canvas_MouseUp;
        }

        public void ActionCreateComponentInCanvas()
        {
            string componentType = OxHelper.AffixToken(comboBoxComponentType.Text);
            PlaceInCanvas(controller.CreateComponent(componentType));
        }

        public void ActionPasteInCanvas()
        {
            PlaceInCanvas(controller.Paste());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                canvas.MouseUp -= canvas_MouseUp;
                treeViewWrapper.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void SaveDocumentHook() { }

        protected override void LoadDocumentHook() { }

        protected override void NewDocumentHook() { }

        protected override Type[] ComponentTokenTypesHook
        {
            get { return GuiConfiguration.GuiConstructionDictionary.GetConstructedTypes(); }
        }

        private Vector2? CanvasCreationPosition
        {
            get
            {
                return rightMouseUpPosition != null ?
                    (Vector2?)(rightMouseUpPosition.Value).GetSnap(controller.Snap) : null;
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (controller.Root == null || controller.Selection.Count == 1)
                {
                    GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                    rightMouseUpPosition = guiSystem.AppMousePosition;
                }
                else rightMouseUpPosition = null;
            }
        }

        private void PlaceInCanvas(object item)
        {
            BaseGuiComponentToken guiComponent = item as BaseGuiComponentToken;
            if (guiComponent == null) return;
            Vector2? creationPosition = CanvasCreationPosition;
            if (creationPosition == null) return;

            controller.OperationRecorder.PushPause();
            {
                BaseGuiComponent parent = guiComponent.Instance.GetParent<BaseGuiComponent>();
                Vector2 offsetPosition = parent != null ? parent.PositionWorld : Vector2.Zero;
                Vector2 createPosition = creationPosition.Value - offsetPosition;
                guiComponent.Position = createPosition;
            }
            controller.OperationRecorder.PopPause();
        }

        private readonly RootedEditorTreeViewWrapper treeViewWrapper;
        private readonly GuiEditorController controller;
        private readonly ToolStripComboBox comboBoxComponentType;
        private readonly EditorCanvas canvas;
        private Vector2? rightMouseUpPosition;
    }
}

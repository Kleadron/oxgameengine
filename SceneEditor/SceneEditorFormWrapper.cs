using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.MathNamespace;
using Ox.Gui;
using Ox.Scene;
using Ox.Scene.Component;

namespace SceneEditorNamespace
{
    public class SceneEditorFormWrapper : GroupedEditorFormWrapper
    {
        public SceneEditorFormWrapper(OxEngine engine, SceneEditorController controller,
            Form form, PropertyGrid propertyGrid, ToolStripSplitButton undoButton,
            ToolStripSplitButton redoButton, ToolStripComboBox comboBoxComponentType,
            OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog,
            RichTextBox richTextBoxConsole, TreeView treeView,
            ToolStripTextBox textBoxPositionSnap, ToolStripTextBox textBoxScaleSnap,
            ToolStripTextBox textBoxOrientationSnap, ToolStripTextBox textBoxCreationDepth,
            EditorCanvas canvas)
            : base(engine, controller, form, propertyGrid, undoButton, redoButton,
            comboBoxComponentType, openFileDialog, saveFileDialog, richTextBoxConsole, treeView)
        {
            this.controller = controller;
            this.comboBoxComponentType = comboBoxComponentType;
            this.textBoxPositionSnap = textBoxPositionSnap;
            this.textBoxScaleSnap = textBoxScaleSnap;
            this.textBoxOrientationSnap = textBoxOrientationSnap;
            this.textBoxCreationDepth = textBoxCreationDepth;
            this.canvas = canvas;

            treeViewWrapper = new GroupedEditorTreeViewWrapper(Engine, controller, treeView);

            ResetSettings();

            canvas.Leave += delegate { rightMouseUpPosition = null; };
            canvas.MouseEnter += delegate { controller.Picked = true; };
            canvas.MouseLeave += delegate { controller.Picked = false; };
            canvas.MouseUp += canvas_MouseUp;
        }

        public void ActionAutoBox()
        {
            controller.AutoBox();
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

        private Vector3? CanvasCreationPosition
        {
            get
            {
                if (rightMouseUpPosition == null) return null;
                Camera camera = Engine.Camera;
                Viewport viewport = Engine.GraphicsDevice.Viewport;
                Vector3 cameraForward = controller.CameraForward;
                Vector2 mousePositionViewport = viewport.FromVirtual(rightMouseUpPosition.Value);
                Segment mousePositionWorld = viewport.ToWorld(camera, mousePositionViewport);
                Ray creationRay = new Ray(mousePositionWorld.End, mousePositionWorld.Direction);
                Vector3 creationPlanePosition = camera.Position + cameraForward * controller.CreationDepth;
                Plane creationPlane = PlaneHelper.CreateFromPositionAndNormal(creationPlanePosition, cameraForward);
                Vector3 rayPlaneIntersection;
                OxMathHelper.Intersection(ref creationRay, ref creationPlane, out rayPlaneIntersection);
                return rayPlaneIntersection.GetSnap(controller.PositionSnap);
            }
        }

        private void PlaceInCanvas(object item)
        {
            SceneComponentToken sceneComponent = item as SceneComponentToken;
            if (sceneComponent == null) return;
            Vector3? creationPosition = CanvasCreationPosition;
            if (creationPosition == null) return;

            controller.OperationRecorder.PushPause();
            {
                sceneComponent.Position = creationPosition.Value;
            }
            controller.OperationRecorder.PopPause();
        }

        protected override void SaveDocumentHook() { }

        protected override void LoadDocumentHook()
        {
            ConfigureSettings();
        }

        protected override void NewDocumentHook()
        {
            ResetSettings();
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

        protected override Type[] ComponentTokenTypesHook
        {
            get { return SceneConfiguration.SceneConstructionDictionary.GetConstructedTypes(); }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                rightMouseUpPosition = guiSystem.AppMousePosition;
            }
        }

        private void ResetSettings()
        {
            controller.ResetSettings();
            ConfigureSettings();
        }

        private void ConfigureSettings()
        {
            textBoxPositionSnap.Text = controller.PositionSnap.ToString();
            textBoxScaleSnap.Text = controller.ScaleSnap.ToString();
            textBoxOrientationSnap.Text = controller.OrientationSnap.ToString();
            textBoxCreationDepth.Text = controller.CreationDepth.ToString();
        }

        private readonly GroupedEditorTreeViewWrapper treeViewWrapper;
        private readonly SceneEditorController controller;
        private readonly ToolStripComboBox comboBoxComponentType;
        private readonly ToolStripTextBox textBoxPositionSnap;
        private readonly ToolStripTextBox textBoxScaleSnap;
        private readonly ToolStripTextBox textBoxOrientationSnap;
        private readonly ToolStripTextBox textBoxCreationDepth;
        private readonly EditorCanvas canvas;
        private Vector2? rightMouseUpPosition;
    }
}

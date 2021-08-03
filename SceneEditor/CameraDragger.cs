using Microsoft.Xna.Framework;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace SceneEditorNamespace
{
    public class CameraDragger : MouseDragger<SceneEditorController>
    {
        public CameraDragger(OxEngine engine, SceneEditorController component, Document document,
            Camera camera, EularOrientation cameraOrientation)
            : base(engine, component, dragBeginDelay)
        {
            this.cameraOrientation = cameraOrientation;
            this.document = document;
            this.camera = camera;
        }

        protected override void PrepareDragHook(Vector2 mousePosition)
        {
            dragOffset = mousePosition;
            switch (Modifier)
            {
                case KeyboardModifier.ControlShift: dragMode = DragMode.Look; break;
                case KeyboardModifier.Control: dragMode = DragMode.LookAt; break;
                case KeyboardModifier.Shift: dragMode = DragMode.UpPlane; break;
                case KeyboardModifier.None: dragMode = DragMode.ForwardPlane; break;
            }
        }

        protected override void BeginDragHook(Vector2 mousePosition) { }

        protected override void UpdateDragHook(Vector2 mousePosition)
        {
            switch (dragMode)
            {
                case DragMode.ForwardPlane: DragForwardPlane(mousePosition); break;
                case DragMode.UpPlane: DragUpPlane(mousePosition); break;
                case DragMode.LookAt: DragLookAt(mousePosition); break;
                case DragMode.Look: DragLook(mousePosition); break;
            }
        }

        protected override void EndDragHook(Vector2 mousePosition) { }

        private enum DragMode
        {
            ForwardPlane = 0,
            UpPlane,
            LookAt,
            Look
        }

        private void DragForwardPlane(Vector2 mousePosition)
        {
            Vector2 dragAmount = mousePosition - dragOffset;
            Component.CameraPosition +=
                dragAmount.X * -cameraOrientation.Right +
                dragAmount.Y * cameraOrientation.Up;
            dragOffset = mousePosition;
        }

        private void DragUpPlane(Vector2 mousePosition)
        {
            Vector2 dragAmount = mousePosition - dragOffset;
            Component.CameraPosition +=
                dragAmount.X * -cameraOrientation.Right +
                dragAmount.Y * cameraOrientation.Forward;
            dragOffset = mousePosition;
        }

        private void DragLookAt(Vector2 mousePosition)
        {
            Vector2 dragAmount = mousePosition - dragOffset;
            cameraOrientation.Angle1 += dragAmount.Y * -angleMultiplier;
            cameraOrientation.Angle2 += dragAmount.X * -angleMultiplier;
            Component.CameraPosition +=
                dragAmount.X * -cameraOrientation.Right * lookAtMultiplier +
                dragAmount.Y * cameraOrientation.Up * lookAtMultiplier;
            dragOffset = mousePosition;
        }

        private void DragLook(Vector2 mousePosition)
        {
            Vector2 dragAmount = mousePosition - dragOffset;
            cameraOrientation.Angle1 += dragAmount.Y * -angleMultiplier;
            cameraOrientation.Angle2 += dragAmount.X * -angleMultiplier;
            dragOffset = mousePosition;
        }

        private const double dragBeginDelay = 0;
        private const float lookAtMultiplier = 0.5f;
        private const float angleMultiplier = 0.005f;

        private readonly EularOrientation cameraOrientation;
        private readonly Camera camera;
        private readonly Document document;
        private DragMode dragMode;
        private Vector2 dragOffset;
    }
}

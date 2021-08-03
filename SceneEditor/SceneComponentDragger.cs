using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Scene.Component;

namespace SceneEditorNamespace
{
    public enum Axis
    {
        X = 0, Y, Z, V, XY, YZ, ZX
    }

    public class SceneComponentDragger : MouseDragger<SceneEditorController>
    {
        public SceneComponentDragger(OxEngine engine, SceneEditorController parent, Document document, Camera camera)
            : base(engine, parent, dragBeginDelay)
        {
            this.document = document;
            this.camera = camera;
        }

        public Axis Axis
        {
            get { return axis; }
            set { axis = value; }
        }

        public float PositionSnap
        {
            get { return positionSnap; }
            set { positionSnap = value; }
        }

        public float ScaleSnap
        {
            get { return scaleSnap; }
            set { scaleSnap = value; }
        }

        public float OrientationSnap
        {
            get { return orientationSnap; }
            set { orientationSnap = value; }
        }

        protected override void PrepareDragHook(Vector2 mousePosition)
        {
            selected = FirstSelectedComponent;
            if (!CanDrag) return;
            componentDragSelectedPosition = selected.Position;
            componentDragSelectedScale = selected.Scale;
            componentDragSelectedOrientation = selected.Orientation;
            Viewport viewport = Engine.GraphicsDevice.Viewport;
            Vector2 mousePositionViewport = viewport.FromVirtual(mousePosition);
            Segment mousePositionWorld = viewport.ToWorld(camera, mousePositionViewport);
            Vector3 mousePositionObjectPlane = WorldToObjectPlane(componentDragSelectedPosition, mousePositionWorld.Start);
            componentDragOffset = mousePositionObjectPlane - selected.Position;
            switch (Modifier)
            {
                case KeyboardModifier.Control: dragMode = DragMode.Scale; break;
                case KeyboardModifier.Shift: dragMode = DragMode.Orientation; break;
                case KeyboardModifier.None:
                case KeyboardModifier.ControlShift: dragMode = DragMode.Position; break;
            }
        }

        protected override void BeginDragHook(Vector2 mousePosition)
        {
            if (!CanDrag) return;
            Component.OperationRecorder.PushGroup();
        }

        protected override void UpdateDragHook(Vector2 mousePosition)
        {
            if (!CanDrag) return;
            Viewport viewport = Engine.GraphicsDevice.Viewport;
            Vector2 mousePositionViewport = OxMathHelper.FromVirtual(viewport, mousePosition);
            Segment mousePositionWorld = viewport.ToWorld(camera, mousePositionViewport);
            Vector3 mousePositionObjectPlane = WorldToObjectPlane(componentDragSelectedPosition, mousePositionWorld.Start);
            Vector3 delta = mousePositionObjectPlane - componentDragOffset - componentDragSelectedPosition;
            switch (axis)
            {
                case Axis.X: delta = OxMathHelper.ComponentVector(delta, Vector3.Right); break;
                case Axis.Y: delta = OxMathHelper.ComponentVector(delta, Vector3.Up); break;
                case Axis.Z: delta = OxMathHelper.ComponentVector(delta, Vector3.Backward); break;
                case Axis.V: break;
                case Axis.XY: delta = OxMathHelper.ComponentVector(delta, Vector3.Right) + OxMathHelper.ComponentVector(delta, Vector3.Up); break;
                case Axis.YZ: delta = OxMathHelper.ComponentVector(delta, Vector3.Up) + OxMathHelper.ComponentVector(delta, Vector3.Backward); break;
                case Axis.ZX: delta = OxMathHelper.ComponentVector(delta, Vector3.Backward) + OxMathHelper.ComponentVector(delta, Vector3.Right); break;
            }
            switch (dragMode)
            {
                case DragMode.Position:
                    Vector3 newPosition = componentDragSelectedPosition + delta;
                    Vector3 newSnappedPosition = newPosition.GetSnap(positionSnap);
                    Vector3 newDeltaPosition = newSnappedPosition - selected.Position;
                    foreach (SceneComponentToken component in Selection.OfType<SceneComponentToken>()) component.Position += newDeltaPosition;
                    break;
                case DragMode.Scale:
                    Vector3 newScale = componentDragSelectedScale + delta;
                    Vector3 newSnappedScale = newScale.GetSnap(scaleSnap);
                    Vector3 newDeltaScale = newSnappedScale - selected.Scale;
                    foreach (SceneComponentToken component in Selection.OfType<SceneComponentToken>()) component.Scale += newDeltaScale;
                    break;
                case DragMode.Orientation:
                    Vector3 newOrientation = componentDragSelectedOrientation + delta * orientationCoefficient;
                    Vector3 newSnappedOrientation = newOrientation.GetSnap(orientationSnap);
                    Vector3 newDeltaOrientation = newSnappedOrientation - selected.Orientation;
                    foreach (SceneComponentToken component in Selection.OfType<SceneComponentToken>()) component.Orientation += newDeltaOrientation;
                    break;
            }
        }

        protected override void EndDragHook(Vector2 mousePosition)
        {
            if (!CanDrag) return;
            switch (dragMode)
            {
                case DragMode.Position:
                    Vector3 deltaPosition = selected.Position - componentDragSelectedPosition;
                    foreach (SceneComponentToken component in Selection.OfType<SceneComponentToken>())
                    {
                        Vector3 oldPosition = component.Position - deltaPosition;
                        Vector3 newPosition = component.Position;
                        Action undo = delegate { component.Position = oldPosition; };
                        Action redo = delegate { component.Position = newPosition; };
                        Component.OperationRecorder.Record("Change Position", undo, redo);
                    }
                    break;
                case DragMode.Scale:
                    Vector3 deltaScale = selected.Scale - componentDragSelectedScale;
                    foreach (SceneComponentToken component in Selection.OfType<SceneComponentToken>())
                    {
                        Vector3 oldScale = component.Scale - deltaScale;
                        Vector3 newScale = component.Scale;
                        Action undo = delegate { component.Scale = oldScale; };
                        Action redo = delegate { component.Scale = newScale; };
                        Component.OperationRecorder.Record("Change Scale", undo, redo);
                    }
                    break;
                case DragMode.Orientation:
                    Vector3 deltaOrientation = selected.Orientation - componentDragSelectedOrientation;
                    foreach (SceneComponentToken component in Selection.OfType<SceneComponentToken>())
                    {
                        Vector3 oldOrientation = component.Orientation - deltaOrientation;
                        Vector3 newOrientation = component.Orientation;
                        Action undo = delegate { component.Orientation = oldOrientation; };
                        Action redo = delegate { component.Orientation = newOrientation; };
                        Component.OperationRecorder.Record("Change Orientation", undo, redo);
                    }
                    break;
            }
            Component.OperationRecorder.PopGroup();
        }

        private enum DragMode
        {
            Position = 0,
            Scale,
            Orientation
        }

        private bool CanDrag { get { return selected != null; } }
        
        /// <summary>May be null.</summary>
        private SceneComponentToken FirstSelectedComponent
        {
            get
            {
                Func<object, bool> componentFilter = x => x is SceneComponentToken;
                object component = Selection.FirstOrDefault(componentFilter);
                return OxHelper.Cast<SceneComponentToken>(component);
            }
        }
        
        private Selection Selection
        {
            get { return document.Selection; }
        }
        
        private Vector3 WorldToObjectPlane(Vector3 objectPos, Vector3 nearPlanePos)
        {
            Matrix viewProjection;
            camera.GetViewProjection(out viewProjection);
            Vector4 objPos4 = new Vector4(objectPos, 1.0f);
            Vector4 nearPos4 = new Vector4(nearPlanePos, 1.0f);
            Vector4 objPosCS = Vector4.Transform(objPos4, viewProjection);
            Vector4 nearPosCS = Vector4.Transform(nearPos4, viewProjection);
            objPosCS /= objPosCS.W;
            nearPosCS /= nearPosCS.W;
            objPosCS.X = nearPosCS.X;
            objPosCS.Y = nearPosCS.Y;
            Vector4 newPosWS = Vector4.Transform(objPosCS, Matrix.Invert(viewProjection));
            return new Vector3(newPosWS.X, newPosWS.Y, newPosWS.Z) / newPosWS.W;
        }
        
        private const double dragBeginDelay = 0.15;
        private const float orientationCoefficient = 30;

        private readonly Camera camera;
        private readonly Document document;
        /// <summary>May be null.</summary>
        private SceneComponentToken selected;
        private DragMode dragMode;
        private Vector3 componentDragOffset;
        private Vector3 componentDragSelectedPosition;
        private Vector3 componentDragSelectedScale;
        private Vector3 componentDragSelectedOrientation;
        private Axis axis;
        private float positionSnap;
        private float scaleSnap;
        private float orientationSnap;
    }
}

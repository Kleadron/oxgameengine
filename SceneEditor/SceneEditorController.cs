using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui;
using Ox.Gui.Component;
using Ox.Gui.Event;
using Ox.Scene;
using Ox.Scene.Component;

namespace SceneEditorNamespace
{
    public class SceneEditorController : GroupedEditorController
    {
        public SceneEditorController(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            camera = engine.Camera;
            cameraDragger = new CameraDragger(engine, this, Document, camera, cameraOrientation);
            cameraOrientation.OrientationChanged += delegate { UpdateCameraView(); };
            ResetCamera();

            componentDragger = new SceneComponentDragger(engine, this, Document, camera);            

            AddGarbage(screen = new Panel(engine, domainName, false));
            screen.Position = OxConfiguration.VirtualScreen.Position;
            screen.Scale = OxConfiguration.VirtualScreen.Scale;
            screen.Color = new Color();
            screen.MouseButtonAction += screen_MouseButtonAction;
            screen.MouseWheelAction += screen_MouseWheelAction;
            engine.GetService<GuiSystem>().Screen = screen;

            AddGarbage(selectionVisualizer = new SceneSelectionVisualizer(engine, domainName, false, Document));
            AddGarbage(axisTriad = new AxisTriad(engine, domainName, false));
        }

        public Vector3 CameraRight { get { return cameraOrientation.Right; } }

        public Vector3 CameraUp { get { return cameraOrientation.Up; } }

        public Vector3 CameraForward { get { return cameraOrientation.Forward; } }

        public Vector3 CameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                _cameraPosition = value;
                UpdateCameraView();
            }
        }

        public Vector3 CameraOrientation
        {
            get
            {
                return new Vector3(
                    MathHelper.ToDegrees(cameraOrientation.Angle1),
                    MathHelper.ToDegrees(cameraOrientation.Angle2),
                    MathHelper.ToDegrees(cameraOrientation.Angle3));
            }
            set
            {
                cameraOrientation.Angle1 = MathHelper.ToRadians(value.X);
                cameraOrientation.Angle2 = MathHelper.ToRadians(value.Y);
                cameraOrientation.Angle3 = MathHelper.ToRadians(value.Z);
            }
        }

        public Axis Axis
        {
            get { return componentDragger.Axis; }
            set { componentDragger.Axis = value; }
        }

        public float PositionSnap
        {
            get { return componentDragger.PositionSnap; }
            set { componentDragger.PositionSnap = value; }
        }

        public float ScaleSnap
        {
            get { return componentDragger.ScaleSnap; }
            set { componentDragger.ScaleSnap = value; }
        }

        public float OrientationSnap
        {
            get { return componentDragger.OrientationSnap; }
            set { componentDragger.OrientationSnap = value; }
        }

        public float CreationDepth
        {
            get { return creationDepth; }
            set { creationDepth = value; }
        }

        public bool Focused
        {
            get { return _focused; }
            set
            {
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                _focused = value;
                guiSystem.EventsEnabled = value;
            }
        }

        public bool Picked
        {
            get { return picked; }
            set { picked = value; }
        }

        /// <summary>
        /// Find a component at the mouse's position.
        /// May return null.
        /// </summary>
        /// <param name="mousePosition">The mouse position at which to search.</param>
        public SceneComponentToken FindComponent(Vector2 mousePosition)
        {
            Viewport viewport = Engine.GraphicsDevice.Viewport;
            Vector2 mousePositionViewport = viewport.FromVirtual(mousePosition);
            Segment mousePositionWorld = viewport.ToWorld(camera, mousePositionViewport);
            Vector3[] mousePositionWorldPoints = new Vector3[] { mousePositionWorld.Start, mousePositionWorld.End };
            IList<SceneComponentToken> foundComponents = new List<SceneComponentToken>();
            IList<SceneComponentToken> components = new List<SceneComponentToken>();
            Document.Collect(components);

            for (int i = components.Count - 1; i > -1; --i)
            {
                SceneComponentToken component = components[i];
                BoundingBox boundingBoxWorld = component.Instance.BoundingBoxWorld;
                if (!component.Frozen &&
                    boundingBoxWorld.Intersects(ref mousePositionWorld))
                    foundComponents.Add(component);
            }

            SceneComponentToken nearestComponent = null;
            float nearestComponentDistance = float.MaxValue;

            foreach (SceneComponentToken component in foundComponents)
            {
                float distance;
                BoundingBox boundingBox = component.Instance.BoundingBoxWorld;
                if (boundingBox.Intersects(ref mousePositionWorld, out distance))
                {
                    if (distance < nearestComponentDistance)
                    {
                        nearestComponent = component;
                        nearestComponentDistance = distance;
                    }
                }
            }

            return nearestComponent;
        }

        public void Look(Direction direction)
        {
            cameraOrientation.ClearAngles();
            switch (direction)
            {
                case Direction.Up: cameraOrientation.Angle1 = MathHelper.PiOver2; break;
                case Direction.Down: cameraOrientation.Angle1 = -MathHelper.PiOver2; break;
                case Direction.Left: cameraOrientation.Angle2 = MathHelper.PiOver2; break;
                case Direction.Right: cameraOrientation.Angle2 = -MathHelper.PiOver2; break;
                case Direction.Backward: cameraOrientation.Angle2 = MathHelper.Pi; break;
            }
        }

        public void AutoBox()
        {
            OperationRecorder.PushGroup();
            {
                Selection.OfType<SceneComponentToken>().ForEach(x => AutoBox(x));
            }
            OperationRecorder.PopGroup();
        }

        public void ResetSettings()
        {
            PositionSnap = 1;
            ScaleSnap = 1.0f / 8.0f;
            OrientationSnap = 15;
            CreationDepth = 100;
            ResetCamera();
        }

        private Vector3 ComponentSpawnPosition
        {
            get
            {
                Vector3 position = camera.Position + cameraOrientation.Forward * creationDepth;
                Vector3 snappedPosition = position.GetSnap(componentDragger.PositionSnap);
                return snappedPosition;
            }
        }

        private void AutoBox(SceneComponentToken component)
        {
            BoundingBox boundingBox;
            if (component.Instance.TryGenerateBoundingBox(out boundingBox))
                component.Box = new Box(boundingBox);
        }

        protected new SceneDocument Document
        {
            get { return OxHelper.Cast<SceneDocument>(base.Document); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) componentDragger.Dispose();
            base.Dispose(disposing);
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            if (Focused) UpdateCamera(gameTime);
        }

        private void UpdateCameraView()
        {
            camera.SetTransformByLookForward(_cameraPosition, cameraOrientation.Up, cameraOrientation.Forward);
        }

        protected override void SaveDocumentHook()
        {
            Document.CameraPosition = CameraPosition;
            Document.CameraOrientation = CameraOrientation;
            Document.PositionSnap = PositionSnap;
            Document.ScaleSnap = ScaleSnap;
            Document.OrientationSnap = OrientationSnap;
            Document.CreationDepth = CreationDepth;
        }

        protected override void LoadDocumentHook()
        {
            CameraPosition = Document.CameraPosition;
            CameraOrientation = Document.CameraOrientation;
            PositionSnap = Document.PositionSnap;
            ScaleSnap = Document.ScaleSnap;
            OrientationSnap = Document.OrientationSnap;
            CreationDepth = Document.CreationDepth;
        }

        protected override void NewDocumentHook()
        {
            ResetCamera();
        }

        protected override Document CreateDocumentHook()
        {
            return new SceneDocument(Engine);
        }

        protected override void SetUpComponentHook(ComponentToken component, ItemCreationStyle creationStyle)
        {
            if (creationStyle != ItemCreationStyle.Load &&
                creationStyle != ItemCreationStyle.External &&
                creationStyle != ItemCreationStyle.Clone &&
                creationStyle != ItemCreationStyle.Undelete &&
                creationStyle != ItemCreationStyle.Replacement)
            {
                SceneComponentToken componentAsScene = component as SceneComponentToken;
                if (componentAsScene != null) componentAsScene.Position = ComponentSpawnPosition;
            }
        }

        private void screen_MouseButtonAction(BaseGuiComponent sender, InputType inputType, MouseButton button, Vector2 mousePosition)
        {
            OxHelper.ArgumentNullCheck(sender);
            switch (button)
            {
                case MouseButton.Left: HandleLeftMouseButton(inputType, mousePosition); break;
                case MouseButton.Right: HandleRightMouseButton(inputType, mousePosition); break;
                case MouseButton.Middle: HandleMiddleMouseButton(inputType, mousePosition); break;
            }
        }

        private void screen_MouseWheelAction(BaseGuiComponent sender, int wheelDelta, Vector2 mousePosition)
        {
            // NOTE: this event never gets raised when the controller is running inside Windows
            // Forms... A hack is needed to make it work.
        }

        private void HandleLeftMouseButton(InputType inputType, Vector2 mousePosition)
        {
            HandleLeftSelection(inputType, mousePosition);
            HandleComponentManipulation(inputType, mousePosition);
        }

        private void HandleRightMouseButton(InputType inputType, Vector2 mousePosition)
        {
            HandleRightSelection(inputType, mousePosition);
        }

        private void HandleMiddleMouseButton(InputType inputType, Vector2 mousePosition)
        {
            HandleCameraManipulation(inputType, mousePosition);
        }

        private void HandleComponentManipulation(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickUp)
            {
                componentDragger.HandleButton(inputType, mousePosition);
            }
            else if (picked)
            {
                componentDragger.HandleButton(inputType, mousePosition);
            }
        }

        private void HandleCameraManipulation(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickUp)
            {
                cameraDragger.HandleButton(inputType, mousePosition);
            }
            else if (picked)
            {
                cameraDragger.HandleButton(inputType, mousePosition);
            }
        }

        private void HandleLeftSelection(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickDown && picked)
            {
                KeyboardState keyboardState = Engine.KeyboardState;
                if (keyboardState.GetControlState()) AddSelection(mousePosition);
                else if (keyboardState.IsKeyDown(Keys.Escape)) RemoveSelection(mousePosition);
                else LeftSetSelection(mousePosition);
            }
        }

        private void HandleRightSelection(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickDown && picked && Selection.Count <= 1)
            {
                RightSetSelection(mousePosition);
            }
        }

        private void AddSelection(Vector2 mousePosition)
        {
            SceneComponentToken pickedComponent = FindComponent(mousePosition);
            if (pickedComponent != null) Selection.Add(pickedComponent);
        }

        private void RemoveSelection(Vector2 mousePosition)
        {
            SceneComponentToken pickedComponent = FindComponent(mousePosition);
            if (pickedComponent != null) Selection.Remove(pickedComponent);
        }

        private void LeftSetSelection(Vector2 mousePosition)
        {
            SceneComponentToken pickedComponent = FindComponent(mousePosition);
            if (pickedComponent == null) Selection.Clear();
            else
            {
                bool componentSelected = Selection.Contains(pickedComponent);
                if (!componentSelected) Selection.Set(pickedComponent);
            }
        }

        private void RightSetSelection(Vector2 mousePosition)
        {
            SceneComponentToken pickedComponent = FindComponent(mousePosition);
            if (pickedComponent != null) Selection.Set(pickedComponent);
        }

        private void ResetCamera()
        {
            CameraPosition = -cameraOrientation.Forward * creationDepth;
            CameraOrientation = Vector3.Zero;
        }

        private void UpdateCamera(GameTime gameTime)
        {
            KeyboardState keyboard = Engine.KeyboardState;
            if (keyboard.GetControlState()) return;
            float motionMultiplier = CalculateMotionMultiplier(gameTime);
            float moveSpeed = cameraMoveSpeed * motionMultiplier;
            float turnSpeed = cameraTurnSpeed * motionMultiplier;
            if (keyboard.IsKeyDown(Keys.W)) CameraPosition += cameraOrientation.Forward * moveSpeed;
            if (keyboard.IsKeyDown(Keys.S)) CameraPosition -= cameraOrientation.Forward * moveSpeed;
            if (keyboard.IsKeyDown(Keys.A)) cameraOrientation.Angle2 += turnSpeed;
            if (keyboard.IsKeyDown(Keys.D)) cameraOrientation.Angle2 -= turnSpeed;
            if (keyboard.IsKeyDown(Keys.Q) || keyboard.IsKeyDown(Keys.Left)) CameraPosition -= cameraOrientation.Right * moveSpeed;
            if (keyboard.IsKeyDown(Keys.E) || keyboard.IsKeyDown(Keys.Right)) CameraPosition += cameraOrientation.Right * moveSpeed;
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.R)) CameraPosition += cameraOrientation.Up * moveSpeed;
            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.F)) CameraPosition -= cameraOrientation.Up * moveSpeed;
        }

        private float CalculateMotionMultiplier(GameTime gameTime)
        {
            bool shiftState = Engine.KeyboardState.GetShiftState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float inputMultiplier = shiftState ? 2 : 1;
            return delta * inputMultiplier;
        }

        private const float cameraMoveSpeed = 128;
        private const float cameraTurnSpeed = 4;

        private readonly SceneSelectionVisualizer selectionVisualizer;
        private readonly SceneComponentDragger componentDragger;
        private readonly EularOrientation cameraOrientation = new EularOrientation();
        private readonly CameraDragger cameraDragger;
        private readonly Camera camera;
        private readonly AxisTriad axisTriad;
        private Panel screen;
        private float creationDepth;
        private bool picked;
        private Vector3 _cameraPosition;
        private bool _focused;
    }
}

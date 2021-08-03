using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Gui.Event;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// Compares the x-axis position of gui components.
    /// </summary>
    public class HorizontalComparer2D : IComparer<BaseGuiComponent>
    {
        public int Compare(BaseGuiComponent x, BaseGuiComponent y)
        {
            Vector2 xPosition = x.PositionWorld; // OPTIMIZATION: cache property
            Vector2 yPosition = y.PositionWorld; // OPTIMIZATION: cache property
            if (xPosition.X < yPosition.X) return -1;
            if (xPosition.X > yPosition.X) return 1;
            if (xPosition.Y < yPosition.Y) return -1;
            if (xPosition.Y > yPosition.Y) return 1;
            return 0;
        }
    }

    /// <summary>
    /// Compares the y-axis position of gui components.
    /// </summary>
    public class VerticalComparer2D : IComparer<BaseGuiComponent>
    {
        public int Compare(BaseGuiComponent x, BaseGuiComponent y)
        {
            Vector2 xPosition = x.PositionWorld; // OPTIMIZATION: cache property
            Vector2 yPosition = y.PositionWorld; // OPTIMIZATION: cache property
            if (xPosition.Y < yPosition.Y) return -1;
            if (xPosition.Y > yPosition.Y) return 1;
            if (xPosition.X < yPosition.X) return -1;
            if (xPosition.X > yPosition.X) return 1;
            return 0;
        }
    }

    /// <summary>
    /// Compares the drawing Z of gui components.
    /// </summary>
    public class ZComparer : IComparer<BaseGuiComponent>
    {
        public int Compare(BaseGuiComponent x, BaseGuiComponent y)
        {
            return OxMathHelper.InvertedCompare(x.ZWorld, y.ZWorld);
        }
    }

    /// <summary>
    /// A graphical user interface component.
    /// </summary>
    public class BaseGuiComponent : OxComponent
    {
        /// <summary>
        /// Create a BaseGuiComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="viewType">The type of view to use.</param>
        public BaseGuiComponent(OxEngine engine, string domainName, bool ownedByDomain, Type viewType)
            : base(engine, domainName, ownedByDomain)
        {
            view = CreateView(viewType);

            ChildAdded += this_ChildAdded;
            ChildRemoved += this_ChildRemoved;
            GuiAction += this_GuiAction;
            GuiHierarchyAction += this_GuiHierarchyAction;
            KeyAction += this_KeyAction;
            MousePositionUpdate += this_MousePositionUpdate;
            GamePadStateUpdate += this_GamePadStateUpdate;
            MouseButtonAction += this_MouseButtonAction;
            MouseWheelAction += this_MouseWheelAction;
            DirectionAction += this_DirectionAction;
            AbstractAction += this_AbstractAction;
        }
        
        /// <summary>
        /// The child component focused by the component.
        /// May be null.
        /// </summary>
        public BaseGuiComponent FocusedComponent
        {
            get { return focusedComponent; }
            set { focusedComponent = value; }
        }
        
        /// <summary>
        /// The child component that the mouse is hovering over.
        /// May be null.
        /// </summary>
        public BaseGuiComponent PickedComponent
        {
            get
            {
                BaseGuiComponent result = null;

                CollectChildren<BaseGuiComponent>(CollectionAlgorithm.Shallow, pickedAndCanFocusByMouseInputPredicate, cachedChildren);
                {
                    result = FindNearest(cachedChildren);
                }
                cachedChildren.Clear();

                return result;
            }
        }
        
        /// <summary>
        /// The position.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                UpdatePositionWorld(); // put self in a valid state before firing events
                RaiseBoundsChangedEvent(false);
                RaisePositionChangedEvent(false);
            }
        }
        
        /// <summary>
        /// The position in world space.
        /// </summary>
        public Vector2 PositionWorld
        {
            get { return _positionWorld; }
            set
            {
                BaseGuiComponent parent = GetParent<BaseGuiComponent>();
                Vector2 parentPosition = parent == null ? Vector2.Zero : parent.PositionWorld;
                Position = value - parentPosition;
            }
        }
        
        /// <summary>
        /// The Vector2 to convert from local to world position. Apply via addition.
        /// </summary>
        public Vector2 PositionLocalToWorld { get { return PositionWorld; } }
        
        /// <summary>
        /// The Vector2 to convert from world to local position. Apply via addition.
        /// </summary>
        public Vector2 PositionWorldToLocal { get { return -PositionWorld; } }
        
        /// <summary>
        /// The scale.
        /// </summary>
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return;
                _scale = value;
                UpdateScale(); // put self in a valid state before firing events
                RaiseBoundsChangedEvent(false);
                RaiseScaleChangedEvent();
            }
        }
        
        /// <summary>
        /// The color.
        /// </summary>
        public Color Color
        {
            get { return view.Color; }
            set
            {
                if (view.Color == value) return;
                view.Color = value;
                RaiseGuiEvent(GuiEventType.ColorChanged);
            }
        }
        
        /// <summary>
        /// The rectangle.
        /// </summary>
        public Rect Rect { get { return new Rect(Position.X, Position.Y, Scale.X, Scale.Y); } }
        
        /// <summary>
        /// The rectangle in world space.
        /// </summary>
        public Rect RectWorld { get { return new Rect(PositionWorld.X, PositionWorld.Y, Scale.X, Scale.Y); } }
        
        /// <summary>
        /// The clipping rectangle.
        /// </summary>
        public Rect Bounds { get { return new Rect(Position.X, Position.Y, Scale.X, Scale.Y); } }
        
        /// <summary>
        /// The cumulative clipping rectangle.
        /// </summary>
        public Rect BoundsWorld { get { return _boundsWorld; } }
        
        /// <summary>
        /// The drawing Z.
        /// </summary>
        public float Z
        {
            get { return _z; }
            set
            {
                if (_z == value) return;
                _z = value;
                UpdateZWorld(); // put self in a valid state before firing events
                RaiseZChangedEvent(false);
            }
        }
        
        /// <summary>
        /// The drawing Z in world space.
        /// </summary>
        public float ZWorld
        {
            get { return _zWorld; }
            set
            {
                BaseGuiComponent parent = GetParent<BaseGuiComponent>();
                float parentZ = parent == null ? 0 : parent.ZWorld;
                Z = value - parentZ;
            }
        }
        
        /// <summary>
        /// The float to convert from local to world Z. Apply via addition.
        /// </summary>
        public float ZLocalToWorld { get { return ZWorld; } }
        
        /// <summary>
        /// The float to convert from world to local Z. Apply via addition.
        /// </summary>
        public float ZWorldToLocal { get { return -ZWorld; } }
        
        /// <summary>
        /// The state of activity.
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active == value) return;
                _active = value;
                UpdateActiveWorld(); // put self in a valid state before firing events
                RaiseActiveChangedEvent(false);
            }
        }
        
        /// <summary>
        /// The cumulative state of activity.
        /// </summary>
        public bool ActiveWorld { get { return _activeWorld; } }
        
        /// <summary>
        /// The visibility.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return;
                _visible = value;
                UpdateVisibleWorld(); // put self in a valid state before firing events
                RaiseVisibleChangedEvent(false);
            }
        }
        
        /// <summary>
        /// The cumulative visibility.
        /// </summary>
        public bool VisibleWorld { get { return _visibleWorld; } }
        
        /// <summary>
        /// Does the component block its parent and sibling components from gaining focus?
        /// </summary>
        public bool Modal
        {
            get { return modal; }
            set { modal = value; }
        }
        
        /// <summary>
        /// Can the component be focused by mouse input?
        /// </summary>
        public bool CanFocusByMouseInput { get { return focusableByMouseInput && VisibleWorld && ActiveWorld; } }
        
        /// <summary>
        /// Can the component be focused input other than mouse?
        /// </summary>
        public bool CanFocusByOtherInput { get { return focusableByOtherInput && VisibleWorld && ActiveWorld; } }
        
        /// <summary>
        /// Can the component be focused by means other than user input?
        /// </summary>
        public bool CanFocusByNonInput { get { return focusableByNonInput && VisibleWorld && ActiveWorld; } }
        
        /// <summary>
        /// Has the component been focused by mouse input?
        /// </summary>
        public bool FocusedByMouseInput { get { return _focusedByMouseInput; } }
        
        /// <summary>
        /// Has the component been focused by input other than mouse?
        /// </summary>
        public bool FocusedByOtherInput { get { return _focusedByOtherInput; } }
        
        /// <summary>
        /// Has the component been focused by means other than user input
        /// </summary>
        public bool FocusedByNonInput { get { return _focusedByNonInput; } }
        
        /// <summary>
        /// Is the component focused in any manner?
        /// </summary>
        public bool Focused { get { return FocusedByMouseInput || FocusedByOtherInput || FocusedByNonInput; } }

        /// <summary>
        /// Is the component focusable by mouse input?
        /// </summary>
        public bool FocusableByMouseInput
        {
            get { return focusableByMouseInput; }
            set { focusableByMouseInput = value; }
        }

        /// <summary>
        /// Is the component focusable by input other than mouse?
        /// </summary>
        public bool FocusableByOtherInput
        {
            get { return focusableByOtherInput; }
            set { focusableByOtherInput = value; }
        }

        /// <summary>
        /// Is the component focusable by means other than user input?
        /// </summary>
        public bool FocusableByNonInput
        {
            get { return focusableByNonInput; }
            set { focusableByNonInput = value; }
        }

        /// <summary>
        /// Is the mouse hovering over this component?
        /// </summary>
        public bool Picked
        {
            get
            {
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                return BoundsWorld.Contains(guiSystem.AppMousePosition);
            }
        }

        /// <summary>
        /// The ID.
        /// </summary>
        public int GuiID
        {
            get { return guiID; }
            set { guiID = value; }
        }

        /// <summary>
        /// Raised on any gui event that cascades down the gui hierarchy.
        /// </summary>
        public event GuiHierarchyAction GuiHierarchyAction;

        /// <summary>
        /// Raised on any non-cascading gui event.
        /// </summary>
        public event GuiAction GuiAction;

        /// <summary>
        /// Raised on a keyboard event.
        /// </summary>
        public event KeyAction KeyAction;

        /// <summary>
        /// Raised when the mouse is over the component and its position is updated.
        /// </summary>
        public event MousePositionUpdate MousePositionUpdate;

        /// <summary>
        /// Raised on a mouse button event.
        /// </summary>
        public event MouseButtonAction MouseButtonAction;

        /// <summary>
        /// Raised on a mouse wheel event.
        /// </summary>
        public event MouseWheelAction MouseWheelAction;

        /// <summary>
        /// Raised on a game pad state event.
        /// </summary>
        public event GamePadStateUpdate GamePadStateUpdate;

        /// <summary>
        /// Raised on a directional gui event.
        /// </summary>
        public event DirectionAction DirectionAction;

        /// <summary>
        /// Raised on an abstract gui event.
        /// </summary>
        public event AbstractAction AbstractAction;

        /// <summary>
        /// Update the component. Must be called once per game cycle.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            UpdateHook(gameTime);
        }

        /// <summary>
        /// Draw the component.
        /// </summary>
        public void Draw(GameTime gameTime, Camera camera)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);

            cachedComponents.Add(this);
            CollectChildren(CollectionAlgorithm.Descending, cachedComponents);
            {
                CollectDrawingSprites(cachedComponents, cachedSprites);
                {
                    DrawSprites(gameTime, camera, cachedSprites);
                }
                cachedSprites.Clear();
            }
            cachedComponents.Clear();
        }

        /// <summary>
        /// Collect all the component's sprites.
        /// </summary>
        public IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result)
        {
            return view.CollectQuickSprites(result);
        }
        
        /// <summary>
        /// Returns true if the input is absorbed by the component, meaning that the container
        /// should not process the event on itself. An example would be a list component. The drop
        /// down component needs to use DirectionAction for its own purposes. So if it is in focus,
        /// the user should be able to press a direction without switching controls in the
        /// container.
        /// Another example is a text box. The user should be able to press directions, tabs,
        /// enter, etc. without the container processing these events by switching controls,
        /// closing the dialog, etc.
        /// </summary>
        public bool SinkDirectionEvent(InputType inputType, Direction2D direction)
        {
            return SinkDirectionEventHook(inputType, direction);
        }

        /// <summary>
        /// Refer to explanation for SinkDirectionEvent.
        /// </summary>
        public bool SinkAbstractEvent(InputType inputType, AbstractEventType eventType)
        {
            return SinkAbstractEventHook(inputType, eventType);
        }

        /// <summary>
        /// Automatically assign GuiIDs for all of the children.
        /// </summary>
        public void AutoAssignIDsForChildren()
        {
            CollectChildren<BaseGuiComponent>(CollectionAlgorithm.Shallow, cachedChildren);
            {
                for (int i = 0; i < cachedChildren.Count; ++i) cachedChildren[i].GuiID = i;
            }
            cachedChildren.Clear();
        }

        /// <summary>
        /// Focus the component by mouse input.
        /// </summary>
        public void FocusByMouseInput()
        {
            if (!AllowFocusByMouseInput) return;
            if (_focusedByMouseInput) return;
            if (!Focused)
            {
                Engine.GetService<GuiSystem>().Screen.Defocus();
                CascadeSetFocusedComponent();
            }
            _focusedByMouseInput = true;
            RaiseGuiEvent(GuiEventType.FocusedByMouseInput);
        }

        /// <summary>
        /// Focus the component by input other than the mouse.
        /// </summary>
        public void FocusByOtherInput()
        {
            if (!AllowFocusByOtherInput) return;
            if (_focusedByOtherInput) return;
            if (!Focused)
            {
                Engine.GetService<GuiSystem>().Screen.Defocus();
                CascadeSetFocusedComponent();
                MouseToComponent();
            }
            _focusedByOtherInput = true;
            RaiseGuiEvent(GuiEventType.FocusedByOtherInput);
        }

        /// <summary>
        /// Focus the component by means other than user input.
        /// </summary>
        public void FocusByNonInput()
        {
            if (!AllowFocusByNonInput) return;
            if (_focusedByNonInput) return;
            if (!Focused)
            {
                Engine.GetService<GuiSystem>().Screen.Defocus();
                CascadeSetFocusedComponent();
            }
            _focusedByNonInput = true;
            RaiseGuiEvent(GuiEventType.FocusedByNonInput);
        }

        /// <summary>
        /// Remove focus from the component.
        /// </summary>
        public void Defocus()
        {
            if (_focusedByMouseInput || _focusedByOtherInput || _focusedByNonInput)
            {
                _focusedByMouseInput = false;
                _focusedByOtherInput = false;
                _focusedByNonInput = false;
                CascadeClearFocusedComponent();
                RaiseGuiEvent(GuiEventType.Defocused);
            }
            else if (focusedComponent != null) focusedComponent.Defocus();
        }

        /// <summary>
        /// Raise a general gui event.
        /// </summary>
        public void RaiseGuiEvent(GuiEventType e)
        {
            if (GuiAction != null) GuiAction(this, e);
        }

        /// <summary>
        /// Raise a hierarchical gui event.
        /// </summary>
        public void RaiseGuiHierarchyEvent(GuiHierarchyEventType e, bool world)
        {
            if (GuiHierarchyAction != null) GuiHierarchyAction(this, e, world);
        }

        /// <summary>
        /// Raise a keyboard key event.
        /// </summary>
        public void RaiseKeyEvent(InputType inputType, Keys key)
        {
            if (KeyAction != null) KeyAction(this, inputType, key);
        }

        /// <summary>
        /// Raise a mouse position update event.
        /// </summary>
        public void RaiseMousePositionUpdate(Vector2 position)
        {
            if (MousePositionUpdate != null) MousePositionUpdate(this, position);
        }

        /// <summary>
        /// Raise a mouse button event.
        /// </summary>
        public void RaiseMouseButtonEvent(InputType inputType, MouseButton button, Vector2 mousePosition)
        {
            if (MouseButtonAction != null) MouseButtonAction(this, inputType, button, mousePosition);
        }

        /// <summary>
        /// Raise a mouse wheel event.
        /// </summary>
        public void RaiseMouseWheelEvent(int wheelDelta, Vector2 mousePosition)
        {
            if (MouseWheelAction != null) MouseWheelAction(this, wheelDelta, mousePosition);
        }

        /// <summary>
        /// Raise a game pad event.
        /// </summary>
        public void RaiseGamePadStateUpdate(int playerIndex, ref GamePadState state)
        {
            if (GamePadStateUpdate != null) GamePadStateUpdate(this, playerIndex, ref state);
        }

        /// <summary>
        /// Raise a direction event.
        /// </summary>
        public void RaiseDirectionEvent(InputType inputType, Direction2D direction)
        {
            if (DirectionAction != null) DirectionAction(this, inputType, direction);
        }

        /// <summary>
        /// Raise an non-device-specific input event.
        /// </summary>
        public void RaiseAbstractEvent(InputType inputType, AbstractEventType eventType)
        {
            if (AbstractAction != null) AbstractAction(this, inputType, eventType);
        }

        /// <summary>
        /// Raise a picked changed event.
        /// </summary>
        public void RaisePickedChangedEvent()
        {
            if (GuiAction != null) GuiAction(this, GuiEventType.PickedChanged);
        }

        /// <summary>
        /// The view strategy.
        /// </summary>
        protected GuiView View { get { return view; } }

        /// <summary>
        /// Handle updating the component.
        /// </summary>
        protected virtual void UpdateHook(GameTime gameTime)
        {
            UpdateChildren(gameTime);
            view.Picked = Picked;
            view.Update(gameTime);
        }

        /// <summary>
        /// Implement the sink direction event algorithm.
        /// </summary>
        protected virtual bool SinkDirectionEventHook(InputType inputType, Direction2D direction)
        {
            return false;
        }

        /// <summary>
        /// Implement the sink abstract event algorithm.
        /// </summary>
        protected virtual bool SinkAbstractEventHook(InputType inputType, AbstractEventType eventType)
        {
            return false;
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new BaseGuiComponentToken();
        }

        /// <inheritdoc />
        protected override bool IsValidChildHook(OxComponent child)
        {
            return child is BaseGuiComponent;
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertyHook(string property)
        {
            base.UpdateWorldPropertyHook(property);
            switch (property)
            {
                case "Active": UpdateActiveWorld(); break;
                case "Bounds": UpdateBoundsWorld(); break;
                case "Position": UpdatePositionWorld(); break;
                case "Visible": UpdateVisibleWorld(); break;
                case "Z": UpdateZWorld(); break;
            }
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertiesHook()
        {
            base.UpdateWorldPropertiesHook();
            UpdateActiveWorld();
            UpdateBoundsWorld();
            UpdatePositionWorld();
            UpdateVisibleWorld();
            UpdateZWorld();
        }
        
        private bool AllowFocusByMouseInput
        {
            get
            {
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                return guiSystem.Screen != null && GetRoot() == guiSystem.Screen && CanFocusByMouseInput;
            }
        }
        
        private bool AllowFocusByOtherInput
        {
            get
            {
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                return guiSystem.Screen != null && GetRoot() == guiSystem.Screen && CanFocusByOtherInput;
            }
        }
        
        private bool AllowFocusByNonInput
        {
            get
            {
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                return guiSystem.Screen != null && GetRoot() == guiSystem.Screen && CanFocusByNonInput;
            }
        }

        private void this_ChildAdded(OxComponent sender, OxComponent child)
        {
            OxHelper.ArgumentNullCheck(sender, child);
            if (OxHelper.Cast<BaseGuiComponent>(child).Focused) throw new InvalidOperationException(
                "Cannot add a focused component to a hierarchy. " +
                "This might cause multiple components in the hierarchy to be focused.");
        }

        private void this_ChildRemoved(OxComponent sender, OxComponent child)
        {
            OxHelper.ArgumentNullCheck(sender, child);
            OxHelper.Cast<BaseGuiComponent>(child).Defocus();
        }

        private void this_GuiAction(BaseGuiComponent sender, GuiEventType e)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (e == GuiEventType.FocusedByMouseInput ||
                e == GuiEventType.FocusedByOtherInput ||
                e == GuiEventType.FocusedByNonInput ||
                e == GuiEventType.Defocused)
                view.Focused = Focused;
        }

        private void this_GuiHierarchyAction(BaseGuiComponent sender, GuiHierarchyEventType e, bool world)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (e == GuiHierarchyEventType.ActiveChanged ||
                e == GuiHierarchyEventType.VisibleChanged)
                EnsureCorrectFocus();
        }

        private void this_KeyAction(BaseGuiComponent sender, InputType type, Keys key)
        {
            OxHelper.ArgumentNullCheck(sender);
            BaseGuiComponent focusedComponent = FocusedComponent; // OPTIMIZATION: cache property
            if (focusedComponent != null) focusedComponent.RaiseKeyEvent(type, key);
        }

        private void this_MousePositionUpdate(BaseGuiComponent sender, Vector2 position)
        {
            OxHelper.ArgumentNullCheck(sender);
            BaseGuiComponent focusedComponent = FocusedComponent; // OPTIMIZATION: cache property
            if (focusedComponent != null) focusedComponent.RaiseMousePositionUpdate(position);
        }

        private void this_GamePadStateUpdate(BaseGuiComponent sender, int playerIndex, ref GamePadState state)
        {
            OxHelper.ArgumentNullCheck(sender);
            BaseGuiComponent focusedComponent = FocusedComponent; // OPTIMIZATION: cache property
            if (focusedComponent != null) focusedComponent.RaiseGamePadStateUpdate(playerIndex, ref state);
        }

        private void this_MouseButtonAction(BaseGuiComponent sender, InputType type, MouseButton button, Vector2 mousePos)
        {
            OxHelper.ArgumentNullCheck(sender);
            bool passOffEvent = false;
            if (type != InputType.ClickDown) passOffEvent = true;
            else
            {
                BaseGuiComponent oldTarget = GetFocusedLeaf();
                BaseGuiComponent newTarget = GetPickedLeaf();
                if (oldTarget == newTarget) passOffEvent = true;
                else if (CanChangeContainersByMouseInput(oldTarget, newTarget))
                {
                    newTarget.FocusByMouseInput();
                    if (newTarget.Focused) passOffEvent = true;
                }
            }
            if (passOffEvent)
            {
                BaseGuiComponent focusedComponent = FocusedComponent; // OPTIMIZATION: cache property
                if (focusedComponent != null) focusedComponent.RaiseMouseButtonEvent(type, button, mousePos);
            }
        }

        private void this_MouseWheelAction(BaseGuiComponent sender, int wheelDelta, Vector2 mousePosition)
        {
            OxHelper.ArgumentNullCheck(sender);
            BaseGuiComponent focusedComponent = FocusedComponent; // OPTIMIZATION: cache property
            if (focusedComponent != null) focusedComponent.RaiseMouseWheelEvent(wheelDelta, mousePosition);
        }

        private void this_DirectionAction(BaseGuiComponent sender, InputType type, Direction2D direction)
        {
            OxHelper.ArgumentNullCheck(sender);
            BaseGuiComponent focusedComponent = FocusedComponent; // OPTIMIZATION: cache property
            bool sendToFocusedComponent = false;
            if (focusedComponent != null && focusedComponent.SinkDirectionEvent(type, direction)) sendToFocusedComponent = true;
            if (sendToFocusedComponent) focusedComponent.RaiseDirectionEvent(type, direction);
            else
            {
                if (type == InputType.Repeat || type == InputType.ClickDown)
                {
                    BaseGuiComponent target = GetTargetFocusableComponent(direction);
                    if (target != null) target.FocusByOtherInput();
                }
            }
        }

        private void this_AbstractAction(BaseGuiComponent sender, InputType type, AbstractEventType eventType)
        {
            OxHelper.ArgumentNullCheck(sender);
            BaseGuiComponent focusedComponent = FocusedComponent; // OPTIMIZATION: cache property
            bool sendToFocusedComponent = false;
            if (focusedComponent != null && focusedComponent.SinkAbstractEvent(type, eventType)) sendToFocusedComponent = true;
            if (sendToFocusedComponent) focusedComponent.RaiseAbstractEvent(type, eventType);
            else
            {
                if (type == InputType.ClickDown || type == InputType.Repeat)
                {
                    if (eventType == AbstractEventType.Tab)
                    {
                        BaseGuiComponent target = GetTargetFocusableComponent(Direction2D.Right);
                        if (target != null) target.FocusByOtherInput();
                    }
                    if (eventType == AbstractEventType.ShiftTab)
                    {
                        BaseGuiComponent target = GetTargetFocusableComponent(Direction2D.Left);
                        if (target != null) target.FocusByOtherInput();
                    }
                }
            }
        }

        private GuiView CreateView(Type viewType)
        {
            GuiViewFactory viewFactory = Engine.GetService<GuiViewFactory>();
            GuiView view = viewFactory.Create(this, viewType);
            view.Active = ActiveWorld;
            view.Z = ZWorld;
            view.Position = PositionWorld;
            view.Scale = Scale;
            view.Bounds = BoundsWorld;
            view.Visible = VisibleWorld;
            view.Focused = Focused;
            AddGarbage(view);
            return view;
        }

        private void DrawSprites(GameTime gameTime, Camera camera, IList<QuickSprite> sprites)
        {
            QuickSpriteDrawer spriteDrawer = Engine.GetService<QuickSpriteDrawer>();
            spriteDrawer.DrawSprites(gameTime, camera, sprites);
        }

        private void UpdateChildren(GameTime gameTime)
        {
            CollectChildren<BaseGuiComponent>(CollectionAlgorithm.Shallow, cachedChildren);
            {
                for (int i = 0; i < cachedChildren.Count; ++i) cachedChildren[i].Update(gameTime);
            }
            cachedChildren.Clear();
        }

        private void UpdateActiveWorld()
        {
            view.Active = _activeWorld = CalculateActiveWorld();
            UpdateWorldPropertyOfChildren("Active");
            RaiseActiveChangedEvent(true);
        }

        private void UpdateBoundsWorld()
        {
            view.Bounds = _boundsWorld = CalculateBoundsWorld();
            UpdateWorldPropertyOfChildren("Bounds");
            RaiseBoundsChangedEvent(true);
        }

        private void UpdatePositionWorld()
        {
            view.Position = _positionWorld = CalculatePositionWorld();
            UpdateWorldPropertyOfChildren("Position");
            UpdateBoundsWorld();
            RaisePositionChangedEvent(true);
        }

        private void UpdateScale()
        {
            view.Scale = _scale;
            UpdateBoundsWorld();
        }

        private void UpdateVisibleWorld()
        {
            view.Visible = _visibleWorld = CalculateVisibleWorld();
            UpdateWorldPropertyOfChildren("Visible");
            RaiseVisibleChangedEvent(true);
        }

        private void UpdateZWorld()
        {
            view.Z = _zWorld = CalculateZWorld();
            UpdateWorldPropertyOfChildren("Z");
            RaiseZChangedEvent(true);
        }

        private bool CalculateActiveWorld()
        {
            BaseGuiComponent parent = GetParent<BaseGuiComponent>();
            if (parent == null) return Active;
            return Active & parent.ActiveWorld;
        }

        private Rect CalculateBoundsWorld()
        {
            BaseGuiComponent parent = GetParent<BaseGuiComponent>();
            if (parent == null) return Bounds;
            return RectWorld.Union(parent.BoundsWorld);
        }

        private Vector2 CalculatePositionWorld()
        {
            BaseGuiComponent parent = GetParent<BaseGuiComponent>();
            if (parent == null) return Position;
            return Position + parent.PositionWorld;
        }

        private bool CalculateVisibleWorld()
        {
            BaseGuiComponent parent = GetParent<BaseGuiComponent>();
            if (parent == null) return Visible;
            return Visible & parent.VisibleWorld;
        }

        private float CalculateZWorld()
        {
            BaseGuiComponent parent = GetParent<BaseGuiComponent>();
            if (parent == null) return Z;
            return Z + parent.ZWorld;
        }

        private void EnsureCorrectFocus()
        {
            if (!ActiveWorld || !VisibleWorld) Defocus();
        }

        private void RaisePositionChangedEvent(bool world)
        {
            if (GuiHierarchyAction != null) GuiHierarchyAction(this, GuiHierarchyEventType.PositionChanged, world);
        }

        private void RaiseScaleChangedEvent()
        {
            if (GuiAction != null) GuiAction(this, GuiEventType.ScaleChanged);
        }

        private void RaiseBoundsChangedEvent(bool world)
        {
            if (GuiHierarchyAction != null) GuiHierarchyAction(this, GuiHierarchyEventType.BoundsChanged, world);
        }

        private void RaiseZChangedEvent(bool world)
        {
            if (GuiHierarchyAction != null) GuiHierarchyAction(this, GuiHierarchyEventType.ZChanged, world);
        }

        private void RaiseActiveChangedEvent(bool world)
        {
            if (GuiHierarchyAction != null) GuiHierarchyAction(this, GuiHierarchyEventType.ActiveChanged, world);
        }

        private void RaiseVisibleChangedEvent(bool world)
        {
            if (GuiHierarchyAction != null) GuiHierarchyAction(this, GuiHierarchyEventType.VisibleChanged, world);
        }

        /// <summary>May return null.</summary>
        private BaseGuiComponent GetTargetFocusableComponent(Direction2D direction)
        {
            BaseGuiComponent result;

            CollectChildren(CollectionAlgorithm.Shallow, cachedSortedChildren);
            {
                switch (direction)
                {
                    case Direction2D.Up:
                        cachedSortedChildren.Sort(horizontalComparer);
                        cachedSortedChildren.Reverse();
                        result = GetNextFocusableByOtherInputComponent(FocusedComponent, cachedSortedChildren);
                        break;
                    case Direction2D.Down:
                        cachedSortedChildren.Sort(horizontalComparer);
                        result = GetNextFocusableByOtherInputComponent(FocusedComponent, cachedSortedChildren);
                        break;
                    case Direction2D.Left:
                        cachedSortedChildren.Sort(verticalPosComparer);
                        cachedSortedChildren.Reverse();
                        result = GetNextFocusableByOtherInputComponent(FocusedComponent, cachedSortedChildren);
                        break;
                    case Direction2D.Right:
                        cachedSortedChildren.Sort(verticalPosComparer);
                        result = GetNextFocusableByOtherInputComponent(FocusedComponent, cachedSortedChildren);
                        break;
                    default:
                        result = null;
                        break;
                }
            }
            cachedSortedChildren.Clear();

            return result;
        }

        private bool CanChangeContainersByMouseInput(BaseGuiComponent oldTarget, BaseGuiComponent newTarget)
        {
            if (newTarget == this) return true;
            BaseGuiComponent oldTargetNearestModalContainer = null;
            BaseGuiComponent newTargetNearestModalContainer = null;
            oldTargetNearestModalContainer = FindNearestModalContainer(oldTarget);
            newTargetNearestModalContainer = FindNearestModalContainer(newTarget);
            return
                oldTargetNearestModalContainer == null ||
                oldTargetNearestModalContainer == this ||
                oldTargetNearestModalContainer == newTargetNearestModalContainer;
        }

        private void CascadeClearFocusedComponent()
        {
            BaseGuiComponent componentWalker = GetParent<BaseGuiComponent>();
            while (componentWalker != null)
            {
                componentWalker.FocusedComponent = null;
                componentWalker = componentWalker.GetParent<BaseGuiComponent>();
            }
        }

        private void CascadeSetFocusedComponent()
        {
            BaseGuiComponent targetComponent = this;
            BaseGuiComponent containerWalker = FindParent<BaseGuiComponent>();
            while (containerWalker != null)
            {
                containerWalker.FocusedComponent = targetComponent;
                targetComponent = containerWalker;
                containerWalker = containerWalker.GetParent<BaseGuiComponent>();
            }
        }

        private BaseGuiComponent GetRoot()
        {
            BaseGuiComponent result = this;
            while (result.GetParent<BaseGuiComponent>() != null) result = result.GetParent<BaseGuiComponent>();
            return result;
        }

        private BaseGuiComponent GetFocusedLeaf()
        {
            BaseGuiComponent result = this;
            while (result.FocusedComponent != null) result = result.FocusedComponent;
            return result;
        }

        private BaseGuiComponent GetPickedLeaf()
        {
            BaseGuiComponent result = null;

            cachedComponents.Add(this);
            CollectChildren<BaseGuiComponent>(CollectionAlgorithm.Descending, pickedAndCanFocusByMouseInputPredicate, cachedComponents);
            {
                result = FindNearest(cachedComponents);
            }
            cachedComponents.Clear();

            return result;
        }

        private void MouseToComponent()
        {
            GuiSystem guiSystem = Engine.GetService<GuiSystem>();
            if (!guiSystem.MouseSnap) return;
            // NOTE: since there is inaccuracy when translating from application space to OS
            // space, the outgoing position of the mouse must be offset.
            guiSystem.AppMousePosition = PositionWorld + new Vector2(2);
        }

        /// <summary>May return null.</summary>
        private static BaseGuiComponent GetNextFocusableByOtherInputComponent(
            BaseGuiComponent focusedComponent, List<BaseGuiComponent> children)
        {
            BaseGuiComponent result;
            if (focusedComponent != null && !children.Contains(focusedComponent))
            {
                System.Diagnostics.Trace.Fail("Children should contain focusedComponent.");
                result = null;
            }
            else
            {
                CollectFocusableByOtherInputComponents(children, cachedFocusableComponents);
                {
                    if (cachedFocusableComponents.Count == 0) result = null;
                    else
                    {
                        if (focusedComponent == null || !focusedComponent.FocusableByOtherInput)
                        {
                            result = cachedFocusableComponents[0];
                        }
                        else
                        {
                            int index = cachedFocusableComponents.IndexOf(focusedComponent);
                            if (index + 1 < cachedFocusableComponents.Count)
                            {
                                result = cachedFocusableComponents[index + 1];
                            }
                            else
                            {
                                result = cachedFocusableComponents[0];
                            }
                        }
                    }
                }
                cachedFocusableComponents.Clear();
            }
            return result;
        }

        private static IList<BaseGuiComponent> CollectFocusableByOtherInputComponents(
            IList<BaseGuiComponent> components, IList<BaseGuiComponent> result)
        {
            for (int i = 0; i < components.Count; ++i)
            {
                BaseGuiComponent component = components[i];
                if (component.CanFocusByOtherInput) result.Add(component);
            }
            return result;
        }

        /// <summary>May return null.</summary>
        private static BaseGuiComponent FindNearestModalContainer(BaseGuiComponent target)
        {
            if (target.Modal) return target;
            return target.FindParent<BaseGuiComponent>(modalPredicate);
        }

        /// <summary>May return null.</summary>
        private BaseGuiComponent FindNearest(IList<BaseGuiComponent> components)
        {
            BaseGuiComponent result = null;
            for (int i = 0; i < components.Count; ++i)
            {
                BaseGuiComponent child = components[i];
                if (result == null) result = child;
                else if (result.ZWorld < child.ZWorld) result = child;
            }
            return result;
        }

        private static IList<QuickSprite> CollectDrawingSprites(
            IList<BaseGuiComponent> components, List<QuickSprite> result)
        {
            for (int i = 0; i < components.Count; ++i)
            {
                BaseGuiComponent component = components[i];
                if (component.VisibleWorld && // OPTIMIZATION
                    component.BoundsWorld.Intersects(OxConfiguration.VirtualScreen)) // OPTIMIZATION
                    component.CollectQuickSprites(result);
            }
            return result;
        }

        private static readonly Func<BaseGuiComponent, bool> pickedAndCanFocusByMouseInputPredicate = x => x.Picked && x.CanFocusByMouseInput;
        private static readonly Func<BaseGuiComponent, bool> modalPredicate = x => x.Modal;
        private static readonly List<BaseGuiComponent> cachedFocusableComponents = new List<BaseGuiComponent>();
        private static readonly HorizontalComparer2D horizontalComparer = new HorizontalComparer2D();
        private static readonly VerticalComparer2D verticalPosComparer = new VerticalComparer2D();

        private readonly List<BaseGuiComponent> cachedComponents = new List<BaseGuiComponent>();
        private readonly List<BaseGuiComponent> cachedChildren = new List<BaseGuiComponent>();
        private readonly List<BaseGuiComponent> cachedSortedChildren = new List<BaseGuiComponent>();
        private readonly List<QuickSprite> cachedSprites = new List<QuickSprite>();
        private readonly GuiView view;
        /// <summary>May be null.</summary>
        private BaseGuiComponent focusedComponent;
        private bool focusableByMouseInput = true;
        private bool focusableByOtherInput = true;
        private bool focusableByNonInput = true;
        private bool modal;
        private int guiID;
        private Vector2 _position;
        private Vector2 _positionWorld;
        private Vector2 _scale = new Vector2(240, 60);
        private Rect _boundsWorld = new Rect(0, 0, 240, 60);
        private float _z = GuiConfiguration.ZGap;
        private float _zWorld = GuiConfiguration.ZGap;
        private bool _active = true;
        private bool _activeWorld = true;
        private bool _visible = true;
        private bool _visibleWorld = true;
        private bool _focusedByMouseInput;
        private bool _focusedByOtherInput;
        private bool _focusedByNonInput;
    }
}

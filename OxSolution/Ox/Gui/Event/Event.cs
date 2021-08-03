using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ox.Engine.MathNamespace;
using Ox.Gui.Component;

namespace Ox.Gui.Event
{
    /// <summary>
    /// Represents the type of an input event.
    /// </summary>
    public enum InputType
    {
        ClickDown = 0,
        Down,
        ClickUp,
        //Up, // OPTIMIZATION: can't events every frame for every key / button that is up.
        Repeat
    }

    /// <summary>
    /// Represents the type of a mouse button.
    /// </summary>
    public enum MouseButton
    {
        Left = 0,
        Middle,
        Right,
        X1,
        X2,
        Count
    }

    /// <summary>
    /// Represents the type of a game pad button.
    /// </summary>
    public enum GamePadButton
    {
        A = 0,
        B,
        X,
        Y,
        LeftShoulder,
        RightShoulder,
        LeftStick,
        RightStick,
        Back,
        Start
    }

    /// <summary>
    /// Represents the type of a non-device-specific input button.
    /// </summary>
    public enum AbstractButtonType
    {
        TabButton = 0,
        ShiftTabButton,
        AffirmButton,
        CancelButton,
        NextPageButton,
        PreviousPageButton,
        Count
    }

    /// <summary>
    /// Represents the type of a non-device-specific input event.
    /// </summary>
    public enum AbstractEventType
    {
        Tab = 0,
        ShiftTab,
        Affirm,
        Cancel,
        NextPage,
        PreviousPage
    }

    /// <summary>
    /// Represents the type of an independent gui component event.
    /// </summary>
    public enum GuiEventType
    {
        ScaleChanged = 0,
        PickedChanged,
        ColorChanged,
        FocusedByMouseInput,
        FocusedByOtherInput,
        FocusedByNonInput,
        Defocused
    }

    /// <summary>
    /// Represents the type of an gui component event that cascades down the gui hierarchy.
    /// </summary>
    public enum GuiHierarchyEventType
    {
        PositionChanged = 0,
        BoundsChanged,
        ZChanged,
        ActiveChanged,
        VisibleChanged
    }

    /// <summary>
    /// Raised on any gui event that cascades down the gui hierarchy.
    /// </summary>
    public delegate void GuiHierarchyAction(BaseGuiComponent sender, GuiHierarchyEventType e, bool world);
    /// <summary>
    /// Raised on any independant gui event.
    /// </summary>
    public delegate void GuiAction(BaseGuiComponent sender, GuiEventType e);
    /// <summary>
    /// Raised on a keyboard event.
    /// </summary>
    public delegate void KeyAction(BaseGuiComponent sender, InputType inputType, Keys key);
    /// <summary>
    /// Raised when the mouse is over the component and its position is updated.
    /// </summary>
    public delegate void MousePositionUpdate(BaseGuiComponent sender, Vector2 position);
    /// <summary>
    /// Raised on a mouse button event.
    /// </summary>
    public delegate void MouseButtonAction(BaseGuiComponent sender, InputType inputType, MouseButton button, Vector2 mousePosition);
    /// <summary>
    /// Raised on a mouse wheel event.
    /// </summary>
    public delegate void MouseWheelAction(BaseGuiComponent sender, int wheelDelta, Vector2 mousePosition);
    /// <summary>
    /// Raised on a game pad state event.
    /// </summary>
    public delegate void GamePadStateUpdate(BaseGuiComponent sender, int playerIndex, ref GamePadState state);
    /// <summary>
    /// Raised on a directional gui event.
    /// </summary>
    public delegate void DirectionAction(BaseGuiComponent sender, InputType inputType, Direction2D direction);
    /// <summary>
    /// Raised on an abstract gui event.
    /// </summary>
    public delegate void AbstractAction(BaseGuiComponent sender, InputType inputType, AbstractEventType eventType);
    /// <summary>
    /// Raised when a child component is added to a gui component.
    /// </summary>
    public delegate void ChildAdded(BaseGuiComponent sender, BaseGuiComponent child);
    /// <summary>
    /// Raised when a child component is removed from a gui component.
    /// </summary>
    public delegate void ChildRemoved(BaseGuiComponent sender, BaseGuiComponent child);
}

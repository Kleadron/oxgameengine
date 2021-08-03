using Microsoft.Xna.Framework.Input;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui.Event;

namespace Ox.Gui.Input
{
    /// <summary>
    /// Polls gui input from raw input states.
    /// </summary>
    public class InputRouter
    {
        /// <summary>
        /// The [0..1] threshold at which an input event is registered.
        /// </summary>
        public float InputThreshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

#if !XBOX360
        /// <summary>
        /// Get the state of a mouse button.
        /// </summary>
        public ButtonState GetMouseButtonState(MouseButton button, ref MouseState mouseState)
        {
            switch (button)
            {
                case MouseButton.Left: return mouseState.LeftButton;
                case MouseButton.Middle: return mouseState.MiddleButton;
                case MouseButton.Right: return mouseState.RightButton;
                case MouseButton.X1: return mouseState.XButton1;
                case MouseButton.X2: return mouseState.XButton2;
                default: return ButtonState.Released;
            }
        }
#endif

        /// <summary>
        /// Get the state of a game pad button.
        /// </summary>
        public ButtonState GetGamePadButtonState(GamePadButton button, ref GamePadButtons gamePadButtons)
        {
            switch (button)
            {
                case GamePadButton.A: return gamePadButtons.A;
                case GamePadButton.B: return gamePadButtons.B;
                case GamePadButton.X: return gamePadButtons.X;
                case GamePadButton.Y: return gamePadButtons.Y;
                case GamePadButton.LeftShoulder: return gamePadButtons.LeftShoulder;
                case GamePadButton.RightShoulder: return gamePadButtons.RightShoulder;
                case GamePadButton.LeftStick: return gamePadButtons.LeftStick;
                case GamePadButton.RightStick: return gamePadButtons.RightStick;
                case GamePadButton.Start: return gamePadButtons.Start;
                case GamePadButton.Back: return gamePadButtons.Back;
                default: return ButtonState.Released;
            }
        }

        /// <summary>
        /// Get the state of the specfied directional input.
        /// </summary>
        public ButtonState GetInputDirectionState(
            Direction2D direction, ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            switch (direction)
            {
                case Direction2D.Up: return GetDirectionUpState(ref keyboardState, ref gamePadState);
                case Direction2D.Down: return GetDirectionDownState(ref keyboardState, ref gamePadState);
                case Direction2D.Left: return GetDirectionLeftState(ref keyboardState, ref gamePadState);
                case Direction2D.Right: return GetDirectionRightState(ref keyboardState, ref gamePadState);
                default: return ButtonState.Released;
            }
        }

        /// <summary>
        /// Get the state of an abstract input.
        /// </summary>
        public ButtonState GetAbstractButtonState(
            AbstractButtonType type, ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            switch (type)
            {
                case AbstractButtonType.AffirmButton: return GetAffirmState(ref keyboardState, ref gamePadState);
                case AbstractButtonType.CancelButton: return GetCancelState(ref keyboardState, ref gamePadState);
                case AbstractButtonType.NextPageButton: return GetNextPageState(ref keyboardState, ref gamePadState);
                case AbstractButtonType.PreviousPageButton: return GetPreviousPageState(ref keyboardState, ref gamePadState);
                case AbstractButtonType.ShiftTabButton: return GetShiftTabState(ref keyboardState);
                case AbstractButtonType.TabButton: return GetTabState(ref keyboardState);
                default: return ButtonState.Released;
            }
        }

        private ButtonState GetTabState(ref KeyboardState keyboardState)
        {
            return ToButtonState(keyboardState.IsKeyDown(Keys.Tab));
        }

        private ButtonState GetShiftTabState(ref KeyboardState keyboardState)
        {
            return ToButtonState(
                keyboardState.GetShiftState() &&
                keyboardState.IsKeyDown(Keys.Tab));
        }

        private ButtonState GetAffirmState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.Buttons.A == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Enter));
        }

        private ButtonState GetCancelState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.Buttons.B == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape));
        }

        private ButtonState GetNextPageState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.Buttons.RightShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.PageDown));
        }

        private ButtonState GetPreviousPageState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.Buttons.LeftShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.PageUp));
        }

        private ButtonState GetDirectionRightState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.DPad.Right == ButtonState.Pressed ||
                InPositiveThreshold(gamePadState.ThumbSticks.Left.X) ||
                InPositiveThreshold(gamePadState.ThumbSticks.Right.X) ||
                keyboardState.IsKeyDown(Keys.Right));
        }

        private ButtonState GetDirectionLeftState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.DPad.Left == ButtonState.Pressed ||
                InNegativeThreshold(gamePadState.ThumbSticks.Left.X) ||
                InNegativeThreshold(gamePadState.ThumbSticks.Right.X) ||
                keyboardState.IsKeyDown(Keys.Left));
        }

        private ButtonState GetDirectionDownState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.DPad.Down == ButtonState.Pressed ||
                InNegativeThreshold(gamePadState.ThumbSticks.Left.Y) ||
                InNegativeThreshold(gamePadState.ThumbSticks.Right.Y) ||
                keyboardState.IsKeyDown(Keys.Down));
        }

        private ButtonState GetDirectionUpState(ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            return ToButtonState(
                gamePadState.DPad.Up == ButtonState.Pressed ||
                InPositiveThreshold(gamePadState.ThumbSticks.Left.Y) ||
                InPositiveThreshold(gamePadState.ThumbSticks.Right.Y) ||
                keyboardState.IsKeyDown(Keys.Up));
        }

        private ButtonState ToButtonState(bool value)
        {
            return value ? ButtonState.Pressed : ButtonState.Released;
        }

        private bool InPositiveThreshold(float value)
        {
            return value >= threshold;
        }

        private bool InNegativeThreshold(float value)
        {
            return value <= -threshold;
        }

        private float threshold = 0.5f;
    }
}

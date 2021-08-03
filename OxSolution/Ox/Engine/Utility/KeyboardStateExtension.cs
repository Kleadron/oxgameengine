using Microsoft.Xna.Framework.Input;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// The static of the input modifiers on the keyboard.
    /// </summary>
    public enum KeyboardModifier
    {
        None = 0,
        Shift,
        Control,
        ControlShift
    }

    /// <summary>
    /// An extension class for KeyboardState.
    /// </summary>
    public static class KeyboardStateExtension
    {
        public static KeyboardModifier GetModifier(this KeyboardState keyboard)
        {
            bool controlKeyState, shiftKeyState;
            keyboard.GetModifierStates(out controlKeyState, out shiftKeyState);
            if (controlKeyState & shiftKeyState) return KeyboardModifier.ControlShift;
            if (controlKeyState) return KeyboardModifier.Control;
            if (shiftKeyState) return KeyboardModifier.Shift;
            return KeyboardModifier.None;
        }

        public static void GetModifierStates(this KeyboardState keyboard, out bool controlState, out bool shiftState)
        {
            controlState = keyboard.GetControlState();
            shiftState = keyboard.GetShiftState();
        }

        public static bool GetShiftState(this KeyboardState keyboard)
        {
            return keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
        }

        public static bool GetControlState(this KeyboardState keyboard)
        {
            return keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
        }
    }
}

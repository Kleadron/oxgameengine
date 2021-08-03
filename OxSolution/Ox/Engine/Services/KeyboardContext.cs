using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// Tracks the state of the system keyboard and calculates the character value of a key during
    /// that state.
    /// </summary>
    public class KeyboardContext
    {
        /// <summary>
        /// Create a KeyboardContext.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public KeyboardContext(OxEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Is caps-lock active?
        /// </summary>
        public bool CapsLockActive
        {
            get { return capsLockActive; }
            set { capsLockActive = value; }
        }

        /// <summary>
        /// Is one or both of the shift keys pressed?
        /// </summary>
        public bool ShiftKeyActive
        {
            get { return rightShiftKeyDown || leftShiftKeyDown; }
        }

        /// <summary>
        /// Update the state of the keyboard. Must be called once per game cycle.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            KeyboardState keyboardState;
            engine.GetKeyboardState(out keyboardState);
            rightShiftKeyDown = keyboardState.IsKeyDown(Keys.RightShift);
            leftShiftKeyDown = keyboardState.IsKeyDown(Keys.LeftShift);
            if (!capsLockKeyDown && keyboardState.IsKeyDown(Keys.CapsLock))
            {
                capsLockKeyDown = true;
                capsLockActive = !capsLockActive;
            }
            else if(capsLockKeyDown && keyboardState.IsKeyUp(Keys.CapsLock))
            {
                capsLockKeyDown = false;
            }
        }

        /// <summary>
        /// Convert an XNA Key to a char according to the current keyboard context.
        /// </summary>
        /// <param name="key">The XNA Key to convert.</param>
        /// <param name="character">The resulting character.</param>
        /// <returns>Returns true if a valid char has been found.</returns>
        public bool KeyToChar(Keys key, out char character)
        {
            character = '!';

            if (key >= Keys.A && key <= Keys.Z)
            {
                character = AlphaToChar(key, character);
                return true;
            }

            if (key >= Keys.D0 && key <= Keys.D9)
            {
                character = NumberToChar(key, character);
                return true;
            }

            return OtherToChar(key, out character);
        }

        private char AlphaToChar(Keys key, char character)
        {
            const char caseOffset = (char)32;
            if (CapsLockActive)
            {
                if (ShiftKeyActive) character = (char)(key + caseOffset);
                else character = (char)key;
            }
            else
            {
                if (ShiftKeyActive) character = (char)key;
                else character = (char)(key + caseOffset);
            }
            return character;
        }

        private char NumberToChar(Keys key, char character)
        {
            if (!ShiftKeyActive) return (char)key;
            switch (key)
            {
                case Keys.D0: return (char)41;
                case Keys.D1: return (char)33;
                case Keys.D2: return (char)64;
                case Keys.D3: return (char)35;
                case Keys.D4: return (char)36;
                case Keys.D5: return (char)37;
                case Keys.D6: return (char)94;
                case Keys.D7: return (char)38;
                case Keys.D8: return (char)42;
                case Keys.D9: return (char)40;
                default: return character;
            }
        }

        private static bool OtherToChar(Keys key, out char character)
        {
            bool result = true;
            character = '\0';
            switch (key)
            {
                case Keys.Space: character = ' '; break;
                case Keys.OemMinus: character = '-'; break;
                case Keys.OemPlus: character = '+'; break;
                case Keys.OemBackslash: character = '\b'; break;
                case Keys.OemSemicolon: character = ';'; break;
                case Keys.OemQuotes: character = '"'; break;
                case Keys.OemComma: character = ','; break;
                case Keys.OemPeriod: character = '.'; break;
                case Keys.OemQuestion: character = '?'; break;
                case Keys.OemTilde: character = '~'; break;
                case Keys.OemOpenBrackets: character = '['; break;
                case Keys.OemCloseBrackets: character = ']'; break;
                default: result = false; break;
            }
            return result;
        }

        private readonly OxEngine engine;
        private bool rightShiftKeyDown;
        private bool leftShiftKeyDown;
        private bool capsLockKeyDown;
        private bool capsLockActive;
    }
}

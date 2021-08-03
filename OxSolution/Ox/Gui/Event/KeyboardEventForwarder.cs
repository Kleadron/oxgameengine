using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.Component;
using Ox.Gui.Input;

namespace Ox.Gui.Event
{
    public class KeyboardEventForwarder : OxUpdateable
    {
        public KeyboardEventForwarder(OxEngine engine, RepeatRate repeatRate)
        {
            this.engine = engine;
            this.repeatRate = repeatRate;
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            UpdateKeyboardInput(gameTime);
        }

        private void UpdateKeyboardInput(GameTime gameTime)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            KeyboardState keyboardState;
            engine.GetKeyboardState(out keyboardState);
            for (int i = 0; i < OxConfiguration.KeyCount; ++i)
                UpdateSpecificKeyInput(gameTime, ref keyboardState, (Keys)i);
        }

        private void UpdateSpecificKeyInput(
            GameTime gameTime, ref KeyboardState keyboardState, Keys key)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            if (keyboardState.IsKeyDown(key))
            {
                if (isKeyPressed[(int)key] == ButtonState.Released)
                {
                    isKeyPressed[(int)key] = ButtonState.Pressed;
                    screen.RaiseKeyEvent(InputType.ClickDown, key);
                }
                KeyDownRepeat(gameTime, key);
                screen.RaiseKeyEvent(InputType.Down, key);
            }
            else
            {
                if (isKeyPressed[(int)key] == ButtonState.Pressed)
                {
                    isKeyPressed[(int)key] = ButtonState.Released;
                    KeyUp(key);
                    screen.RaiseKeyEvent(InputType.ClickUp, key);
                }
                // OPTIMIZATION: this functionality is not worth the speed cost
                //KeyUpRepeat(gameTime, key);
                //screen.RaiseKeyEvent(InputType.Up, key);
            }
        }

        private void KeyDownRepeat(GameTime gameTime, Keys key)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            keyPressedElapsedTime[(int)key] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyPressedElapsedTime[(int)key] <= repeatRate.FirstDelay) return;
            keyPressedElapsedTime2[(int)key] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyPressedElapsedTime2[(int)key] <= repeatRate.Delay) return;
            keyPressedElapsedTime2[(int)key] = 0;
            screen.RaiseKeyEvent(InputType.Repeat, key);
        }

        private void KeyUp(Keys key)
        {
            keyPressedElapsedTime[(int)key] = 0;
            keyPressedElapsedTime2[(int)key] = 0;
        }

        private readonly ButtonState[] isKeyPressed = new ButtonState[OxConfiguration.KeyCount];
        private readonly float[] keyPressedElapsedTime = new float[OxConfiguration.KeyCount];
        private readonly float[] keyPressedElapsedTime2 = new float[OxConfiguration.KeyCount];
        private readonly RepeatRate repeatRate;
        private readonly OxEngine engine;
    }
}

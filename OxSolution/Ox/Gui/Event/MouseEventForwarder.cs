using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.Component;
using Ox.Gui.Input;

namespace Ox.Gui.Event
{
    public class MouseEventForwarder : OxUpdateable
    {
        public MouseEventForwarder(OxEngine engine, InputRouter inputRouter)
        {
            OxHelper.ArgumentNullCheck(engine, inputRouter);
            this.engine = engine;
            this.inputRouter = inputRouter;
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
#if !XBOX360
            UpdateMouseInput();
#endif
        }

#if !XBOX360
        private void UpdateMouseInput()
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            MouseState mouseState;
            engine.GetMouseState(out mouseState);
            Vector2 mousePosition = engine.GetService<GuiSystem>().AppMousePosition;
            for (int i = 0; i < (int)MouseButton.Count; ++i)
            {
                ButtonState mouseButtonState = inputRouter.GetMouseButtonState((MouseButton)i, ref mouseState);
                UpdateSpecificButtonInput(mousePosition, (MouseButton)i, mouseButtonState);
            }
            UpdateScrollWheelInput(ref mouseState, mousePosition);
        }

        private void UpdateSpecificButtonInput(Vector2 mousePosition, MouseButton button, ButtonState mouseButtonState)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            if (mouseButtonState == ButtonState.Pressed)
            {
                if (isMouseButtonPressed[(int)button] == ButtonState.Released)
                {
                    isMouseButtonPressed[(int)button] = ButtonState.Pressed;
                    screen.RaiseMouseButtonEvent(InputType.ClickDown, button, mousePosition);
                }
                screen.RaiseMouseButtonEvent(InputType.Down, button, mousePosition);
            }
            else
            {
                if (isMouseButtonPressed[(int)button] == ButtonState.Pressed)
                {
                    isMouseButtonPressed[(int)button] = ButtonState.Released;
                    screen.RaiseMouseButtonEvent(InputType.ClickUp, button, mousePosition);
                }
                // OPTIMIZATION: this functionality is not worth the speed cost
                //screen.RaiseMouseButtonEvent(InputType.Up, button, mousePosition);
            }
        }

        private void UpdateScrollWheelInput(ref MouseState mouseState, Vector2 mousePosition)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            int currentScrollWheelValue = mouseState.ScrollWheelValue;
            int scrollWheelDelta = currentScrollWheelValue - scrollWheelValue;
            if (scrollWheelDelta == 0) return;
            scrollWheelValue = currentScrollWheelValue;
            screen.RaiseMouseWheelEvent(scrollWheelDelta, mousePosition);
        }
#endif
    
        private readonly ButtonState[] isMouseButtonPressed = new ButtonState[(int)MouseButton.Count];
        private readonly InputRouter inputRouter;
        private readonly OxEngine engine;
#if !XBOX360
        private int scrollWheelValue;
#endif
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui.Component;
using Ox.Gui.Input;

namespace Ox.Gui.Event
{
    public class AbstractEventForwarder : OxUpdateable
    {
        public AbstractEventForwarder(OxEngine engine, InputRouter inputRouter, RepeatRate repeatRate)
        {
            this.engine = engine;
            this.inputRouter = inputRouter;
            this.repeatRate = repeatRate;
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            KeyboardState keyboardState = engine.KeyboardState;
            GamePadState gamePadState = engine.GamePadState;
            // the abstract input events -
            // Direction =       Keyboard.Arrows / GamePad.Directions
            // Tab =             Keyboard.Tab
            // ShiftTab =        Keyboard.ShiftTab
            // Affirm =          Keyboard.Enter / GamePad.A
            // Cancel =          Keyboard.Escape / GamePad.B
            // NextPage =        Keyboard.PgDn / GamePad.RightShoulder
            // PreviousPage =    Keyboard.PgUp / GamePad.LeftShoulder
            UpdateDirectionInput(gameTime, ref keyboardState, ref gamePadState);
            UpdateAbstractButtonsInput(gameTime, ref keyboardState, ref gamePadState);
        }

        private void UpdateDirectionInput(GameTime gameTime, ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            for (int i = 0; i < (int)Direction2D.Count; ++i)
            {
                ButtonState directionButtonState = inputRouter.GetInputDirectionState((Direction2D)i, ref keyboardState, ref gamePadState);
                UpdateSpecificDirectionInput(gameTime, (Direction2D)i, directionButtonState);
            }
        }

        private void UpdateSpecificDirectionInput(GameTime gameTime, Direction2D direction, ButtonState directionButtonState)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            if (directionButtonState == ButtonState.Pressed)
            {
                if (isDirectionPressed[(int)direction] != ButtonState.Pressed) screen.RaiseDirectionEvent(InputType.ClickDown, direction);
                DirectionDownRepeat(gameTime, direction);
                screen.RaiseDirectionEvent(InputType.Down, direction);
            }
            else if (directionButtonState != ButtonState.Pressed)
            {
                if (isDirectionPressed[(int)direction] == ButtonState.Pressed)
                {
                    DirectionUp(direction);
                    screen.RaiseDirectionEvent(InputType.ClickUp, direction);
                }
                // OPTIMIZATION: this functionality is not worth the speed cost
                //DirectionUpRepeat(gameTime, direction);
                //screen.RaiseDirectionEvent(InputType.Up, direction);
            }
            isDirectionPressed[(int)direction] = directionButtonState;
        }

        private void DirectionDownRepeat(GameTime gameTime, Direction2D direction)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            directionPressedElapsedTime[(int)direction] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (directionPressedElapsedTime[(int)direction] <= repeatRate.FirstDelay) return;
            directionPressedElapsedTime2[(int)direction] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (directionPressedElapsedTime2[(int)direction] <= repeatRate.Delay) return;
            directionPressedElapsedTime2[(int)direction] = 0;
            screen.RaiseDirectionEvent(InputType.Repeat, direction);
        }

        private void DirectionUp(Direction2D direction)
        {
            directionPressedElapsedTime[(int)direction] = 0;
            directionPressedElapsedTime2[(int)direction] = 0;
        }

        private void UpdateAbstractButtonsInput(GameTime gameTime, ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            UpdateAbstractButtonInput(gameTime, AbstractButtonType.AffirmButton, ref keyboardState, ref gamePadState);
            UpdateAbstractButtonInput(gameTime, AbstractButtonType.CancelButton, ref keyboardState, ref gamePadState);
            UpdateAbstractButtonInput(gameTime, AbstractButtonType.NextPageButton, ref keyboardState, ref gamePadState);
            UpdateAbstractButtonInput(gameTime, AbstractButtonType.PreviousPageButton, ref keyboardState, ref gamePadState);
            if (keyboardState.GetShiftState()) UpdateAbstractButtonInput(gameTime, AbstractButtonType.ShiftTabButton, ref keyboardState, ref gamePadState);
            else UpdateAbstractButtonInput(gameTime, AbstractButtonType.TabButton, ref keyboardState, ref gamePadState);
        }

        private void UpdateAbstractButtonInput(GameTime gameTime, AbstractButtonType type, ref KeyboardState keyboardState, ref GamePadState gamePadState)
        {
            ButtonState state = inputRouter.GetAbstractButtonState( type, ref keyboardState, ref gamePadState);
            if (state == ButtonState.Pressed)
            {
                if (isAbstractButtonPressed[(int)type] != ButtonState.Pressed) AbstractButtonInput(type, InputType.ClickDown);
                AbstractButtonDownRepeat(gameTime, type);
                AbstractButtonInput(type, InputType.Down);
            }
            else if (state != ButtonState.Pressed)
            {
                if (isAbstractButtonPressed[(int)type] == ButtonState.Pressed)
                {
                    AbstractButtonUp(type);
                    AbstractButtonInput(type, InputType.ClickUp);
                }
                // OPTIMIZATION: this functionality is not worth the speed cost
                //AbstractButtonUpRepeat(type);
                //AbstractButtonInput(type, InputType.Up);
            }
            isAbstractButtonPressed[(int)type] = state;
        }

        private void AbstractButtonInput(AbstractButtonType type, InputType inputType)
        {
            BaseGuiComponent screen = engine.GetService<GuiSystem>().Screen;
            if (screen == null) return;
            switch (type)
            {
                case AbstractButtonType.AffirmButton: screen.RaiseAbstractEvent(inputType, AbstractEventType.Affirm);break;
                case AbstractButtonType.CancelButton: screen.RaiseAbstractEvent(inputType, AbstractEventType.Cancel); break;
                case AbstractButtonType.NextPageButton: screen.RaiseAbstractEvent(inputType, AbstractEventType.NextPage); break;
                case AbstractButtonType.PreviousPageButton: screen.RaiseAbstractEvent(inputType, AbstractEventType.PreviousPage); break;
                case AbstractButtonType.ShiftTabButton: screen.RaiseAbstractEvent(inputType, AbstractEventType.ShiftTab); break;
                case AbstractButtonType.TabButton: screen.RaiseAbstractEvent(inputType, AbstractEventType.Tab); break;
            }
        }

        private void AbstractButtonDownRepeat(GameTime gameTime, AbstractButtonType type)
        {
            abstractButtonPressedElapsedTime[(int)type] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (abstractButtonPressedElapsedTime[(int)type] <= repeatRate.FirstDelay) return;
            abstractButtonPressedElapsedTime2[(int)type] += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (abstractButtonPressedElapsedTime2[(int)type] <= repeatRate.Delay) return;
            abstractButtonPressedElapsedTime2[(int)type] = 0;
            AbstractButtonInput(type, InputType.Repeat);
        }

        private void AbstractButtonUp(AbstractButtonType type)
        {
            abstractButtonPressedElapsedTime[(int)type] = 0;
            abstractButtonPressedElapsedTime2[(int)type] = 0;
        }

        private readonly ButtonState[] isAbstractButtonPressed = new ButtonState[(int)AbstractButtonType.Count];
        private readonly ButtonState[] isDirectionPressed = new ButtonState[(int)Direction2D.Count];
        private readonly float[] abstractButtonPressedElapsedTime = new float[(int)AbstractButtonType.Count];
        private readonly float[] abstractButtonPressedElapsedTime2 = new float[(int)AbstractButtonType.Count];
        private readonly float[] directionPressedElapsedTime = new float[(int)Direction2D.Count];
        private readonly float[] directionPressedElapsedTime2 = new float[(int)Direction2D.Count];
        private readonly InputRouter inputRouter;
        private readonly RepeatRate repeatRate;
        private readonly OxEngine engine;
    }
}

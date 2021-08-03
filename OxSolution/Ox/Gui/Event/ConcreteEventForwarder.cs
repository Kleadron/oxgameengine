using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Utility;
using Ox.Gui.Component;

namespace Ox.Gui.Event
{
    public class ConcreteEventForwarder : OxUpdateable
    {
        public ConcreteEventForwarder(OxEngine engine)
        {
            OxHelper.ArgumentNullCheck(engine);
            this.engine = engine;
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            GuiSystem guiSystem = engine.GetService<GuiSystem>();
            BaseGuiComponent screen = guiSystem.Screen;
            if (screen == null) return;
            Vector2 appMousePosition = guiSystem.AppMousePosition;
            screen.RaiseMousePositionUpdate(appMousePosition);
            for (int i = 0; i < OxConfiguration.PlayerMax; ++i)
            {
                GamePadState gamePadState = engine.GetGamePadState((PlayerIndex)i);
                if (gamePadState.IsConnected) screen.RaiseGamePadStateUpdate(i, ref gamePadState);
            }
        }

        private readonly OxEngine engine;
    }
}

using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace GameTemplate
{
    /// <summary>
    /// GameScript implements your Ox Game Engine game.
    /// </summary>
    public class GameScript : ComponentScript<OxComponent>
    {
        public GameScript(OxEngine engine, Transfer<OxComponent> component) : base(engine, component)
        {
            // TODO: Put your game's initialization code here
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: Put your game's disposal code here
            }
            base.Dispose(disposing);
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            // TODO: Put your game's update code here
        }
    }
}

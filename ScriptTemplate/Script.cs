using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace ScriptTemplate
{
    /// <summary>
    /// Implements a script that controls a component.
    /// </summary>
    public class Script
        // TODO: If your script needs to talk with its component through a more specific interface
        // than OxComponent, change ComponentsScript's generic type specifier.
        : ComponentScript<OxComponent>
    {
        public Script(OxEngine engine, Transfer<OxComponent> component)
            : base(engine, component)
        {
            // TODO: Put your script's initialization code here.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: Put your script's disposal code here.
            }
            base.Dispose(disposing);
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            // TODO: Put your script's update code here.
        }
    }
}

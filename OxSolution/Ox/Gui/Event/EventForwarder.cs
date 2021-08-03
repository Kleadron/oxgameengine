using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.Component;
using Ox.Gui.Input;

namespace Ox.Gui.Event
{
    /// <summary>
    /// Picks up input events and forwards them to a panel that represents the current gui screen.
    /// </summary>
    public class EventForwarder : OxUpdateable
    {
        /// <summary>
        /// Create an EventForwarder.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="repeatRate">The repeat rate.</param>
        public EventForwarder(OxEngine engine, RepeatRate repeatRate)
        {
            OxHelper.ArgumentNullCheck(engine, repeatRate);
            this.engine = engine;
            inputRouter = new InputRouter();
            concreteEventForwarder = new ConcreteEventForwarder(engine);
            keyboardEventForwarder = new KeyboardEventForwarder(engine, repeatRate);
            mouseEventForwarder = new MouseEventForwarder(engine, inputRouter);
            abstractEventForwarder = new AbstractEventForwarder(engine, inputRouter, repeatRate);
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            if (!engine.Game.IsActive) return;
            concreteEventForwarder.Update(gameTime);
            keyboardEventForwarder.Update(gameTime);
            mouseEventForwarder.Update(gameTime);
            abstractEventForwarder.Update(gameTime);
        }

        private readonly KeyboardEventForwarder keyboardEventForwarder;
        private readonly ConcreteEventForwarder concreteEventForwarder;
        private readonly AbstractEventForwarder abstractEventForwarder;
        private readonly MouseEventForwarder mouseEventForwarder;
        private readonly InputRouter inputRouter;
        private readonly OxEngine engine;
    }
}

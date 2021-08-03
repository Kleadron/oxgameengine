using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// An input portal from the gui to gameplay.
    /// </summary>
    public class Portal : GuiComponent<GuiView>
    {
        /// <summary>
        /// Create a Portal.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public Portal(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            FocusableByOtherInput = false;
        }

        /// <inheritdoc />
        protected override bool SinkDirectionEventHook(InputType inputType, Direction2D direction)
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool SinkAbstractEventHook(InputType inputType, AbstractEventType eventType)
        {
            return true;
        }
    }
}

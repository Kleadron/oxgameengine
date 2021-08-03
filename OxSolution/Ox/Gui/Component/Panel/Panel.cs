using Ox.Engine;
using Ox.Engine.Component;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical panel for a user interface.
    /// </summary>
    public class Panel : GuiComponent<PanelView>
    {
        /// <summary>
        /// Create a Panel.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public Panel(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            FocusableByMouseInput = false;
            FocusableByOtherInput = false;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                if (guiSystem.Screen == this) guiSystem.Screen = null;
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new PanelToken();
        }
    }
}

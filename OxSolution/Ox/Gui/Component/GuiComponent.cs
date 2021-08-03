using Ox.Engine;

namespace Ox.Gui.Component
{
    /// <summary>
    /// Augments BaseGuiComponent with a generically-typed reference to its gui view.
    /// </summary>
    public class GuiComponent<T> : BaseGuiComponent where T : GuiView
    {
        /// <summary>
        /// Create a GuiComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public GuiComponent(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain, typeof(T))
        {
            view = OxHelper.Cast<T>(base.View);
        }

        /// <inheritdoc />
        protected new T View { get { return view; } }

        private readonly T view;
    }
}

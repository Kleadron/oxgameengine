using Ox.Engine;
using Ox.Scene.Component;

namespace SceneEditorNamespace
{
    /// <summary>
    /// An axis triad that shows how the camera is oriented.
    /// </summary>
    public class AxisTriad : SingleSurfaceComponent<AxisTriadSurface>
    {
        /// <summary>
        /// Create an AxisTriad.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public AxisTriad(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            surface = new AxisTriadSurface(engine, this);
        }

        /// <inheritdoc />
        protected override AxisTriadSurface SurfaceHook { get { return surface; } }

        private readonly AxisTriadSurface surface;
    }
}

using Ox.Engine;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Draws the bounding boxes of all non-boundless items in a scene.
    /// </summary>
    public class BoundingBoxVisualizer : SingleSurfaceComponent<BoundingBoxVisualizerSurface>
    {
        /// <summary>
        /// Create a BoundingBoxVisualizer.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public BoundingBoxVisualizer(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            AddGarbage(surface = engine.GetService<SurfaceFactory>().CreateSurface<BoundingBoxVisualizerSurface>(this));
        }

        /// <inheritdoc />
        protected override BoundingBoxVisualizerSurface SurfaceHook { get { return surface; } }

        private readonly BoundingBoxVisualizerSurface surface;
    }
}

using Ox.Engine;
using Ox.Scene;
using Ox.Scene.Component;

namespace SceneEditorNamespace
{
    /// <summary>
    /// Visualizes scene selections with bounding boxes.
    /// </summary>
    public class SceneSelectionVisualizer : SingleSurfaceComponent<SceneSelectionVisualizerSurface>
    {
        /// <summary>
        /// Create a SceneSelectionVisualizer.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="document">The scene document.</param>
        public SceneSelectionVisualizer(OxEngine engine, string domainName, bool ownedByDomain, SceneDocument document)
            : base(engine, domainName, ownedByDomain)
        {
            AddGarbage(surface = new SceneSelectionVisualizerSurface(engine, this, document));
        }

        /// <inheritdoc />
        protected override SceneSelectionVisualizerSurface SurfaceHook { get { return surface; } }

        private readonly SceneSelectionVisualizerSurface surface;
    }
}

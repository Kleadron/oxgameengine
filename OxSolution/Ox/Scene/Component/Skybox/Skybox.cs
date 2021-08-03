using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.GeometryNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Various default values used by Skyboxes.
    /// </summary>
    public static class SkyboxDefaults
    {
        public const string EffectFileName = "Ox/Effects/oxSkybox";
        public const string DiffuseMapFileName = "Ox/Textures/skyCubeMap";
        public const DrawStyle DrawStyleDefault = DrawStyle.Prioritized;
        public const FaceMode FaceModeDefault = FaceMode.BackFaces;
    }

    /// <summary>
    /// A skybox that only works in perspective view.
    /// TODO: make this work in orthographic view.
    /// </summary>
    public class Skybox : SingleSurfaceComponent<SkyboxSurface>
    {
        /// <summary>
        /// Create a Skybox.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public Skybox(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            AddGarbage(surface = engine.GetService<SurfaceFactory>().CreateSurface<SkyboxSurface>(this));
        }

        /// <inheritdoc />
        protected override SkyboxSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new SkyboxToken();
        }

        private readonly SkyboxSurface surface;
    }
}

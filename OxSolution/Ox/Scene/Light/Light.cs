using Ox.Engine;
using Ox.Engine.Primitive;
using Ox.Scene.Component;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// A light that can be added to a scene.
    /// </summary>
    public abstract class Light : SceneComponent
    {
        /// <summary>
        /// Initialize a Light that is Boundless.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public Light(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain) { }

        /// <summary>
        /// Initialize a Light.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="primitive">The transformable primitive that describes the space occupied by the light.</param>
        public Light(OxEngine engine, string domainName, bool ownedByDomain, IPrimitive primitive)
            : base(engine, domainName, ownedByDomain, primitive) { }

        /// <summary>
        /// Initialize a Light.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="boxBuilder">Describes the space occupied by the light.</param>
        public Light(OxEngine engine, string domainName, bool ownedByDomain, IBoundingBoxBuilder boxBuilder)
            : base(engine, domainName, ownedByDomain, boxBuilder) { }
    }
}

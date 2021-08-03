using System;
using System.Collections.Generic;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Primitive;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A scene component with a single surface.
    /// </summary>
    /// <typeparam name="T">The type of surface.</typeparam>
    public abstract class SingleSurfaceComponent<T> : SceneComponent
        where T : BaseSurface
    {
        /// <summary>
        /// Create an SingleSurfaceComponent that is Boundless.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public SingleSurfaceComponent(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain) { }

        /// <summary>
        /// Create an SingleSurfaceComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="primitive">The transformable primitive that describes the space occupied by the component.</param>
        public SingleSurfaceComponent(OxEngine engine, string domainName, bool ownedByDomain, IPrimitive primitive)
            : base(engine, domainName, ownedByDomain, primitive) { }

        /// <summary>
        /// Create an SingleSurfaceComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="boxBuilder">Describes the space occupied by the component.</param>
        public SingleSurfaceComponent(OxEngine engine, string domainName, bool ownedByDomain, IBoundingBoxBuilder boxBuilder)
            : base(engine, domainName, ownedByDomain, boxBuilder) { }

        /// <summary>
        /// The surface.
        /// </summary>
        public T Surface { get { return SurfaceHook; } }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            throw new InvalidOperationException("SingleSurfaceComponent cannot create a token.");
        }

        protected abstract T SurfaceHook { get; }
    }
}

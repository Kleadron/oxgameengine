using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Primitive;
using Ox.Scene.Component;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// A point light in a scene.
    /// </summary>
    public class PointLight : Light
    {
        /// <summary>
        /// Create a PointLight that is Boundless.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public PointLight(OxEngine engine, string domainName, bool ownedByDomain)
            : this(engine, domainName, ownedByDomain, new BoxPrimitive(Vector3.Zero, new Vector3(0.5f))) { }

        /// <summary>
        /// Create a PointLight.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="primitive">The transformable primitive that describes the space occupied by the component.</param>
        public PointLight(OxEngine engine, string domainName, bool ownedByDomain, IPrimitive primitive)
            : base(engine, domainName, ownedByDomain, new PrimitiveBoundingBoxBuilder(primitive)) { }

        /// <summary>
        /// The diffuse color of the light.
        /// </summary>
        public Color DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        /// <summary>
        /// The range of the light.
        /// </summary>
        public float Range
        {
            get { return range; }
            set { range = value; }
        }

        /// <summary>
        /// The falloff of the light.
        /// </summary>
        public float Falloff
        {
            get { return falloff; }
            set { falloff = value; }
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new PointLightToken();
        }

        private Color diffuseColor = Color.Gray;
        private Color specularColor = Color.Gray;
        private float range = 64;
        private float falloff = 64;
    }
}

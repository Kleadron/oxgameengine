using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Scene.Component;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// An ambient light in a scene.
    /// </summary>
    public class AmbientLight : Light
    {
        /// <summary>
        /// Create an AmbientLight.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public AmbientLight(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain) { }

        /// <summary>
        /// The color of the light.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new AmbientLightToken();
        }
        
        private Color color = Color.Gray;
    }
}

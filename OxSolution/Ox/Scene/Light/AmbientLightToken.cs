using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Scene.Component;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// A serializable token for an AmbientLight.
    /// </summary>
    public class AmbientLightToken : SceneComponentToken
    {
        /// <summary>
        /// Create an AmbientLightToken.
        /// </summary>
        public AmbientLightToken()
        {
            color = new ProxyProperty<Color>(this, "Color", Color.Gray);
        }

        /// <summary>May be null.</summary>
        [XmlIgnore, Browsable(false)]
        public new AmbientLight Instance
        {
            get { return OxHelper.Cast<AmbientLight>(base.Instance); }
            set { base.Instance = value; }
        }
        
        public Color Color
        {
            get { return color.Value; }
            set { color.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new AmbientLight(engine, domainName, ownedByDomain);
        }
        
        private readonly ProxyProperty<Color> color;
    }
}

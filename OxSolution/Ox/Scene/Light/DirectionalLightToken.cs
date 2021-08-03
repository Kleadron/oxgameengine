using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Scene.Component;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// A serializable token for a DirectionalLight.
    /// </summary>
    public class DirectionalLightToken : SceneComponentToken
    {
        /// <summary>
        /// Create a DirectionalLightToken.
        /// </summary>
        public DirectionalLightToken()
        {
            direction = new ProxyProperty<Vector3>(this, "Direction", Vector3.Down);
            diffuseColor = new ProxyProperty<Color>(this, "DiffuseColor", Color.Gray);
            specularColor = new ProxyProperty<Color>(this, "SpecularColor", Color.Gray);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new DirectionalLight Instance
        {
            get { return OxHelper.Cast<DirectionalLight>(base.Instance); }
            set { base.Instance = value; }
        }

        public Vector3 Direction
        {
            get { return direction.Value; }
            set { direction.Value = value; }
        }
        
        public Color DiffuseColor
        {
            get { return diffuseColor.Value; }
            set { diffuseColor.Value = value; }
        }
        
        public Color SpecularColor
        {
            get { return specularColor.Value; }
            set { specularColor.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new DirectionalLight(engine, domainName, ownedByDomain, false);
        }

        private readonly ProxyProperty<Vector3> direction;
        private readonly ProxyProperty<Color> diffuseColor;
        private readonly ProxyProperty<Color> specularColor;
    }
}

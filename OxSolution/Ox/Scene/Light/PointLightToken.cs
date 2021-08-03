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
    /// A serializable token for a PointLight.
    /// </summary>
    public class PointLightToken : SceneComponentToken
    {
        /// <summary>
        /// Create a PointLightToken.
        /// </summary>
        public PointLightToken()
        {
            range = new ProxyProperty<float>(this, "Range", 64);
            falloff = new ProxyProperty<float>(this, "Falloff", 64);
            diffuseColor = new ProxyProperty<Color>(this, "DiffuseColor", Color.Gray);
            specularColor = new ProxyProperty<Color>(this, "SpecularColor", Color.Gray);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new PointLight Instance
        {
            get { return OxHelper.Cast<PointLight>(base.Instance); }
            set { base.Instance = value; }
        }

        [DefaultValue(64.0f)]
        public float Range
        {
            get { return range.Value; }
            set { range.Value = value; }
        }

        [DefaultValue(64.0f)]
        public float Falloff
        {
            get { return falloff.Value; }
            set { falloff.Value = value; }
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
            return new PointLight(engine, domainName, ownedByDomain);
        }

        private readonly ProxyProperty<float> range;
        private readonly ProxyProperty<float> falloff;
        private readonly ProxyProperty<Color> diffuseColor;
        private readonly ProxyProperty<Color> specularColor;
    }
}

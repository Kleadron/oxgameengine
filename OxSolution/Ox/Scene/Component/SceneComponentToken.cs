using System.ComponentModel;
using System.Xml.Serialization;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A serializable token for an SceneComponent.
    /// </summary>
    public class SceneComponentToken : TransformableComponentToken
    {
        /// <summary>
        /// Create a SceneComponentToken.
        /// </summary>
        public SceneComponentToken()
        {
            visible = new ProxyProperty<bool>(this, "Visible", true);
        }

        /// <summary>May be null.</summary>
        [XmlIgnore, Browsable(false)]
        public new SceneComponent Instance
        {
            get { return OxHelper.Cast<SceneComponent>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(true)]
        public bool Visible
        {
            get { return visible.Value; }
            set { visible.Value = value; }
        }

        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new SceneComponent(engine, domainName, ownedByDomain, BoxPrimitive);
        }

        private readonly ProxyProperty<bool> visible;
    }
}

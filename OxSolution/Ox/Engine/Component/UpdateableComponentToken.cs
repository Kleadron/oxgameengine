using System.ComponentModel;
using System.Xml.Serialization;
using Ox.Engine.DocumentNamespace;

namespace Ox.Engine.Component
{
    /// <summary>
    /// A serializable updateable component token.
    /// </summary>
    public class UpdateableComponentToken : ComponentToken
    {
        /// <summary>
        /// Create an UpdateableComponentToken.
        /// </summary>
        public UpdateableComponentToken()
        {
            enabled = new ProxyProperty<bool>(this, "Enabled", true);
            updateOrder = new ProxyProperty<int>(this, "UpdateOrder");
        }

        /// <summary>May be null.</summary>
        [XmlIgnore, Browsable(false)]
        public new UpdateableComponent Instance
        {
            get { return OxHelper.Cast<UpdateableComponent>(base.Instance); }
            set { base.Instance = value; }
        }

        [DefaultValue(true)]
        public bool Enabled
        {
            get { return enabled.Value; }
            set { enabled.Value = value; }
        }

        [DefaultValue(0)]
        public int UpdateOrder
        {
            get { return updateOrder.Value; }
            set { updateOrder.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new UpdateableComponent(engine, domainName, ownedByDomain);
        }

        private readonly ProxyProperty<bool> enabled;
        private readonly ProxyProperty<int> updateOrder;
    }
}

using System.ComponentModel;
using System.Xml.Serialization;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable token for a FillBar.
    /// </summary>
    public class FillBarToken : BaseGuiComponentToken
    {
        /// <summary>
        /// Create a FillBarToken.
        /// </summary>
        public FillBarToken()
        {
            fillMode = new ProxyProperty<Direction2D>(this, "FillMode", Direction2D.Right);
            fill = new ProxyProperty<float>(this, "Fill");
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new FillBar Instance
        {
            get { return OxHelper.Cast<FillBar>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(Direction2D.Right)]
        public Direction2D FillMode
        {
            get { return fillMode.Value; }
            set { fillMode.Value = value; }
        }
        
        [DefaultValue(0.0f)]
        public float Fill
        {
            get { return fill.Value; }
            set { fill.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new FillBar(engine, domainName, ownedByDomain);
        }

        private readonly ProxyProperty<Direction2D> fillMode;
        private readonly ProxyProperty<float> fill;
    }
}

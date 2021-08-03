using System.ComponentModel;
using System.Xml.Serialization;
using Ox.Engine;
using Ox.Engine.Component;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable token for a Panel.
    /// </summary>
    public class PanelToken : BaseGuiComponentToken
    {
        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new Panel Instance
        {
            get { return OxHelper.Cast<Panel>(base.Instance); }
            set { base.Instance = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new Panel(engine, domainName, ownedByDomain);
        }
    }
}

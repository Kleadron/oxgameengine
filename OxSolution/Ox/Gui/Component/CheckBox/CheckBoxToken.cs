using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable token for a CheckBox.
    /// </summary>
    public class CheckBoxToken : BaseGuiComponentToken
    {
        /// <summary>
        /// Create a CheckBoxToken.
        /// </summary>
        public CheckBoxToken()
        {
            isChecked = new ProxyProperty<bool>(this, "Checked");
            Scale = new Vector2(40);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new CheckBox Instance
        {
            get { return OxHelper.Cast<CheckBox>(base.Instance); }
            set { base.Instance = value; }
        }

        [DefaultValue(false)]
        public bool Checked
        {
            get { return isChecked.Value; }
            set { isChecked.Value = value; }
        }

        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new CheckBox(engine, domainName, ownedByDomain);
        }

        private readonly ProxyProperty<bool> isChecked;
    }
}

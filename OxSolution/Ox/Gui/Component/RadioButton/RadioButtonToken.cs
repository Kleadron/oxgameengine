using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable token for a RadioButton.
    /// </summary>
    public class RadioButtonToken : BaseGuiComponentToken
    {
        /// <summary>
        /// Create a RadioButtonToken.
        /// </summary>
        public RadioButtonToken()
        {
            radioButtonGroup = new ProxyProperty<string>(this, "RadioButtonGroup", string.Empty);
            Scale = new Vector2(40);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new RadioButton Instance
        {
            get { return OxHelper.Cast<RadioButton>(base.Instance); }
            set { base.Instance = value; }
        }

        [DefaultValue("")]
        public string RadioButtonGroup
        {
            get { return radioButtonGroup.Value; }
            set { radioButtonGroup.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new RadioButton(engine, domainName, ownedByDomain);
        }

        private readonly ProxyProperty<string> radioButtonGroup;
    }
}

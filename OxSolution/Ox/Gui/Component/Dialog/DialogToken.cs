using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable token for a Dialog.
    /// </summary>
    public class DialogToken : BaseGuiComponentToken
    {
        /// <summary>
        /// Create a DialogToken.
        /// </summary>
        public DialogToken()
        {
            textScale = new ProxyTraitProperty<Vector2>(this, "TextScale", new Vector2(16, 32));
            text = new ProxyProperty<string>(this, "Text", "Dialog");
            closeButtonActive = new ProxyProperty<bool>(this, "CloseButtonActive", true);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new Dialog Instance
        {
            get { return OxHelper.Cast<Dialog>(base.Instance); }
            set { base.Instance = value; }
        }
        
        public Vector2 TextScale
        {
            get { return textScale.Value; }
            set { textScale.Value = value; }
        }
        
        [DefaultValue("Dialog")]
        public string Text
        {
            get { return text.Value; }
            set { text.Value = value; }
        }
        
        [DefaultValue(true)]
        public bool CloseButtonActive
        {
            get { return closeButtonActive.Value; }
            set { closeButtonActive.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new Dialog(engine, domainName, ownedByDomain);
        }

        private readonly ProxyTraitProperty<Vector2> textScale;
        private readonly ProxyProperty<string> text;
        private readonly ProxyProperty<bool> closeButtonActive;
    }
}

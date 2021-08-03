using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable token for a Button.
    /// </summary>
    public class ButtonToken : BaseGuiComponentToken
    {
        /// <summary>
        /// Create a ButtonToken.
        /// </summary>
        public ButtonToken()
        {
            textJustification = new ProxyTraitProperty<Justification2D>(this, "TextJustification", Justification2D.Center);
            textScale = new ProxyTraitProperty<Vector2>(this, "TextScale", new Vector2(16, 32));
            textColor = new ProxyTraitProperty<Color>(this, "TextColor", Color.Black);
            textInactiveColor = new ProxyTraitProperty<Color>(this, "TextInactiveColor", Color.Gray);
            text = new ProxyProperty<string>(this, "Text", "Button");
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new Button Instance
        {
            get { return OxHelper.Cast<Button>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(Justification2D.Center)]
        public Justification2D TextJustification
        {
            get { return textJustification.Value; }
            set { textJustification.Value = value; }
        }
        
        public Vector2 TextScale
        {
            get { return textScale.Value; }
            set { textScale.Value = value; }
        }
        
        public Color TextColor
        {
            get { return textColor.Value; }
            set { textColor.Value = value; }
        }
        
        public Color TextInactiveColor
        {
            get { return textInactiveColor.Value; }
            set { textInactiveColor.Value = value; }
        }
        
        [DefaultValue("Button")]
        public string Text
        {
            get { return text.Value; }
            set { text.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new Button(engine, domainName, ownedByDomain);
        }

        private readonly ProxyTraitProperty<Justification2D> textJustification;
        private readonly ProxyTraitProperty<Vector2> textScale;
        private readonly ProxyTraitProperty<Color> textColor;
        private readonly ProxyTraitProperty<Color> textInactiveColor;
        private readonly ProxyProperty<string> text;
    }
}

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
    /// A serializable token for a Label.
    /// </summary>
    public class LabelToken : BaseGuiComponentToken
    {
        /// <summary>
        /// Create a LabelToken.
        /// </summary>
        public LabelToken()
        {
            textJustification = new ProxyTraitProperty<Justification2D>(this, "TextJustification", Justification2D.Left);
            textScale = new ProxyTraitProperty<Vector2>(this, "TextScale", new Vector2(16, 32));
            textColor = new ProxyTraitProperty<Color>(this, "TextColor", Color.Black);
            text = new ProxyProperty<string>(this, "Text", "Label");
            Scale = new Vector2(100, 40);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new Label Instance
        {
            get { return OxHelper.Cast<Label>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(Justification2D.Left)]
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
        
        [DefaultValue("Label")]
        public string Text
        {
            get { return text.Value; }
            set { text.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new Label(engine, domainName, ownedByDomain);
        }

        private readonly ProxyTraitProperty<Justification2D> textJustification;
        private readonly ProxyTraitProperty<Vector2> textScale;
        private readonly ProxyTraitProperty<Color> textColor;
        private readonly ProxyProperty<string> text;
    }
}

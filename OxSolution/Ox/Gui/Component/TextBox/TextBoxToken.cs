using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable token for a TextBox.
    /// </summary>
    public class TextBoxToken : BaseGuiComponentToken
    {
        /// <summary>
        /// Create a TextBoxToken.
        /// </summary>
        public TextBoxToken()
        {
            textScale = new ProxyTraitProperty<Vector2>(this, "TextScale", new Vector2(16, 32));
            textColor = new ProxyTraitProperty<Color>(this, "TextColor", Color.Black);
            textInactiveColor = new ProxyTraitProperty<Color>(this, "TextInactiveColor", Color.Gray);
            text = new ProxyProperty<string>(this, "Text");
            characterLimit = new ProxyProperty<int>(this, "CharacterLimit", 32);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new TextBox Instance
        {
            get { return OxHelper.Cast<TextBox>(base.Instance); }
            set { base.Instance = value; }
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
        
        [DefaultValue("")]
        public string Text
        {
            get { return text.Value; }
            set
            {
                value = value.Substring(0, Math.Min(value.Length, CharacterLimit));
                text.Value = value;
            }
        }
        
        [DefaultValue(32)]
        public int CharacterLimit
        {
            get { return characterLimit.Value; }
            set
            {
                value = Math.Max(0, value);
                Text = Text.Substring(0, Math.Min(Text.Length, value));
                characterLimit.Value = value;
            }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new TextBox(engine, domainName, ownedByDomain);
        }

        private readonly ProxyTraitProperty<Vector2> textScale;
        private readonly ProxyTraitProperty<Color> textColor;
        private readonly ProxyTraitProperty<Color> textInactiveColor;
        private readonly ProxyProperty<string> text;
        private readonly ProxyProperty<int> characterLimit;
    }
}

using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A serializable base Gui component token.
    /// </summary>
    public class BaseGuiComponentToken : ComponentToken
    {
        /// <summary>
        /// Create a BaseGuiComponentToken.
        /// </summary>
        public BaseGuiComponentToken()
        {
            position = new ProxyProperty<Vector2>(this, "Position");
            scale = new ProxyProperty<Vector2>(this, "Scale", new Vector2(240, 60));
            color = new ProxyProperty<Color>(this, "Color", Color.White);
            z = new ProxyProperty<float>(this, "Z", GuiConfiguration.ZGap);
            active = new ProxyProperty<bool>(this, "Active", true);
            visible = new ProxyProperty<bool>(this, "Visible", true);
            modal = new ProxyProperty<bool>(this, "Modal");
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new BaseGuiComponent Instance
        {
            get { return OxHelper.Cast<BaseGuiComponent>(base.Instance); }
            set { base.Instance = value; }
        }
        
        public Vector2 Position
        {
            get { return position.Value; }
            set { position.Value = value; }
        }
        
        public Vector2 Scale
        {
            get { return scale.Value; }
            set { scale.Value = value; }
        }
        
        public Color Color
        {
            get { return color.Value; }
            set { color.Value = value; }
        }
        
        [DefaultValue(GuiConfiguration.ZGap)]
        public float Z
        {
            get { return z.Value; }
            set { z.Value = value; }
        }
        
        [DefaultValue(true)]
        public bool Active
        {
            get { return active.Value; }
            set { active.Value = value; }
        }
        
        [DefaultValue(true)]
        public bool Visible
        {
            get { return visible.Value; }
            set { visible.Value = value; }
        }
        
        [DefaultValue(false)]
        public bool Modal
        {
            get { return modal.Value; }
            set { modal.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new BaseGuiComponent(engine, domainName, ownedByDomain, typeof(GuiView));
        }

        private readonly ProxyProperty<Vector2> position;
        private readonly ProxyProperty<Vector2> scale;
        private readonly ProxyProperty<float> z;
        private readonly ProxyProperty<bool> active;
        private readonly ProxyProperty<bool> visible;
        private readonly ProxyProperty<Color> color;
        private readonly ProxyProperty<bool> modal;
    }
}

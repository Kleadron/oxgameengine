using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A serializable token for Water.
    /// </summary>
    public class WaterToken : SingleSurfaceComponentToken<WaterSurface>
    {
        public WaterToken() : base(WaterDefaults.EffectFileName)
        {
            waveMap0Velocity = new ProxyMemberProperty<Vector2>(this, "Surface", "WaveMap0Velocity", WaterDefaults.WaveMap0Velocity);
            waveMap1Velocity = new ProxyMemberProperty<Vector2>(this, "Surface", "WaveMap1Velocity", WaterDefaults.WaveMap1Velocity);
            colorMultiplier = new ProxyMemberProperty<Color>(this, "Surface", "ColorMultiplier", WaterDefaults.ColorMultiplier);
            colorAdditive = new ProxyMemberProperty<Color>(this, "Surface", "ColorAdditive", WaterDefaults.ColorAdditive);
            waveMap0FileName = new ProxyMemberProperty<string>(this, "Surface", "WaveMap0FileName", WaterDefaults.WaveMap0FileName);
            waveMap1FileName = new ProxyMemberProperty<string>(this, "Surface", "WaveMap1FileName", WaterDefaults.WaveMap1FileName);
            waveLength = new ProxyMemberProperty<float>(this, "Surface", "WaveLength", WaterDefaults.WaveLength);
            waveHeight = new ProxyMemberProperty<float>(this, "Surface", "WaveHeight", WaterDefaults.WaveHeight);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new Water Instance
        {
            get { return OxHelper.Cast<Water>(base.Instance); }
            set { base.Instance = value; }
        }
        
        public Vector2 WaveMap0Velocity
        {
            get { return waveMap0Velocity.Value; }
            set { waveMap0Velocity.Value = value; }
        }
        
        public Vector2 WaveMap1Velocity
        {
            get { return waveMap1Velocity.Value; }
            set { waveMap1Velocity.Value = value; }
        }
        
        public Color ColorMultiplier
        {
            get { return colorMultiplier.Value; }
            set { colorMultiplier.Value = value; }
        }
        
        public Color ColorAdditive
        {
            get { return colorAdditive.Value; }
            set { colorAdditive.Value = value; }
        }
        
        [DefaultValue(WaterDefaults.WaveMap0FileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string WaveMap0FileName
        {
            get { return waveMap0FileName.Value; }
            set { waveMap0FileName.Value = value; }
        }
        
        [DefaultValue(WaterDefaults.WaveMap1FileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string WaveMap1FileName
        {
            get { return waveMap1FileName.Value; }
            set { waveMap1FileName.Value = value; }
        }
        
        [DefaultValue(WaterDefaults.WaveLength)]
        public float WaveLength
        {
            get { return waveLength.Value; }
            set { waveLength.Value = value; }
        }
        
        [DefaultValue(WaterDefaults.WaveHeight)]
        public float WaveHeight
        {
            get { return waveHeight.Value; }
            set { waveHeight.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new Water(engine, domainName, ownedByDomain);
        }
        
        private readonly ProxyMemberProperty<Vector2> waveMap0Velocity;
        private readonly ProxyMemberProperty<Vector2> waveMap1Velocity;
        private readonly ProxyMemberProperty<Color> colorMultiplier;
        private readonly ProxyMemberProperty<Color> colorAdditive;
        private readonly ProxyMemberProperty<string> waveMap0FileName;
        private readonly ProxyMemberProperty<string> waveMap1FileName;
        private readonly ProxyMemberProperty<float> waveLength;
        private readonly ProxyMemberProperty<float> waveHeight;
    }
}

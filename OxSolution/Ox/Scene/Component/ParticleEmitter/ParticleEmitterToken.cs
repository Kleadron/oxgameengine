using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A serializable token for a ParticleEmitter.
    /// </summary>
    public class ParticleEmitterToken : SingleSurfaceComponentToken<ParticleEmitterSurface>
    {
        /// <summary>
        /// Create a ParticleEmitterToken.
        /// </summary>
        public ParticleEmitterToken() : base(ParticleEmitterDefaults.EffectFileName)
        {
            diffuseMapFileName = new ProxyMemberProperty<string>(this, "Surface", "DiffuseMapFileName", ParticleEmitterDefaults.DiffuseMapFileName);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new ParticleEmitter Instance
        {
            get { return OxHelper.Cast<ParticleEmitter>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(ParticleEmitterDefaults.DiffuseMapFileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMapFileName
        {
            get { return diffuseMapFileName.Value; }
            set { diffuseMapFileName.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new ParticleEmitter(engine, domainName, ownedByDomain);
        }
        
        private readonly ProxyMemberProperty<string> diffuseMapFileName;
    }
}

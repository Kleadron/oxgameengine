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
    /// A serializable token for a Skybox.
    /// </summary>
    public class SkyboxToken : SingleSurfaceComponentToken<SkyboxSurface>
    {
        /// <summary>
        /// Create a SkyboxToken.
        /// </summary>
        public SkyboxToken() : base(SkyboxDefaults.EffectFileName)
        {
            diffuseMapFileName = new ProxyMemberProperty<string>(this, "Surface", "DiffuseMapFileName", SkyboxDefaults.DiffuseMapFileName);
            FaceMode = SkyboxDefaults.FaceModeDefault;
            DrawStyle = SkyboxDefaults.DrawStyleDefault;
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new Skybox Instance
        {
            get { return OxHelper.Cast<Skybox>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(SkyboxDefaults.DiffuseMapFileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMapFileName
        {
            get { return diffuseMapFileName.Value; }
            set { diffuseMapFileName.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new Skybox(engine, domainName, ownedByDomain);
        }

        private readonly ProxyMemberProperty<string> diffuseMapFileName;
    }
}

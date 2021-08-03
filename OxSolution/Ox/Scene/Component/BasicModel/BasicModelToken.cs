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
    /// A serializable token for a BasicModel.
    /// </summary>
    public class BasicModelToken : SceneComponentToken
    {
        /// <summary>
        /// Create a BasicModelToken.
        /// </summary>
        public BasicModelToken()
        {
            modelFileName = new ProxyProperty<string>(this, "ModelFileName", "Ox/Models/cube");
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new BasicModel Instance
        {
            get { return OxHelper.Cast<BasicModel>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue("Ox/Models/cube")]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string ModelFileName
        {
            get { return modelFileName.Value; }
            set { modelFileName.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new BasicModel(engine, domainName, ownedByDomain, BoxPrimitive);
        }

        private readonly ProxyProperty<string> modelFileName;
    }
}

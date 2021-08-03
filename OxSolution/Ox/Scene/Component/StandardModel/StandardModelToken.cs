using System.ComponentModel;
using System.Drawing.Design;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A serializable token for a StandardModel.
    /// </summary>
    public class StandardModelToken : SceneComponentToken
    {
        /// <summary>
        /// Create a StandardModelToken.
        /// </summary>
        public StandardModelToken()
        {
            modelFileName = new ProxyProperty<string>(this, "ModelFileName", "Ox/Models/cube");
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
            return new StandardModel(engine, domainName, ownedByDomain, BoxPrimitive);
        }
        
        private readonly ProxyProperty<string> modelFileName;
    }
}

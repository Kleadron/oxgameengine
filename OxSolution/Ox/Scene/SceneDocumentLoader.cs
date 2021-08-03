using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Scene
{
    /// <summary>
    /// Loads scene documents.
    /// </summary>
    public class SceneDocumentLoader : DocumentLoader
    {
        /// <inheritdoc />
        protected override Document CreateDocument(OxEngine engine)
        {
            return new SceneDocument(engine);
        }
    }
}

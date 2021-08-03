using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Gui.Component;

namespace Ox.Gui
{
    /// <summary>
    /// A gui document loader.
    /// </summary>
    public class GuiDocumentLoader : DocumentLoader
    {
        /// <inheritdoc />
        protected override Document CreateDocument(OxEngine engine)
        {
            return new GuiDocument(engine);
        }
    }
}

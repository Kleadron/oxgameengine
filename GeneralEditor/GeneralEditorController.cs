using Ox.Editor;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace GeneralEditorNamespace
{
    public class GeneralEditorController : GroupedEditorController
    {
        public GeneralEditorController(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain) { }

        protected new GeneralDocument Document
        {
            get { return OxHelper.Cast<GeneralDocument>(base.Document); }
        }

        protected override void SaveDocumentHook() { }

        protected override void LoadDocumentHook() { }

        protected override void NewDocumentHook() { }

        protected override Document CreateDocumentHook()
        {
            return new GeneralDocument(Engine);
        }

        protected override void SetUpComponentHook(ComponentToken component, ItemCreationStyle creationStyle) { }
    }
}

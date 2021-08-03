using Ox.Engine;
using Ox.Engine.DocumentNamespace;

namespace Ox.Editor
{
    public abstract class RootedEditorController : EditorController
    {
        public RootedEditorController(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            document = OxHelper.Cast<RootedDocument>(base.Document);
        }

        public ItemToken Root { get { return document.Root; } }

        protected new RootedDocument Document { get { return document; } }

        protected override DeleteResult CanDeleteItemHook(object item)
        {
            return DeleteResult.FailNotFound;
        }

        protected override void DeleteItemHook(object item) { }

        protected override object PasteItemHook(object item) { return null; }

        protected override object CloneItemHook(object item) { return null; }
    
        private readonly RootedDocument document;
    }
}

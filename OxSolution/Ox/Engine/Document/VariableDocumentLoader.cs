using System.Collections.Generic;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// Can load documents of a varying types.
    /// </summary>
    public class VariableDocumentLoader
    {
        /// <summary>
        /// Add a document loader for the specified type of document.
        /// </summary>
        public void AddLoader(string documentType, DocumentLoader loader)
        {
            loaders.Add(documentType, loader);
        }

        /// <summary>
        /// Remove a document loader.
        /// </summary>
        public bool RemoveLoader(string documentType)
        {
            return loaders.Remove(documentType);
        }

        /// <summary>
        /// Load a document.
        /// </summary>
        /// <param name="engine">The engine to which the document items are destined.</param>
        /// <param name="fileName">The name of the document file.</param>
        /// <param name="documentType">
        /// The type of document, for example; "General", "Scene", or "Gui". Case-sensitive.</param>
        /// <param name="domainName">The domain that will own the document items.</param>
        /// <exception cref="LoadDocumentException" />
        public void LoadDocument(OxEngine engine, string fileName, string documentType, string domainName)
        {
            DocumentLoader loader;
            if (loaders.TryGetValue(documentType, out loader)) loader.Load(engine, fileName, domainName);
        }

        private readonly Dictionary<string, DocumentLoader> loaders = new Dictionary<string, DocumentLoader>();
    }
}

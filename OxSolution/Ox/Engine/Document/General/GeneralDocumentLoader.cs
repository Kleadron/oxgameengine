namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// Loads general documents.
    /// </summary>
    public class GeneralDocumentLoader : DocumentLoader
    {
        /// <inheritdoc />
        protected override Document CreateDocument(OxEngine engine)
        {
            return new GeneralDocument(engine);
        }
    }
}

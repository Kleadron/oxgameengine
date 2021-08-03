using System;
using System.Collections.Generic;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// The token to which a general document will serialize.
    /// </summary>
    public class GeneralDocumentToken : GroupedDocumentToken
    {
        public GeneralDocumentToken() { }

        public GeneralDocumentToken(List<GroupToken> groups, List<ComponentToken> components)
            : base(groups, components) { }
    }

    /// <summary>
    /// A generalized document that holds groupable components.
    /// </summary>
    public class GeneralDocument : GroupedDocument
    {
        /// <summary>
        /// Create a GeneralDocument.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GeneralDocument(OxEngine engine) : base(engine) { }

        /// <inheritdoc />
        protected override ConstructionDictionary ConstructionDictionary
        {
            get { return OxConfiguration.GeneralConstructionDictionary; }
        }

        /// <inheritdoc />
        protected override Type DocumentTokenTypeHook
        {
            get { return typeof(GeneralDocumentToken); }
        }

        /// <inheritdoc />
        protected override DocumentToken CreateDocumentTokenHook()
        {
            return new GeneralDocumentToken(Groups, Components);
        }
    }
}

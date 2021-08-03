using System;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;
using Ox.Gui.Component;

namespace Ox.Gui
{
    /// <summary>
    /// A manipulable document composed of various GuiComponentTokens.
    /// </summary>
    public class GuiDocument : RootedDocument
    {
        /// <summary>
        /// Create a GuiDocument.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GuiDocument(OxEngine engine) : base(engine) { }

        /// <inheritdoc />
        protected override ConstructionDictionary ConstructionDictionary
        {
            get { return GuiConfiguration.GuiConstructionDictionary; }
        }

        /// <inheritdoc />
        protected override Type DocumentTokenTypeHook
        {
            get { return typeof(GuiDocumentToken); }
        }

        /// <inheritdoc />
        protected override bool VerifyComponentHook(ComponentToken component)
        {
            return component is BaseGuiComponentToken;
        }

        /// <inheritdoc />
        protected override DocumentToken CreateDocumentTokenHook()
        {
            return new GuiDocumentToken(Components);
        }
    }
}

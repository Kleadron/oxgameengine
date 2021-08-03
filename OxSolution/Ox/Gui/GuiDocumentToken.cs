using System.Collections.Generic;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Gui
{
    /// <summary>
    /// The token to which a gui document will serialize.
    /// </summary>
    public class GuiDocumentToken : DocumentToken
    {
        public GuiDocumentToken() { }

        public GuiDocumentToken(List<ComponentToken> components) : base(components) { }
    }
}

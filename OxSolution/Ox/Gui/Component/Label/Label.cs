using Ox.Engine;
using Ox.Engine.Component;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical label for a user interface.
    /// </summary>
    public class Label : GuiComponent<LabelView>
    {
        /// <summary>
        /// Create a Label.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public Label(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            View.Text = Text;
            FocusableByMouseInput = false;
            FocusableByOtherInput = false;
        }

        /// <summary>
        /// The text string for the label.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                View.Text = _text = value;
            }
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new LabelToken();
        }

        private string _text = string.Empty;
    }
}

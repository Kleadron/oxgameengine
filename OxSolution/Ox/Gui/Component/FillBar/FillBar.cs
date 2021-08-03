using System;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical fill bar for a user inteface. A fill bar often graphically represents the
    /// progress of an operation or the [0..1] amount of some value.</summary>
    public class FillBar : GuiComponent<FillBarView>
    {
        /// <summary>
        /// Create a FillBar.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public FillBar(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            View.Fill = Fill;
            View.FillMode = FillMode;
            FocusableByMouseInput = false;
            FocusableByOtherInput = false;
        }

        /// <summary>
        /// The direction from which the bar is filled.
        /// </summary>
        public Direction2D FillMode
        {
            get { return _fillMode; }
            set { View.FillMode = _fillMode = value; }
        }

        /// <summary>
        /// The [0..1] amount that the bar is filled.
        /// </summary>
        public float Fill
        {
            get { return _fill; }
            set
            {
                if (_fill == value) return;
                View.Fill = _fill = value;
                if (FillChanged != null) FillChanged(this);
            }
        }

        /// <summary>
        /// Raised when bar's fullness is changed.
        /// </summary>
        public event Action<FillBar> FillChanged;

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new FillBarToken();
        }

        private Direction2D _fillMode;
        private float _fill;
    }
}

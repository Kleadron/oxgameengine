using System;
using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Component
{
    /// <summary>
    /// An updateable subcomponent.
    /// </summary>
    public class BaseUpdateableSubcomponent : BaseSubcomponent, IOxUpdateable
    {
        /// <summary>
        /// Create an UpdateableSubcomponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The component to augment.</param>
        public BaseUpdateableSubcomponent(OxEngine engine, OxComponent component) :
            base(engine, component) { }

        /// <inheritdoc />
        public bool Enabled
        {
            get { return updateable.Enabled; }
            set { updateable.Enabled = value; }
        }

        /// <inheritdoc />
        public int UpdateOrder
        {
            get { return updateable.UpdateOrder; }
            set { updateable.UpdateOrder = value; }
        }

        /// <inheritdoc />
        public event Action<IOxUpdateable> EnabledChanged;

        /// <inheritdoc />
        public event Action<IOxUpdateable> UpdateOrderChanged;

        /// <inheritdoc />
        public void Update(GameTime gameTime)
        {
            UpdateHook(gameTime);
        }

        /// <summary>
        /// Handle updating the subcomponent.
        /// </summary>
        protected virtual void UpdateHook(GameTime gameTime) { }

        private void updateable_EnabledChanged(IOxUpdateable sender)
        {
            if (EnabledChanged != null) EnabledChanged(this);
        }

        private void updateable_UpdateOrderChanged(IOxUpdateable sender)
        {
            if (UpdateOrderChanged != null) UpdateOrderChanged(this);
        }

        private readonly IOxUpdateable updateable = new OxUpdateable();
    }
}

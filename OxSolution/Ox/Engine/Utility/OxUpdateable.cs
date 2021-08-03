using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// A vanilla implementation of IOxUpdateable.
    /// </summary>
    public class OxUpdateable : IOxUpdateable
    {
        /// <inheritdoc />
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;
                enabled = value;
                if (EnabledChanged != null) EnabledChanged(this);
            }
        }

        /// <inheritdoc />
        public int UpdateOrder
        {
            get { return updateOrder; }
            set
            {
                if (updateOrder == value) return;
                updateOrder = value;
                if (UpdateOrderChanged != null) UpdateOrderChanged(this);
            }
        }

        /// <inheritdoc />
        public event Action<IOxUpdateable> EnabledChanged;

        /// <inheritdoc />
        public event Action<IOxUpdateable> UpdateOrderChanged;

        /// <inheritdoc />
        public void Update(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            UpdateHook(gameTime);
        }

        /// <summary>
        /// Handle updating the object.
        /// </summary>
        protected virtual void UpdateHook(GameTime gameTime) { }

        private bool enabled = true;
        private int updateOrder;
    }
}

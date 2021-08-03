using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Compares the update order of updateables.
    /// </summary>
    public class UpdateOrderComparer<T> : IComparer<T>
        where T : class, IOxUpdateable
    {
        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            return OxMathHelper.Compare(x.UpdateOrder, y.UpdateOrder);
        }
    }

    /// <summary>
    /// Represents Ox's version of IUpdateable.
    /// </summary>
    public interface IOxUpdateable
    {
        /// <summary>
        /// Is this object eligible for updating?
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// The update order relative to other updateables.
        /// </summary>
        int UpdateOrder { get; set; }
        /// <summary>
        /// Raised when the Enabled property is changed.
        /// </summary>
        event Action<IOxUpdateable> EnabledChanged;
        /// <summary>
        /// Raised when the UpdateOrder property is changed.
        /// </summary>
        event Action<IOxUpdateable> UpdateOrderChanged;
        /// <summary>
        /// Update the object. Must be called once per frame if this object is enabled.
        /// </summary>
        void Update(GameTime gameTime);
    }
}

using System;
using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Component
{
    /// <summary>
    /// Augments BaseUpdateableSubcomponent with a generically-typed component reference.
    /// </summary>
    public class UpdateableSubcomponent<T> : BaseUpdateableSubcomponent where T : OxComponent
    {
        /// <summary>
        /// Create an UpdateableSubcomponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The component to augment.</param>
        public UpdateableSubcomponent(OxEngine engine, T component) : base(engine, component)
        {
            this.component = component;
        }

        /// <summary>
        /// The augmented component.
        /// </summary>
        protected new T Component { get { return component; } }

        private T component;
    }
}

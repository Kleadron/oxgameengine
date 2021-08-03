using System;
using Ox.Engine.Utility;

namespace Ox.Engine.Component
{
    /// <summary>
    /// Augments BaseSubcomponent with a generically-typed component reference.
    /// </summary>
    public class Subcomponent<T> : BaseSubcomponent where T : OxComponent
    {
        /// <summary>
        /// Create a Subcomponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The component to augment.</param>
        public Subcomponent(OxEngine engine, T component) : base(engine, component)
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

using Microsoft.Xna.Framework;

namespace Ox.Engine.Component
{
    /// <summary>
    /// Augments BaseUpdateableSubcomponent with a generically-typed component reference.
    /// </summary>
    public class TransformableSubcomponent<T> : BaseSubcomponent where T : TransformableComponent
    {
        /// <summary>
        /// Create a TransformableSubcomponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The component to augment.</param>
        public TransformableSubcomponent(OxEngine engine, T component) : base(engine, component)
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

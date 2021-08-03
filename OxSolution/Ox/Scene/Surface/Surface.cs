using Ox.Engine;
using Ox.Engine.Component;
using Ox.Scene.Component;

namespace Ox.Scene.SurfaceNamespace
{
    /// <summary>
    /// Augments BaseSurface with a generically-typed scene component reference.
    /// </summary>
    public abstract class Surface<T> : BaseSurface where T : SceneComponent
    {
        /// <summary>
        /// Initialize a BaseSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        /// <param name="effectFileName">See property EffectFileName.</param>
        public Surface(OxEngine engine, T component, string effectFileName)
            : base(engine, component, effectFileName)
        {
            this.component = component;
        }

        /// <inheritdoc />
        protected new T Component { get { return component; } }

        private T component;
    }
}

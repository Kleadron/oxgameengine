using Ox.Engine.MathNamespace;
using Ox.Engine.Primitive;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A box property that synchronizes a ProxyToken.
    /// </summary>
    public class ProxyBoxProperty : BaseProxyProperty<Box>
    {
        /// <summary>
        /// Create a ProxyBoxProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ProxyBoxProperty(ProxyToken token, string name, Box defaultValue)
            : base(token, name, defaultValue) { }

        /// <summary>
        /// Create a ProxyBoxProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyBoxProperty(ProxyToken token, string name)
            : base(token, name, default(Box)) { }

        /// <summary>
        /// The box primitive to synchronize.
        /// </summary>
        public BoxPrimitive Primitive { get { return box; } }

        /// <inheritdoc />
        protected override bool TryGetPropertyHook(out Box value)
        {
            value = default(Box);
            if (Instance == null) return false;
            value = box.Box;
            return true;
        }

        /// <inheritdoc />
        protected override bool TrySetPropertyHook(Box value)
        {
            if (Instance == null) return false;
            box.Box = value;
            return true;
        }

        private readonly BoxPrimitive box = new BoxPrimitive();
    }
}

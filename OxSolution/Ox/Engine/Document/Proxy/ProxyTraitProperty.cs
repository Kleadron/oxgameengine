using System;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A trait that synchronizes a ProxyToken.
    /// </summary>
    /// <typeparam name="T">The type of the trait.</typeparam>
    public class ProxyTraitProperty<T> : BaseProxyProperty<T>
    {
        /// <summary>
        /// Create a ProxyTrait.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ProxyTraitProperty(ProxyToken token, string name, T defaultValue)
            : base(token, name, defaultValue) { }

        /// <summary>
        /// Create a ProxyTrait.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyTraitProperty(ProxyToken token, string name)
            : base(token, name, default(T)) { }

        /// <inheritdoc />
        protected override bool TryGetPropertyHook(out T value)
        {
            value = default(T);
            if (Instance == null) return false;
            Type type = Instance.GetType();
            object propertyValue = type.GetProperty("Traits").GetValue(Instance, null);
            Traits traits = OxHelper.Cast<Traits>(propertyValue);
            return traits.TryGet<T>(Name, out value);
        }

        /// <inheritdoc />
        protected override bool TrySetPropertyHook(T value)
        {
            if (Instance == null) return false;
            Type type = Instance.GetType();
            object propertyValue = type.GetProperty("Traits").GetValue(Instance, null);
            Traits traits = OxHelper.Cast<Traits>(propertyValue);
            traits.Set<T>(Name, value);
            return true;
        }
    }
}

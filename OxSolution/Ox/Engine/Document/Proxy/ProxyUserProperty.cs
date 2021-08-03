using System;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A property that utilizes a delegate to synchronize a ProxyToken.
    /// </summary>
    /// <typeparam name="T">The type of the trait.</typeparam>
    /// <typeparam name="U">The type of the target to synchronize.</typeparam>
    public class ProxyUserProperty<T, U> : BaseProxyProperty<T>
        where U : class, IIdentifiable
    {
        /// <summary>
        /// Create a ProxyUserProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="propertyGetter">The delegate that gets the proxied instance's property.</param>
        /// <param name="propertySetter">The delegate that sets the proxied instance's property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ProxyUserProperty(ProxyToken token, Func<U, T> propertyGetter, Action<T, U> propertySetter, string name, T defaultValue)
            : base(token, name, defaultValue)
        {
            OxHelper.ArgumentNullCheck(propertyGetter, propertySetter);
            this.propertyGetter = propertyGetter;
            this.propertySetter = propertySetter;
        }

        /// <summary>
        /// Create a ProxyUserProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="propertyGetter">The delegate that gets the proxied instance's property.</param>
        /// <param name="propertySetter">The delegate that sets the proxied instance's property.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyUserProperty(ProxyToken token, Func<U, T> propertyGetter, Action<T, U> propertySetter, string name)
            : base(token, name)
        {
            OxHelper.ArgumentNullCheck(propertyGetter, propertySetter);
            this.propertyGetter = propertyGetter;
            this.propertySetter = propertySetter;
        }

        /// <inheritdoc />
        protected override bool TryGetPropertyHook(out T value)
        {
            value = default(T);
            if (Instance == null) return false;
            U instance = OxHelper.Cast<U>(Instance);
            if (instance == null) return false;
            value = propertyGetter(instance);
            return true;
        }

        /// <inheritdoc />
        protected override bool TrySetPropertyHook(T value)
        {
            if (Instance == null) return false;
            U instance = OxHelper.Cast<U>(Instance);
            if (instance == null) return false;
            propertySetter(value, instance);
            return true;
        }

        private readonly Func<U, T> propertyGetter;
        private readonly Action<T, U> propertySetter;
    }
}

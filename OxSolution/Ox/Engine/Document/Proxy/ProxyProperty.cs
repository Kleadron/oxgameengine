using System.Reflection;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A C#-style property that synchronizes a ProxyToken.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    public class ProxyProperty<T> : BaseProxyProperty<T>
    {
        /// <summary>
        /// Create a ProxyProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ProxyProperty(ProxyToken token, string name, T defaultValue)
            : base(token, name, defaultValue) { }

        /// <summary>
        /// Create a ProxyProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyProperty(ProxyToken token, string name)
            : base(token, name, default(T)) { }

        /// <inheritdoc />
        protected override bool TryGetPropertyHook(out T value)
        {
            value = default(T);
            if (Instance == null) return false;
            PropertyInfo property = Instance.GetType().GetProperty(Name);
            object propertyValue = property.GetValue(Instance, null);
            value = (T)propertyValue;
            return true;
        }

        /// <inheritdoc />
        protected override bool TrySetPropertyHook(T value)
        {
            if (Instance == null) return false;
            PropertyInfo property = Instance.GetType().GetProperty(Name);
            property.SetValue(Instance, value, null);
            return true;
        }
    }
}

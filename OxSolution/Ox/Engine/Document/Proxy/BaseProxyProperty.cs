using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// An abstract property machine that synchronizes and calls RaisePropertyChanged on a token
    /// when Value is changed.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public abstract class BaseProxyProperty<T> : ISynchronizable
    {
        /// <summary>
        /// Create a BaseProxyProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public BaseProxyProperty(ProxyToken token, string name, T defaultValue)
        {
            OxHelper.ArgumentNullCheck(token, name);
            this.token = token;
            this.name = name;
            Value = defaultValue;
        }

        /// <summary>
        /// Create a BaseProxyProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        public BaseProxyProperty(ProxyToken token, string name)
            : this(token, name, default(T)) { }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// The value of the property.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                value = NullStringToEmptyString(value); // VALIDATION
                if (Matches(value)) return;
                object oldValue = _value;
                TrySetProperty(value);
                // EXCEPTIONSAFETYLINE
                _value = value;
                token.RaisePropertyChanged(name, oldValue);
            }
        }

        /// <inheritdoc />
        public void SynchronizeFrom()
        {
            T oldValue = _value;
            T newValue;
            if (TryGetProperty(out newValue))
            {
                _value = newValue;
                token.RaisePropertyChanged(name, oldValue);
            }
        }

        /// <inheritdoc />
        public void SynchronizeTo()
        {
            TrySetProperty(Value);
        }

        /// <summary>
        /// Get the propertied object instance.
        /// May be null.
        /// </summary>
        protected IIdentifiable Instance { get { return token.Instance; } }

        /// <summary>
        /// Try to get the property.
        /// </summary>
        protected abstract bool TryGetPropertyHook(out T value);

        /// <summary>
        /// Try to set the property to the specified value.
        /// </summary>
        protected abstract bool TrySetPropertyHook(T value);

        private bool TryGetProperty(out T value)
        {
            return TryGetPropertyHook(out value);
        }

        private bool TrySetProperty(T value)
        {
            return TrySetPropertyHook(value);
        }

        private T NullStringToEmptyString(T value)
        {
            if (typeof(T) != typeof(string)) return value;
            if (value is string) return value;
            return (T)(object)string.Empty;
        }

        private bool Matches(T value)
        {
            return
                (_value != null || value == null) &&
                (_value == null || _value.Equals(value));
        }

        private readonly ProxyToken token;
        private readonly string name;
        private T _value;
    }
}

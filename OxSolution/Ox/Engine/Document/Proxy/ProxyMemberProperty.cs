using System.Reflection;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// An member property that synchronizes a ProxyToken.
    /// </summary>
    /// <typeparam name="T">The type of the trait.</typeparam>
    public class ProxyMemberProperty<T> : BaseProxyProperty<T>
    {
        /// <summary>
        /// Create a ProxyMemberProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="memberName">The member that owns the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ProxyMemberProperty(ProxyToken token, string memberName, string name, T defaultValue)
            : base(token, name, defaultValue)
        {
            OxHelper.ArgumentNullCheck(memberName);
            this.memberName = memberName;
        }

        /// <summary>
        /// Create a ProxyMemberProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="memberName">The member that owns the property.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyMemberProperty(ProxyToken token, string memberName, string name)
            : base(token, name, default(T))
        {
            OxHelper.ArgumentNullCheck(memberName);
            this.memberName = memberName;
        }

        /// <inheritdoc />
        protected override bool TryGetPropertyHook(out T value)
        {
            value = default(T);
            if (Instance == null) return false;
            PropertyInfo memberProperty = Instance.GetType().GetProperty(memberName);
            object memberValue = memberProperty.GetValue(Instance, null);
            PropertyInfo property = memberValue.GetType().GetProperty(Name);
            object propertyValue = property.GetValue(memberValue, null);
            value = (T)propertyValue;
            return true;
        }

        /// <inheritdoc />
        protected override bool TrySetPropertyHook(T value)
        {
            if (Instance == null) return false;
            PropertyInfo memberProperty = Instance.GetType().GetProperty(memberName);
            object memberValue = memberProperty.GetValue(Instance, null);
            PropertyInfo property = memberValue.GetType().GetProperty(Name);
            property.SetValue(memberValue, value, null);
            return true;
        }

        private readonly string memberName;
    }
}

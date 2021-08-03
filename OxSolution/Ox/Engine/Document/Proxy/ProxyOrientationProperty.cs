using System.Reflection;
using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// An degree-based orientation property that synchronizes a ProxyToken.
    /// </summary>
    public class ProxyDegreeOrientationProperty : BaseProxyProperty<Vector3>
    {
        /// <summary>
        /// Create a ProxyDegreeOrientationProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ProxyDegreeOrientationProperty(ProxyToken token, string name, Vector3 defaultValue)
            : base(token, name, defaultValue) { }

        /// <summary>
        /// Create a ProxyDegreeOrientationProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyDegreeOrientationProperty(ProxyToken token, string name)
            : base(token, name) { }

        /// <inheritdoc />
        protected override bool TryGetPropertyHook(out Vector3 value)
        {
            value = default(Vector3);
            return false;
        }

        /// <inheritdoc />
        protected override bool TrySetPropertyHook(Vector3 value)
        {
            if (Instance == null) return false;
            Matrix orientation = BuildOrientation(value);
            PropertyInfo property = Instance.GetType().GetProperty(Name);
            property.SetValue(Instance, orientation, null);
            return true;
        }

        private static Matrix BuildOrientation(Vector3 value)
        {
            return
                Matrix.CreateRotationX(MathHelper.ToRadians(value.X)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(value.Y)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(value.Z));
        }
    }

    /// <summary>
    /// A nullable matrix-based orientation property that synchronizes a ProxyToken.
    /// </summary>
    public class ProxyNullableMatrixOrientationProperty : BaseProxyProperty<Matrix?>
    {
        /// <summary>
        /// Create a ProxyNullableMatrixOrientationProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ProxyNullableMatrixOrientationProperty(ProxyToken token, string name, Matrix? defaultValue)
            : base(token, name, defaultValue) { }

        /// <summary>
        /// Create a ProxyNullableMatrixOrientationProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyNullableMatrixOrientationProperty(ProxyToken token, string name)
            : base(token, name) { }

        protected override bool TryGetPropertyHook(out Matrix? value)
        {
            value = default(Matrix?);
            if (Instance == null) return false;
            PropertyInfo property = Instance.GetType().GetProperty(Name);
            value = (Matrix?)property.GetValue(Instance, null);
            return true;
        }

        protected override bool TrySetPropertyHook(Matrix? value)
        {
            if (Instance == null) return false;
            if (value == null) return false;
            PropertyInfo property = Instance.GetType().GetProperty(Name);
            property.SetValue(Instance, value.Value, null);
            return true;
        }
    }

    public class ProxyOrientationProperty : ISynchronizable
    {
        /// <summary>
        /// Create an ProxyOrientationProperty.
        /// </summary>
        /// <param name="token">The proxy token.</param>
        /// <param name="name">The name of the property.</param>
        public ProxyOrientationProperty(ProxyToken token, string name)
        {
            OxHelper.ArgumentNullCheck(token, name);
            this.token = token;
            this.name = name;
            propertyDesignTime = new ProxyDegreeOrientationProperty(token, name);
            propertyPlayTime = new ProxyNullableMatrixOrientationProperty(token, name);
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The value at design-time.
        /// </summary>
        public Vector3 ValueDesignTime
        {
            get { return propertyDesignTime.Value; }
            set { propertyDesignTime.Value = value; }
        }

        /// <summary>
        /// The value at play-time. Overrides the design-time value at play-time.
        /// </summary>
        public Matrix? ValuePlayTime
        {
            get { return propertyPlayTime.Value; }
            set { propertyPlayTime.Value = value; }
        }

        public void SynchronizeFrom()
        {
            propertyDesignTime.SynchronizeFrom();
            propertyPlayTime.SynchronizeFrom();
        }

        public void SynchronizeTo()
        {
            propertyDesignTime.SynchronizeTo();
            propertyPlayTime.SynchronizeTo();
        }

        /// <summary>May be null.</summary>
        protected IIdentifiable Instance
        {
            get { return token.Instance; }
        }

        private readonly ProxyNullableMatrixOrientationProperty propertyPlayTime;
        private readonly ProxyDegreeOrientationProperty propertyDesignTime;
        private readonly ProxyToken token;
        private readonly string name;
    }
}

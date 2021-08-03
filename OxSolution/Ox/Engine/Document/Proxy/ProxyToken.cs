using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A serializable token that proxies an object instance.
    /// </summary>
    public abstract class ProxyToken : ItemToken, ISynchronizable
    {
        /// <summary>
        /// Create a ProxyToken.
        /// </summary>
        public ProxyToken()
        {
            PropertyChanged += this_PropertyChanged;
        }

        /// <summary>
        /// The proxied object instance.
        /// May be null.
        /// </summary>
        [XmlIgnore, Browsable(false)]
        public IIdentifiable Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        /// <inheritdoc />
        public void SynchronizeTo()
        {
            SynchronizeToNonProxyProperties();
            SynchronizeToProxyProperties();
        }

        /// <inheritdoc />
        public void SynchronizeFrom()
        {
            SynchronizeFromNonProxyProperties();
            SynchronizeFromProxyProperties();
        }

        /// <summary>
        /// Synchronize to a subclass's properties that are NOT proxy properties.
        /// The synchronization of the proxy properties done automatically via reflection over
        /// non-public fields.
        /// </summary>
        protected virtual void SynchronizeToNonProxyPropertiesHook() { }

        /// <summary>
        /// Synchronize from a subclass's properties that are NOT proxy properties.
        /// The synchronization of the proxy properties done automatically via reflection over
        /// non-public fields.
        /// </summary>
        protected virtual void SynchronizeFromNonProxyPropertiesHook() { }

        /// <inheritdoc />
        protected override string FormatName(string name)
        {
            return name == ItemType ? DefaultName : name;
        }

        /// <inheritdoc />
        protected override bool DuplicationFilterHook(PropertyInfo property)
        {
            return
                property.Name != "Instance" &&
                property.Name != "Name";
        }

        /// <inheritdoc />
        protected override bool ImpersonationFilterHook(PropertyInfo property)
        {
            return
                property.Name != "Instance" &&
                property.Name != "Name";
        }

        private void this_PropertyChanged(object sender, string propertyName, object oldValue)
        {
            if (Instance == null) return;
            switch (propertyName)
            {
                case "Guid": Instance.Guid = Guid; break;
                case "Name": Instance.Name = Name; break;
            }
        }

        private void SynchronizeFromNonProxyProperties()
        {
            if (Instance == null) return;
            Guid = Instance.Guid;
            Name = Instance.Name;
            SynchronizeFromNonProxyPropertiesHook();
        }

        private void SynchronizeToNonProxyProperties()
        {
            if (Instance == null) return;
            Instance.Guid = Guid;
            Instance.Name = Name;
            SynchronizeToNonProxyPropertiesHook();
        }

        private void SynchronizeFromProxyProperties()
        {
            Type type = GetType();
            do
            {
                type
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .ForEach(x => SynchronizeFromProxyProperty(x));
                type = type.BaseType;
            }
            while (type != null);
        }

        private void SynchronizeToProxyProperties()
        {
            Type type = GetType();
            do
            {
                type
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .ForEach(x => SynchronizeToProxyProperty(x));
                type = type.BaseType;
            }
            while (type != null);
        }

        private void SynchronizeFromProxyProperty(FieldInfo field)
        {
            object fieldValue = field.GetValue(this);
            ISynchronizable synchronizable = fieldValue as ISynchronizable;
            if (synchronizable != null) synchronizable.SynchronizeFrom();
        }

        private void SynchronizeToProxyProperty(FieldInfo field)
        {
            object fieldValue = field.GetValue(this);
            ISynchronizable synchronizable = fieldValue as ISynchronizable;
            if (synchronizable != null) synchronizable.SynchronizeTo();
        }

        /// <summary>May be null.</summary>
        private IIdentifiable _instance;
    }
}

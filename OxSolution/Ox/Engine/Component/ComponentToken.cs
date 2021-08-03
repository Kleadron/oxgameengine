using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Ox.Engine.DocumentNamespace;

namespace Ox.Engine.Component
{
    /// <summary>
    /// A serializable component token.
    /// </summary>
    public class ComponentToken : ProxyToken
    {
        /// <summary>
        /// Create a ComponentToken.
        /// </summary>
        public ComponentToken()
        {
            designTimeData = new ProxyProperty<string>(this, "DesignTimeData");
        }

        /// <summary>
        /// Create the proxied component instance.
        /// </summary>
        public OxComponent CreateInstance(OxEngine engine, string domainName, bool ownedByDomain)
        {
            OxHelper.ArgumentNullCheck(engine, domainName);
            if (Instance != null) throw new InvalidOperationException("Cannot create more than one instance per token.");
            Instance = CreateInstanceHook(engine, domainName, ownedByDomain);
            SynchronizeTo();
            return Instance;
        }

        /// <summary>May be null.</summary>
        [DefaultValue(null), Browsable(false)]
        public ComponentScriptMemento ScriptMemento
        {
            get { return scriptMemento; }
            set { scriptMemento = value; }
        }

        /// <summary>May be null.</summary>
        [XmlIgnore, Browsable(false)]
        public new OxComponent Instance
        {
            get { return OxHelper.Cast<OxComponent>(base.Instance); }
            set { base.Instance = value; }
        }

        [DefaultValue("")]
        public string DesignTimeData
        {
            get { return designTimeData.Value; }
            set { designTimeData.Value = value; }
        }

        [DefaultValue("")]
        public string ScriptClass
        {
            get { return scriptClass; }
            set
            {
                if (value == null) value = string.Empty; // VALIDATION
                object oldValue = scriptClass;
                scriptClass = value;
                RaisePropertyChanged("ScriptClass", oldValue);
            }
        }

        [DefaultValue(false)]
        public bool Frozen
        {
            get { return frozen; }
            set
            {
                object oldValue = frozen;
                frozen = value;
                RaisePropertyChanged("Frozen", oldValue);
            }
        }

        /// <summary>
        /// Handle creating the proxied component instance.
        /// </summary>
        protected virtual OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new OxComponent(engine, domainName, ownedByDomain);
        }

        /// <summary>
        /// Handle synchronizing the object from its non-proxy properties.
        /// </summary>
        protected override void SynchronizeFromNonProxyPropertiesHook()
        {
            base.SynchronizeFromNonProxyPropertiesHook();
            if (Instance == null) return;
            scriptClass = Instance.ScriptClass;
            scriptMemento = Instance.ProduceScriptMemento();
        }

        /// <summary>
        /// Handle synchronizing the object to its non-proxy properties.
        /// </summary>
        protected override void SynchronizeToNonProxyPropertiesHook()
        {
            base.SynchronizeToNonProxyPropertiesHook();
            if (Instance == null) return;
            // Instance.ScriptClass is intentionally not set since a component should not be
            // scripted in editing mode. NOTE: this will cause problems if people are expecting to
            // use a token to mutate components at play-time.
            Instance.ConsumeScriptMemento(scriptMemento);
        }

        private readonly ProxyProperty<string> designTimeData;
        /// <summary>May be null.</summary>
        private ComponentScriptMemento scriptMemento;
        private string scriptClass = string.Empty;
        private bool frozen;
    }
}

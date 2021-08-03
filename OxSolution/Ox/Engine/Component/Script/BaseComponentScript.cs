using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Component
{
    /// <summary>
    /// An exception that can occur when working with a component script.
    /// </summary>
    [Serializable]
    public class ComponentScriptException : Exception
    {
        public ComponentScriptException() { }
        public ComponentScriptException(string message) : base(message) { }
        public ComponentScriptException(string message, Exception innerException) : base(message, innerException) { }
#if !XBOX360
        protected ComponentScriptException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }

    /// <summary>
    /// A memento for serializing a stateful component script.
    /// </summary>
    public class ComponentScriptMemento : SerializableDictionary<string, object> { }

    /// <summary>
    /// A script that controls a component.
    /// </summary>
    public class BaseComponentScript : Disposable2, IOxUpdateable, IIdentifiable
    {
        /// <summary>
        /// Create a component script.
        /// </summary>
        /// <param name="engine">See property Engine.</param>
        /// <param name="component">The component controlled by the script.</param>
        public BaseComponentScript(OxEngine engine, Transfer<OxComponent> component)
        {
            OxHelper.ArgumentNullCheck(engine, component.Value);            
            if (component.Value.OwnedByDomain) throw new ComponentScriptException(
                "Component must not be owned by the domain; its lifetime must be under the script's control.");

            this.engine = engine;
            this.component = component.Value;

            domainName = this.component.DomainName;
            identifiable = new Identifiable(GetType().Name);

            updateable.EnabledChanged += updateable_EnabledChanged;
            updateable.UpdateOrderChanged += updateable_UpdateOrderChanged;
            identifiable.GuidChanged += identifiable_GuidChanged;
            identifiable.NameChanged += identifiable_NameChanged;

            this.component.Script = this;
            AddGarbage(this.component);
            engine.AddScript(this);
#if DEBUG
            engine.ScriptRemoved += engine_ScriptRemoved;
#endif
        }

        /// <inheritdoc />
        public Guid Guid
        {
            get { return identifiable.Guid; }
            set { identifiable.Guid = value; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return identifiable.Name; }
            set { identifiable.Name = value; }
        }

        /// <inheritdoc />
        public string DefaultName { get { return identifiable.DefaultName; } }

        /// <summary>
        /// Design time data.
        /// </summary>
        public string DesignTimeData { get { return component.DesignTimeData; } }

        /// <summary>
        /// The name of the domain that owns the script.
        /// </summary>
        public string DomainName { get { return domainName; } }

        /// <inheritdoc />
        public bool Enabled
        {
            get { return updateable.Enabled; }
            set { updateable.Enabled = value; }
        }

        /// <inheritdoc />
        public int UpdateOrder
        {
            get { return updateable.UpdateOrder; }
            set { updateable.UpdateOrder = value; }
        }

        /// <inheritdoc />
        public event Action<IOxUpdateable> EnabledChanged;

        /// <inheritdoc />
        public event Action<IOxUpdateable> UpdateOrderChanged;

        /// <inheritdoc />
        public event GuidChanged GuidChanged;

        /// <inheritdoc />
        public event NameChanged NameChanged;

        /// <inheritdoc />
        public void Update(GameTime gameTime)
        {
            UpdateHook(gameTime);
        }

        /// <summary>
        /// If the script is stateful, produce a memento.
        /// May return null.
        /// </summary>
        public ComponentScriptMemento ProduceMemento()
        {
            return ProduceMementoHook();
        }

        /// <summary>
        /// If the script is stateful, consume a memento.
        /// </summary>
        /// <param name="memento">The memento. May be null.</param>
        public void ConsumeMemento(ComponentScriptMemento memento)
        {
            ConsumeMementoHook(memento);
        }

        /// <summary>
        /// Notify script that its document (and thus its peers and children) has been loaded.
        /// </summary>
        public void DocumentLoaded()
        {
            DocumentLoadedHook();
        }

        /// <summary>
        /// Dispose both the script and the component it control.
        /// </summary>
        public void DisposeIncludingComponent()
        {
            OxComponent scriptedComponent = component;
            Dispose();
            scriptedComponent.Dispose();
        }

        /// <summary>
        /// The engine.
        /// </summary>
        protected OxEngine Engine { get { return engine; } }

        /// <summary>
        /// The scripted component.
        /// </summary>
        protected OxComponent Component { get { return component; } }

        /// <summary>
        /// The engine as type U.
        /// </summary>
        protected U GetEngine<U>() where U : class
        {
            return OxHelper.Cast<U>(engine);
        }

        /// <summary>
        /// Handle the fact that the script's document has finished loading.
        /// </summary>
        protected virtual void DocumentLoadedHook() { }

        /// <summary>
        /// Consume the memento used to serialize the script.
        /// </summary>
        /// <param name="memento"></param>
        protected virtual void ConsumeMementoHook(ComponentScriptMemento memento) { }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if DEBUG
                engine.ScriptRemoved -= engine_ScriptRemoved;
#endif
                engine.RemoveScript(this);
                component.Script = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Produce the memento used to serialize the script.
        /// </summary>
        protected virtual ComponentScriptMemento ProduceMementoHook() { return null; }

        /// <summary>
        /// Handle updating the script.
        /// </summary>
        protected virtual void UpdateHook(GameTime gameTime) { }

        private void updateable_EnabledChanged(IOxUpdateable sender)
        {
            if (EnabledChanged != null) EnabledChanged(this);
        }

        private void updateable_UpdateOrderChanged(IOxUpdateable sender)
        {
            if (UpdateOrderChanged != null) UpdateOrderChanged(this);
        }

        private void identifiable_GuidChanged(IIdentifiable sender, Guid oldGuid)
        {
            if (GuidChanged != null) GuidChanged(this, oldGuid);
        }

        private void identifiable_NameChanged(IIdentifiable sender, string oldName)
        {
            if (NameChanged != null) NameChanged(this, oldName);
        }

#if DEBUG
        private void engine_ScriptRemoved(OxEngine sender, BaseComponentScript script)
        {
            System.Diagnostics.Trace.Assert(script != this, "Cannot manually remove a script from its engine.");
        }
#endif

        private readonly IOxUpdateable updateable = new OxUpdateable();
        private readonly IIdentifiable identifiable;
        private readonly OxComponent component;
        private readonly OxEngine engine;
        private readonly string domainName;
    }
}

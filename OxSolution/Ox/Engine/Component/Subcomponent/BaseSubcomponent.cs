using System;
using Ox.Engine.Utility;

namespace Ox.Engine.Component
{
    /// <summary>
    /// Augments a component's capabilities.
    /// </summary>
    public class BaseSubcomponent : Disposable2, IIdentifiable
    {
        /// <summary>
        /// Create a BaseSubcomponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The component to augment.</param>
        public BaseSubcomponent(OxEngine engine, OxComponent component)
        {
            OxHelper.ArgumentNullCheck(engine, component);

            this.engine = engine;
            this.component = component;

            identifiable = new Identifiable(GetType().Name);
            identifiable.GuidChanged += identifiable_GuidChanged;
            identifiable.NameChanged += identifiable_NameChanged;

            component.AddSubcomponent(this);
            engine.AddSubcomponent(this);
#if DEBUG
            engine.SubcomponentRemoved += engine_SubcomponentRemoved;
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

        /// <inheritdoc />
        public event GuidChanged GuidChanged;

        /// <inheritdoc />
        public event NameChanged NameChanged;

        /// <summary>
        /// The augmented component.
        /// </summary>
        protected OxComponent Component { get { return component; } }

        /// <summary>
        /// The engine.
        /// </summary>
        protected OxEngine Engine { get { return engine; } }

        /// <summary>
        /// The domain in which the subcomponent reside.
        /// </summary>
        protected string DomainName { get { return component.DomainName; } }

        /// <summary>
        /// The engine as type U.
        /// </summary>
        protected U GetEngine<U>() where U : class
        {
            return OxHelper.Cast<U>(engine);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if DEBUG
                engine.SubcomponentRemoved -= engine_SubcomponentRemoved;
#endif
                engine.RemoveSubcomponent(this);
                component.RemoveSubcomponent(this);
            }
            base.Dispose(disposing);
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
        private void engine_SubcomponentRemoved(OxEngine sender, BaseSubcomponent subcomponent)
        {
            System.Diagnostics.Trace.Assert(subcomponent != this,
                "Cannot manually remove a subcomponent from its engine.");
        }
#endif

        private readonly Identifiable identifiable;
        private readonly OxEngine engine;
        private OxComponent component;
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Component
{
    /// <summary>
    /// Compares the update order of updateables.
    /// </summary>
    public class ComponentUpdateOrderComparer<T> : IComparer<T>
        where T : UpdateableComponent
    {
        /// <inheritdoc />
        public int Compare(T x, T y) { return OxMathHelper.Compare(x.UpdateOrder, y.UpdateOrder); }
    }

    /// <summary>
    /// Appends hierarchical updating capabilities to OxComponent.
    /// </summary>
    public class UpdateableComponent : OxComponent
    {
        /// <summary>
        /// Create an UpdateableComponent.
        /// </summary>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="engine">The engine.</param>
        public UpdateableComponent(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain) { }

        /// <summary>
        /// The eligibility for updating.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                UpdateEnabledWorld(); // put self in valid state before firing events
                RaiseEnabledLocalChanged();
            }
        }

        /// <summary>
        /// The cumulative eligibility for updating.
        /// </summary>
        public bool EnabledWorld { get { return _enabledWorld; } }

        /// <summary>
        /// The update order relative to peer components.
        /// </summary>
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder == value) return;
                _updateOrder = value;
                if (UpdateOrderChanged != null) UpdateOrderChanged(this);
            }
        }

        /// <summary>
        /// Raised when enabled is changed.
        /// </summary>
        public event Action<UpdateableComponent> EnabledChanged;

        /// <summary>
        /// Raised when cumulative enabled is changed.
        /// </summary>
        public event Action<UpdateableComponent> EnabledWorldChanged;

        /// <summary>
        /// Raised when the update order is changed.
        /// </summary>
        public event Action<UpdateableComponent> UpdateOrderChanged;

        /// <summary>
        /// Update the object. Must be called once per frame if this object is cumulatively enabled.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            UpdateHook(gameTime);
            UpdateChildren(gameTime);
        }

        /// <summary>
        /// Handle updating the component.
        /// </summary>
        protected virtual void UpdateHook(GameTime gameTime) { }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new UpdateableComponentToken();
        }

        /// <inheritdoc />
        protected override bool IsValidChildHook(OxComponent child)
        {
            return child is UpdateableComponent;
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertyHook(string property)
        {
            base.UpdateWorldPropertyHook(property);
            if (property == "Enabled") UpdateEnabledWorld();
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertiesHook()
        {
            base.UpdateWorldPropertiesHook();
            UpdateEnabledWorld();
        }

        private void UpdateEnabledWorld()
        {
            _enabledWorld = CalculateEnabledWorld();
            UpdateWorldPropertyOfChildren("Enabled");
            RaiseEnabledWorldChanged();
        }

        private bool CalculateEnabledWorld()
        {
            UpdateableComponent parent = GetParent<UpdateableComponent>();
            if (parent == null) return Enabled;
            return Enabled & parent.EnabledWorld;
        }

        private void UpdateChildren(GameTime gameTime)
        {
            CollectChildren<UpdateableComponent>(CollectionAlgorithm.Shallow, cachedChildren);
            {
                if (Engine.UpdateInOrder) cachedChildren.Sort(updateOrderComparer);
                for (int i = 0; i < cachedChildren.Count; ++i)
                {
                    UpdateableComponent child = cachedChildren[i];
                    if (child.EnabledWorld) child.Update(gameTime);
                }
            }
            cachedChildren.Clear();
        }

        private void RaiseEnabledLocalChanged()
        {
            if (EnabledChanged != null) EnabledChanged(this);
        }

        private void RaiseEnabledWorldChanged()
        {
            if (EnabledWorldChanged != null) EnabledWorldChanged(this);
        }

        private static readonly ComponentUpdateOrderComparer<UpdateableComponent> updateOrderComparer =
            new ComponentUpdateOrderComparer<UpdateableComponent>();

        private readonly List<UpdateableComponent> cachedChildren = new List<UpdateableComponent>();
        private bool _enabled = true;
        private bool _enabledWorld = true;
        private int _updateOrder;
    }
}

using System;
using System.Collections.Generic;
using Ox.Engine.Utility;

namespace Ox.Engine.Component
{
    /// <summary>
    /// A type of algorithm for recursively collecting child components.
    /// </summary>
    public enum CollectionAlgorithm
    {
        /// <summary>
        /// Collect only the direct children.
        /// </summary>
        Shallow = 0,
        /// <summary>
        /// Collect only the closest children in descending order.
        /// </summary>
        ShallowDescending,
        /// <summary>
        /// Collect all children in descending order.
        /// </summary>
        Descending
    }

    /// <summary>
    /// Raised when the parent of a component is changed.
    /// </summary>
    public delegate void ParentChanged(OxComponent sender, OxComponent oldParent);
    /// <summary>
    /// Raised when a child is added to a component.
    /// </summary>
    public delegate void ChildAdded(OxComponent sender, OxComponent child);
    /// <summary>
    /// Raised when a child is removed from a component.
    /// </summary>
    public delegate void ChildRemoved(OxComponent sender, OxComponent child);
    /// <summary>
    /// Raised when a subcomponent is added to a component.
    /// </summary>
    public delegate void SubcomponentAdded(OxComponent sender, BaseSubcomponent subcomponent);
    /// <summary>
    /// Raised when a subcomponent is removed from a component.
    /// </summary>
    public delegate void SubcomponentRemoved(OxComponent sender, BaseSubcomponent subcomponent);

    /// <summary>
    /// The Ox component type.
    /// </summary>
    public class OxComponent : Disposable2, IIdentifiable
    {
        /// <summary>
        /// Create an OxComponent. Ownership of the instance goes directly to the OxEngine object,
        /// not the invoker of the constructor.
        /// </summary>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="engine">The engine.</param>
        public OxComponent(OxEngine engine, string domainName, bool ownedByDomain)
        {
            OxHelper.ArgumentNullCheck(engine, domainName);

            this.engine = engine;
            this.domainName = domainName;
            this.ownedByDomain = ownedByDomain;

            identifiable = new Identifiable(GetType().Name);

            traits.TraitSet += traits_TraitSet;
            identifiable.GuidChanged += identifiable_GuidChanged;
            identifiable.NameChanged += identifiable_NameChanged;
            subcomponents.NameConflicted += subcomponents_NameConflicted;

            engine.AddComponent(this);
#if DEBUG
            engine.ComponentRemoved += engine_ComponentRemoved;
#endif
        }

        /// <summary>
        /// The script that controls the component.
        /// Do not set this directly. Instead set the ScriptClass property.
        /// May be null.
        /// </summary>
        public BaseComponentScript Script
        {
            get { return script; }
            set { script = value; }
        }
        
        /// <summary>
        /// The parent of the component.
        /// May be null.
        /// </summary>
        public OxComponent Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value) return; // keep from recursing infinitely
                OxComponent oldParent = _parent;
                if (_parent != null) _parent.RemoveChild(this);
                _parent = value;
                if (_parent != null) _parent.AddChild(this);
                if (ParentChanged != null) ParentChanged(this, oldParent);
            }
        }

        /// <inheritdoc />
        public Traits Traits { get { return traits; } }

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
        /// TODO: try to find a way to make this read only.
        /// </summary>
        public string DesignTimeData
        {
            get { return designTimeData; }
            set { designTimeData = value; }
        }
        
        /// <summary>
        /// The domain in which the component lives.
        /// </summary>
        public string DomainName { get { return domainName; } }

        /// <summary>
        /// The name of the script class.
        /// Empty string if no script.
        /// </summary>
        public string ScriptClass
        {
            get { return script != null ? script.GetType().Name : string.Empty; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (ScriptClass == value) return;
                if (script != null) script.Dispose();
                script = Engine.CreateScript(value, new Transfer<OxComponent>(this));
            }
        }
        
        /// <summary>
        /// Does the domain take ownership of the object?
        /// </summary>
        public bool OwnedByDomain { get { return ownedByDomain; } }
        
        /// <summary>
        /// The number of children.
        /// </summary>
        public int ChildCount { get { return children.Count; } }

        /// <summary>
        /// Raised when the parent is changed.
        /// </summary>
        public event ParentChanged ParentChanged;
        
        /// <summary>
        /// Raised when a child component is added.
        /// </summary>
        public event ChildAdded ChildAdded;
        
        /// <summary>
        /// Raised when a child component is removed.
        /// </summary>
        public event ChildRemoved ChildRemoved;
        
        /// <summary>
        /// Raised when a subcomponent is added.
        /// </summary>
        public event SubcomponentAdded SubcomponentAdded;
        
        /// <summary>
        /// Raised when a subcomponent is removed.
        /// </summary>
        public event SubcomponentRemoved SubcomponentRemoved;

        /// <inheritdoc />
        public event GuidChanged GuidChanged;

        /// <inheritdoc />
        public event NameChanged NameChanged;
        
        /// <summary>
        /// Raised when a trait is set.
        /// </summary>
        public event TraitAction<OxComponent> TraitSet;

        /// <summary>
        /// Raised when multiple subcomponents are assigned the same name.
        /// </summary>
        public event Action<OxComponent> SubcomponentNameConflicted;

        /// <summary>
        /// Get the parent as type T.
        /// May return null.
        /// </summary>
        public T GetParent<T>() where T : class
        {
            return Parent as T;
        }

        /// <summary>
        /// Find the first parent of type T by searching upward.
        /// May be null.
        /// </summary>
        public T FindParent<T>() where T : class
        {
            OxComponent parent = Parent;
            if (parent == null) return null;
            T parentAsT = parent as T;
            if (parentAsT != null) return parentAsT;
            return parent.FindParent<T>();
        }

        /// <summary>
        /// Find the first parent of type T that satisfies a predicate by searching upward.
        /// May be null.
        /// </summary>
        public T FindParent<T>(Func<T, bool> predicate) where T : class
        {
            OxComponent parent = Parent;
            if (parent == null) return null;
            T parentAsT = parent as T;
            if (parentAsT != null && predicate(parentAsT)) return parentAsT;
            return parent.FindParent<T>(predicate);
        }

        /// <summary>
        /// Add a child component.
        /// </summary>
        public void AddChild(OxComponent child)
        {
            OxHelper.ArgumentNullCheck(child);
            if (children.Contains(child)) return; // keep from recursing infinitely
            if (child == this) throw new ArgumentException("Cannot make a component a child of itself.");
            if (IsChildParent(child)) throw new ArgumentException("Cannot create a circular component relationship.");
            if (!IsValidChildHook(child)) throw new ArgumentException("Cannot add a invalid child.");
            children.Add(child);
            child.Parent = this;
            child.UpdateWorldProperties();
            if (ChildAdded != null) ChildAdded(this, child);
        }

        /// <summary>
        /// Remove a child component.
        /// </summary>
        public bool RemoveChild(OxComponent child)
        {
            OxHelper.ArgumentNullCheck(child);
            if (!children.Contains(child)) return false; // keep from recursing infinitely
            bool result = children.Remove(child);
            child.Parent = null;
            child.UpdateWorldProperties();
            if (ChildRemoved != null) ChildRemoved(this, child);
            return result;
        }

        /// <summary>
        /// Remove all the child components.
        /// </summary>
        public void ClearChildren()
        {
            while (children.Count != 0) RemoveChild(children[0]);
        }

        /// <summary>
        /// Get child at the specified index.
        /// </summary>
        public OxComponent GetChild(int index)
        {
            return children[index];
        }

        /// <summary>
        /// Get child of type T at the specified index.
        /// May return null.
        /// </summary>
        public T GetChild<T>(int index) where T : OxComponent
        {
            return OxHelper.Cast<T>(GetChild(index));
        }

        /// <summary>
        /// Collect the specified child components of type T in the specified manner.
        /// </summary>
        /// <param name="algorithm">The algorithm for collecting child components.</param>
        /// <param name="result">The list into which the collected childen are added.</param>
        /// <typeparam name="T">The type of child components to collect.</typeparam>
        public IList<T> CollectChildren<T>(CollectionAlgorithm algorithm, IList<T> result)
            where T : OxComponent
        {
            switch (algorithm)
            {
                case CollectionAlgorithm.Shallow: CollectChildrenShallow(result); break;
                case CollectionAlgorithm.ShallowDescending: CollectChildrenShallowDescending(result); break;
                case CollectionAlgorithm.Descending: CollectChildrenDescending(result); break;
            }

            return result;
        }

        /// <summary>
        /// Collect the specified child components of type T in the specified manner that satisfies
        /// the predicate.
        /// </summary>
        /// <param name="algorithm">The algorithm for collecting child components.</param>
        /// <param name="predicate">The filter through which the child must pass.</param>
        /// <param name="result">The list into which the collected childen are added.</param>
        /// <typeparam name="T">The type of child components to collect.</typeparam>
        public IList<T> CollectChildren<T>(CollectionAlgorithm algorithm, Func<T, bool> predicate, IList<T> result)
            where T : OxComponent
        {
            switch (algorithm)
            {
                case CollectionAlgorithm.Shallow: CollectChildrenShallow(predicate, result); break;
                case CollectionAlgorithm.ShallowDescending: CollectChildrenShallowDescending(predicate, result); break;
                case CollectionAlgorithm.Descending: CollectChildrenDescending(predicate, result); break;
            }

            return result;
        }

        /// <summary>
        /// Add a subcomponent.
        /// </summary>
        public void AddSubcomponent(BaseSubcomponent subcomponent)
        {
            System.Diagnostics.Trace.Assert(!subcomponents.Contains<object>(subcomponent.Name),
                "Subcomponent " + subcomponent.Name + " added to a component multiple times.");
            subcomponents.Add(subcomponent);
            if (SubcomponentAdded != null) SubcomponentAdded(this, subcomponent);
        }

        /// <summary>
        /// Remove a subcomponent.
        /// </summary>
        public bool RemoveSubcomponent(BaseSubcomponent subcomponent)
        {
            bool result = subcomponents.Remove(subcomponent);
            if (result && SubcomponentRemoved != null) SubcomponentRemoved(this, subcomponent);
            return result;
        }

        /// <summary>
        /// Does the component have a subcomponent with the specified name?
        /// </summary>
        public bool ContainsSubcomponent<T>(string name) where T : class
        {
            return subcomponents.Contains<T>(name);
        }

        /// <summary>
        /// Get the subcomponent with the specified name.
        /// </summary>
        public T GetSubcomponent<T>(string name) where T : class
        {
            return subcomponents.Get<T>(name);
        }

        /// <summary>
        /// Collect all subcomponents of type T.
        /// </summary>
        public IList<T> CollectSubcomponents<T>(IList<T> result) where T : class
        {
            return subcomponents.Collect(result);
        }

        /// <summary>
        /// Collect all subcomponents of type T that satisfy a predicate.
        /// </summary>
        public IList<T> CollectSubcomponents<T>(Func<T, bool> predicate, IList<T> result) where T : class
        {
            return subcomponents.Collect(predicate, result);
        }

        /// <summary>
        /// Add a trait at its default value.
        /// </summary>
        public void AddTrait<T>(string name)
        {
            traits.Add<T>(name);
        }

        /// <summary>
        /// Is the specified trait attached?
        /// </summary>
        public bool ContainsTrait<T>(string name)
        {
            return traits.Contains<T>(name);
        }

        /// <summary>
        /// Try to get a trait.
        /// </summary>
        public bool TryGetTrait<T>(string name, out T value)
        {
            return traits.TryGet(name, out value);
        }

        /// <summary>
        /// Try to get a trait, returning a default value if it's not found.
        /// </summary>
        public T TryGetTrait<T>(string name, T defaultValue)
        {
            return traits.TryGet(name, defaultValue);
        }

        /// <summary>
        /// Get a trait.
        /// </summary>
        public T GetTrait<T>(string name)
        {
            return traits.Get<T>(name);
        }

        /// <summary>
        /// Set a trait to the specified value.
        /// </summary>
        public void SetTrait<T>(string name, T value)
        {
            traits.Set(name, value);
        }

        /// <summary>
        /// Recursively update a world property.
        /// </summary>
        /// <param name="property">The property to update.</param>
        public void UpdateWorldProperty(string property)
        {
            OxHelper.ArgumentNullCheck(property);
            UpdateWorldPropertyHook(property);
        }

        /// <summary>
        /// Recursively update all world properties.
        /// </summary>
        public void UpdateWorldProperties()
        {
            UpdateWorldPropertiesHook();
        }

        /// <summary>
        /// Encapsulate the component.
        /// </summary>
        public void Encapsulate()
        {
            identifiable.Name = identifiable.DefaultName;
        }

        /// <summary>
        /// Create a component token for this component.
        /// </summary>
        public ComponentToken CreateToken()
        {
            ComponentToken token = CreateComponentTokenHook();
            token.Instance = this;
            token.SynchronizeFrom();
            return token;
        }

        /// <summary>
        /// If the script is stateful (and exists), produce a memento.
        /// May return null.
        /// </summary>
        public ComponentScriptMemento ProduceScriptMemento()
        {
            return script != null ? script.ProduceMemento() : null;
        }

        /// <summary>
        /// If the script is stateful (and exists), consume a memento.
        /// </summary>
        /// <param name="scriptMemento">The memento. May be null.</param>
        public void ConsumeScriptMemento(ComponentScriptMemento scriptMemento)
        {
            if (script != null) script.ConsumeMemento(scriptMemento);
        }

        /// <summary>
        /// Notify component that its document (and thus its peers and children) has been loaded.
        /// </summary>
        public void DocumentLoaded()
        {
            if (script != null) script.DocumentLoaded();
        }

        /// <summary>
        /// The engine.
        /// </summary>
        protected OxEngine Engine { get { return engine; } }

        /// <summary>
        /// Handle creating the related ComponentToken.
        /// </summary>
        protected virtual ComponentToken CreateComponentTokenHook()
        {
            return new ComponentToken();
        }

        /// <summary>
        /// The engine as type T.
        /// </summary>
        protected T GetEngine<T>() where T : class
        {
            return OxHelper.Cast<T>(engine);
        }

        /// <summary>
        /// Update the specified children's world-space property.
        /// </summary>
        protected void UpdateWorldPropertyOfChildren(string property)
        {
            CollectChildren<OxComponent>(CollectionAlgorithm.Shallow, cachedChildren);
            {
                for (int i = 0; i < cachedChildren.Count; ++i)
                    cachedChildren[i].UpdateWorldProperty(property);
            }
            cachedChildren.Clear();
        }

        /// <summary>
        /// Handle updating the world-space properties.
        /// </summary>
        protected virtual void UpdateWorldPropertiesHook() { }

        /// <summary>
        /// Handle update the specified world-space property.
        /// </summary>
        protected virtual void UpdateWorldPropertyHook(string property) { }

        /// <summary>
        /// Is a component eligible for becoming a child?
        /// </summary>
        protected virtual bool IsValidChildHook(OxComponent child) { return true; }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if DEBUG
                engine.ComponentRemoved -= engine_ComponentRemoved;
#endif
                DisposeSubcomponents();
                engine.RemoveComponent(this);
                Parent = null;
            }
            base.Dispose(disposing);
        }

        private void traits_TraitSet(Traits sender, string name)
        {
            if (TraitSet != null) TraitSet(this, name);
        }

        private void identifiable_GuidChanged(IIdentifiable sender, Guid oldGuid)
        {
            if (GuidChanged != null) GuidChanged(this, oldGuid);
        }

        private void identifiable_NameChanged(IIdentifiable sender, string oldName)
        {
            if (NameChanged != null) NameChanged(this, oldName);
        }

        private void subcomponents_NameConflicted(QueriableIdentifiables<BaseSubcomponent> subcomponents)
        {
            if (SubcomponentNameConflicted != null) SubcomponentNameConflicted(this);
        }

        private IList<T> CollectChildrenShallow<T>(IList<T> result)
            where T : OxComponent
        {
            for (int i = 0; i < children.Count; ++i)
            {
                T childAsT = children[i] as T;
                if (childAsT != null) result.Add(childAsT);
            }

            return result;
        }

        private IList<T> CollectChildrenShallowDescending<T>(IList<T> result)
            where T : OxComponent
        {
            for (int i = 0; i < children.Count; ++i)
            {
                OxComponent child = children[i];
                T childAsT = child as T;
                if (childAsT != null) result.Add(childAsT);
                else child.CollectChildren(CollectionAlgorithm.ShallowDescending, result);
            }

            return result;
        }

        private IList<T> CollectChildrenDescending<T>(IList<T> result)
            where T : OxComponent
        {
            for (int i = 0; i < children.Count; ++i)
            {
                OxComponent child = children[i];
                T childAsT = child as T;
                if (childAsT != null) result.Add(childAsT);
                child.CollectChildren(CollectionAlgorithm.Descending, result);
            }

            return result;
        }

        private IList<T> CollectChildrenShallow<T>(Func<T, bool> predicate, IList<T> result)
            where T : OxComponent
        {
            for (int i = 0; i < children.Count; ++i)
            {
                T childAsT = children[i] as T;
                if (childAsT != null && predicate(childAsT)) result.Add(childAsT);
            }

            return result;
        }

        private IList<T> CollectChildrenShallowDescending<T>(Func<T, bool> predicate, IList<T> result)
            where T : OxComponent
        {
            for (int i = 0; i < children.Count; ++i)
            {
                OxComponent child = children[i];
                T childAsT = child as T;
                if (childAsT != null && predicate(childAsT)) result.Add(childAsT);
                else child.CollectChildren(CollectionAlgorithm.ShallowDescending, predicate, result);
            }

            return result;
        }

        private IList<T> CollectChildrenDescending<T>(Func<T, bool> predicate, IList<T> result)
            where T : OxComponent
        {
            for (int i = 0; i < children.Count; ++i)
            {
                OxComponent child = children[i];
                T childAsT = child as T;
                if (childAsT != null && predicate(childAsT)) result.Add(childAsT);
                child.CollectChildren(CollectionAlgorithm.Descending, predicate, result);
            }

            return result;
        }

        private void DisposeSubcomponents()
        {
            IList<IDisposable> disposables = subcomponents.Collect(new List<IDisposable>());
            for (int i = 0; i < disposables.Count; ++i) disposables[i].Dispose();
        }

        private bool IsChildParent(OxComponent child)
        {
            OxComponent walk = this.Parent;
            while (walk != null)
            {
                if (child == walk) return true;
                walk = walk.Parent;
            }
            return false;
        }

#if DEBUG
        private void engine_ComponentRemoved(OxEngine sender, OxComponent component)
        {
            System.Diagnostics.Trace.Assert(component != this, "Cannot manually remove a component from its engine.");
        }
#endif

        private readonly QueriableIdentifiables<BaseSubcomponent> subcomponents = new QueriableIdentifiables<BaseSubcomponent>(true);
        private readonly IList<OxComponent> cachedChildren = new List<OxComponent>();
        private readonly IList<OxComponent> children = new List<OxComponent>();
        private readonly IIdentifiable identifiable;
        private readonly OxEngine engine;
        private readonly Traits traits = new Traits();
        private readonly string domainName;
        private readonly bool ownedByDomain;
        private string designTimeData;
        /// <summary>May be null.</summary>
        private BaseComponentScript script;
        /// <summary>May be null.</summary>
        private OxComponent _parent;
    }
}

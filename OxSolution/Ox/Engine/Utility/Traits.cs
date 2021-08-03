using System.Linq;
using System.Collections.Generic;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Raised when an event relevant to a trait occurs.
    /// </summary>
    public delegate void TraitAction<T>(T sender, string name) where T : class;

    /// <summary>
    /// Represent a dynamic trait.
    /// </summary>
    public struct Trait
    {
        /// <summary>
        /// Create a Trait.
        /// </summary>
        /// <param name="name">See property Name.</param>
        /// <param name="value">See property Value.</param>
        public Trait(string name, object value)
        {
            OxHelper.ArgumentNullCheck(name, value);
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// The name.
        /// May be null.
        /// </summary>
        public string Name;
        /// <summary>
        /// The value.
        /// May be null.
        /// </summary>
        public object Value;
    }

    /// <summary>
    /// Represents a collection of dynamic variables.
    /// </summary>
    public class Traits
    {
        /// <summary>
        /// Raised when a trait is added.
        /// </summary>
        public event TraitAction<Traits> TraitSet;

        /// <summary>
        /// Raised when a trait is set.
        /// </summary>
        public event TraitAction<Traits> TraitAdded;

        /// <summary>
        /// Raised when a trait is removed.
        /// </summary>
        public event TraitAction<Traits> TraitRemoved;

        /// <summary>
        /// Is the specified trait attached?
        /// </summary>
        public bool Contains<T>(string name)
        {
            OxHelper.ArgumentNullCheck(name);
            object traitRef;
            return traits.TryGetValue(name, out traitRef) && traitRef is Ref<T>;
        }

        /// <summary>
        /// Add a trait at its default value.
        /// </summary>
        public void Add<T>(string name)
        {
            OxHelper.ArgumentNullCheck(name);
            DoAdd(name, default(T));
        }

        /// <summary>
        /// Remove a trait.
        /// </summary>
        public bool Remove(string name)
        {
            OxHelper.ArgumentNullCheck(name);
            bool result = traits.Remove(name);
            if (result && TraitRemoved != null) TraitRemoved(this, name);
            return result;
        }

        /// <summary>
        /// Removes all the traits.
        /// </summary>
        public void Clear()
        {
            IList<string> traitNames = traits.Keys.ToList(); // MEMORYCHURN
            traitNames.ForEach(x => Remove(x)); // MEMORYCHURN
        }

        /// <summary>
        /// Try to get a trait.
        /// </summary>
        public bool TryGet<T>(string name, out T value)
        {
            OxHelper.ArgumentNullCheck(name);
            value = default(T);
            object traitRef;
            if (!traits.TryGetValue(name, out traitRef)) return false;
            Ref<T> reference = traitRef as Ref<T>;
            if (reference == null) return false;
            reference.Get(out value);
            return true;
        }

        /// <summary>
        /// Try to get a trait, returning a default value if it's not found.
        /// </summary>
        public T TryGet<T>(string name, T defaultValue)
        {
            OxHelper.ArgumentNullCheck(name);
            T result;
            if (!TryGet(name, out result)) result = defaultValue;
            return result;
        }

        /// <summary>
        /// Get a trait.
        /// </summary>
        public T Get<T>(string name)
        {
            OxHelper.ArgumentNullCheck(name);
            T result;
            OxHelper.Cast<Ref<T>>(traits[name]).Get(out result);
            return result;
        }

        /// <summary>
        /// Set a trait to the specified value.
        /// </summary>
        public void Set<T>(string name, T value)
        {
            OxHelper.ArgumentNullCheck(name);
            object trait;
            traits.TryGetValue(name, out trait);
            if (trait == null) DoAdd(name, value);
            else DoSet(name, value, trait);
        }

        /// <summary>
        /// Collect all the object's traits.
        /// </summary>
        public IList<Trait> Collect(IList<Trait> result)
        {
            OxHelper.ArgumentNullCheck(result);
            foreach (var pair in traits) result.Add(new Trait(pair.Key, pair.Value));
            return result;
        }

        private void DoAdd<T>(string name, T value)
        {
            traits.Add(name, new Ref<T>(value));
            if (TraitAdded != null) TraitAdded(this, name);
            if (TraitSet != null) TraitSet(this, name);
        }

        private void DoSet<T>(string name, T value, object trait)
        {
            Ref<T> traitAsT = trait as Ref<T>;
            if (traitAsT == null) DoAdd(name, value);
            else
            {
                traitAsT.Set(ref value);
                if (TraitSet != null) TraitSet(this, name);
            }
        }

        private readonly IDictionary<string, object> traits = new Dictionary<string, object>();
    }
}

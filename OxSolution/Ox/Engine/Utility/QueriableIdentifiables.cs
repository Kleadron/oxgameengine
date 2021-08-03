using System;
using System.Collections.Generic;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Represents a queriable collection of identifiable items.
    /// </summary>
    /// <typeparam name="T">The type of items to store.</typeparam>
    public class QueriableIdentifiables<T> where T : class, IIdentifiable
    {
        /// <summary>
        /// Create a QueriableIdentifiables object.
        /// </summary>
        /// <param name="useFastQueries">Should FastQueriableCollection be used internally?</param>
        public QueriableIdentifiables(bool useFastQueries)
        {
            if (useFastQueries) items = new FastQueriableCollection<T>();
            else items = new QueriableCollection<T>();
        }

        /// <summary>
        /// Raised when multiple items are assigned the same name.
        /// </summary>
        public event Action<QueriableIdentifiables<T>> NameConflicted;

        /// <inheritdoc />
        public void Add(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            bool nameConflict = namedItems.ContainsKey(item.Name);
            if (nameConflict) item.Name = item.DefaultName;
            item.NameChanged += item_NameChanged;
            namedItems.Add(item.Name, item);
            items.Add(item);
            if (nameConflict && NameConflicted != null) NameConflicted(this);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            bool result = items.Remove(item);
            if (result)
            {
                namedItems.Remove(item.Name);
                item.NameChanged -= item_NameChanged;
            }
            return result;
        }

        /// <inheritdoc />
        public void Clear()
        {
            items.Collect(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i) Remove(cachedItems[i]);
            }
            cachedItems.Clear();
        }

        /// <summary>
        /// Query the existence of an item with the specified name and type.
        /// </summary>
        public bool Contains<U>(string name) where U : class
        {
            OxHelper.ArgumentNullCheck(name);
            T item;
            namedItems.TryGetValue(name, out item);
            return item as U != null;
        }

        /// <summary>
        /// Get the item of type T with the specified name.
        /// May return null.
        /// </summary>
        public U Get<U>(string name) where U : class
        {
            OxHelper.ArgumentNullCheck(name);
            try // TODO: profile this to see if the try / catch slows us down.
            {
                return OxHelper.Cast<U>(namedItems[name]);
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException("Cannot find item " + name + ".");
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException("Cannot cast item " + name + " to type " + typeof(U).Name + ".");
            }
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(IList<U> result) where U : class
        {
            return items.Collect(result);
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(Func<U, bool> predicate, IList<U> result) where U : class
        {
            return items.Collect(predicate, result);
        }

        private void item_NameChanged(IIdentifiable sender, string oldName)
        {
            OxHelper.ArgumentNullCheck(sender, oldName);
            T item = OxHelper.Cast<T>(sender);

            bool nameConflict = namedItems.ContainsKey(item.Name);
            if (nameConflict)
            {
                item.NameChanged -= item_NameChanged;
                {
                    item.Name = item.DefaultName;
                }
                item.NameChanged += item_NameChanged;
            }

            namedItems.Remove(oldName);
            try { namedItems.Add(item.Name, item); }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException(
                    "Could not make item name unique by setting its name to DefaultName.", e);
            }

            if (nameConflict && NameConflicted != null) NameConflicted(this);
        }

        private readonly IQueriableCollection<T> items;
        private readonly IDictionary<string, T> namedItems = new Dictionary<string, T>();
        private readonly IList<T> cachedItems = new List<T>();
    }
}

using System.Collections.Generic;
using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// A vanilla implementation of IQueriableCollection.
    /// </summary>
    public class QueriableCollection<T> : IQueriableCollection<T> where T : class
    {
        /// <inheritdoc />
        public void Add(T item)
        {
            // NOTE: this is an important assertion, but it slows down the application too much
            //System.Diagnostics.Trace.Assert(!items.Contains(item), "Item is already in collection!");
            items.Add(item);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            items.Clear();
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            for (int i = 0; i < items.Count; ++i)
            {
                U item = items[i] as U;
                if (item != null) result.Add(item);
            }
            return result;
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(Func<U, bool> predicate, IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            for (int i = 0; i < items.Count; ++i)
            {
                U item = items[i] as U;
                if (item != null && predicate(item)) result.Add(item);
            }
            return result;
        }

        private readonly IList<T> items = new List<T>();
    }
}

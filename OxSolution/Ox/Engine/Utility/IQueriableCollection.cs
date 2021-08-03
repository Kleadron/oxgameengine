using System.Collections.Generic;
using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Represents a collection that can be queried for items of a specified type.
    /// </summary>
    public interface IQueriableCollection<T> where T : class
    {
        /// <summary>
        /// Add an item to the collection. Item must be unique in the colleciton.
        /// </summary>
        void Add(T item);
        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        bool Remove(T item);
        /// <summary>
        /// Removes all the items from the collection.
        /// </summary>
        void Clear();
        /// <summary>
        /// Collect all items of type U.
        /// </summary>
        IList<U> Collect<U>(IList<U> result) where U : class;
        /// <summary>
        /// Collect all items of type U that satisfy a predicate.
        /// </summary>
        IList<U> Collect<U>(Func<U, bool> predicate, IList<U> result) where U : class;
    }
}

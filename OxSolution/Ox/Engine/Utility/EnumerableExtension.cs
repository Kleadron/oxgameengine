using System;
using System.Collections.Generic;
using System.Linq;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Helper methods for .NET's generic IEnumerable class.
    /// </summary>
    public static class EnumerableExtenstion
    {
        /// <summary>
        /// Iterate over an enumerable.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            OxHelper.ArgumentNullCheck(enumerable, action);
            foreach (T item in enumerable) action(item);
        }

        /// <summary>
        /// Iterate over a copy of an enumerable.
        /// </summary>
        public static void ForEachOnCopy<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            OxHelper.ArgumentNullCheck(enumerable, action);
            enumerable.ToArray().ForEach(action);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Helper methods for .NET's generic IList class.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Iterate through an IList.
        /// </summary>
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            OxHelper.ArgumentNullCheck(list, action);
            for (int i = 0; i < list.Count; ++i) action(list[i]);
        }

        /// <summary>
        /// Reverse iterate through an IList.
        /// </summary>
        public static void ForEachInReverse<T>(this IList<T> list, Action<T> action)
        {
            OxHelper.ArgumentNullCheck(list, action);
            for (int i = list.Count - 1; i > -1; --i) action(list[i]);
        }

        /// <summary>
        /// Iterate through a copy of an IList.
        /// </summary>
        public static void ForEachOnCopy<T>(this IList<T> list, Action<T> action)
        {
            OxHelper.ArgumentNullCheck(list, action);
            list.ToArray().ForEach<T>(action);
        }

        /// <summary>
        /// Reverse iterate through a copy of an IList.
        /// </summary>
        public static void ForEachInReverseOnCopy<T>(this IList<T> list, Action<T> action)
        {
            OxHelper.ArgumentNullCheck(list, action);
            list.ToArray().ForEachInReverse<T>(action);
        }

        /// <summary>
        /// Add the range.
        /// </summary>
        public static void AddRange<T>(this IList<T> list, IList<T> range)
        {
            OxHelper.ArgumentNullCheck(range, list);
            // OPTIMIZATION: try to use AddRange TODO: verify this as an optimization
            List<T> destinationAsList = list as List<T>;
            if (destinationAsList != null) destinationAsList.AddRange(range);
            else for (int i = 0; i < range.Count; ++i) list.Add(range[i]);
        }

        /// <summary>
        /// Transfer the contents of the range into the list.
        /// </summary>
        public static void TransferRange<T>(this IList<T> list, IList<T> range)
        {
            OxHelper.ArgumentNullCheck(list, range);
            list.AddRange(range);
            range.Clear();
        }

        /// <summary>
        /// RemoveUnstable does an unstable (non-order-preserving) remove from a list. This avoids
        /// the needs to scoot several items down a list in many cases.
        /// If this is faster than List.Remove is unverified.
        /// </summary>
        public static bool RemoveUnstable<T>(this List<T> list, T item)
        {
            OxHelper.ArgumentNullCheck(list, item);
            int endIndex = list.Count - 1;
            int itemIndex = list.IndexOf(item);
            if (itemIndex == -1) return false;
            if (itemIndex != endIndex) list[itemIndex] = list[endIndex];
            list.RemoveAt(endIndex);
            return true;
        }

        /// <summary>
        /// RemoveAtUnstable does an unstable (non-order-preserving) remove from a list. This
        /// avoids the needs to scoot several items down a list in many cases.
        /// If this is faster than List.Remove is unverified.
        /// </summary>
        public static void RemoveAtUnstable<T>(this IList<T> list, int index)
        {
            OxHelper.ArgumentNullCheck(list);
            int endIndex = list.Count - 1;
            if (index != endIndex) list[index] = list[endIndex];
            list.RemoveAt(endIndex);
        }
    }
}

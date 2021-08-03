using System;
using System.Collections.Generic;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// An implementation of IQueriableCollection that is optimized for look-up speed.
    /// </summary>
    public class FastQueriableCollection<T> : IQueriableCollection<T> where T : class
    {
        /// <inheritdoc />
        public void Add(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            AddItem(item, item.GetType());
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            return RemoveItem(item, item.GetType());
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var pair in itemLists) pair.Value.Clear();
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            IList<object> items;
            if (itemLists.TryGetValue(typeof(U), out items)) CollectFrom<U>(items, result);
            return result;
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(Func<U, bool> predicate, IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            IList<object> items;
            if (itemLists.TryGetValue(typeof(U), out items)) CollectFrom<U>(predicate, items, result);
            return result;
        }

        private void AddItem(T item, Type itemType)
        {
            AddItemByItsInterfaces(item, itemType);
            AddItemByItsClasses(item, itemType);
        }

        private bool RemoveItem(T item, Type itemType)
        {
            return
                RemoveItemByItsInterfaces(item, itemType) |
                RemoveItemByItsClasses(item, itemType);
        }

        private void CollectFrom<U>(IList<object> items, IList<U> result) where U : class
        {
            for (int i = 0; i < items.Count; ++i)
                result.Add(OxHelper.Cast<U>(items[i]));
        }

        private void CollectFrom<U>(Func<U, bool> predicate, IList<object> items, IList<U> result) where U : class
        {
            for (int i = 0; i < items.Count; ++i)
            {
                U item = OxHelper.Cast<U>(items[i]);
                if (predicate(item)) result.Add(item);
            }
        }

        private void AddItemByItsClasses(T item, Type itemType)
        {
            Type classType = itemType;
            do
            {
                IList<object> items;
                if (!itemLists.TryGetValue(classType, out items))
                {
                    items = new List<object>();
                    itemLists.Add(classType, items);
                }
                // NOTE: this is an important assertion, but it slows down the application too much
                //System.Diagnostics.Trace.Assert(!items.Contains(item), "Item is already in collection!");
                items.Add(item);
                classType = classType.BaseType;
            }
            while (classType != null && typeofT.IsAssignableFrom(classType));
        }

        private void AddItemByItsInterfaces(T item, Type itemType)
        {
            Type[] interfaces = GetInterfaces(itemType);
            for (int i = 0; i < interfaces.Length; ++i)
            {
                Type interface_ = interfaces[i];
                IList<object> items;
                if (!itemLists.TryGetValue(interface_, out items))
                {
                    items = new List<object>();
                    itemLists.Add(interface_, items);
                }
                // NOTE: this is an important assertion, but it slows down the application too much
                //System.Diagnostics.Trace.Assert(!items.Contains(item), "Item is already in collection!");
                items.Add(item);
            }
        }

        private bool RemoveItemByItsClasses(T item, Type itemType)
        {
            bool result = false;
            Type classType = itemType;
            do
            {
                IList<object> items;
                if (itemLists.TryGetValue(classType, out items) && items.Remove(item)) result = true;
                classType = classType.BaseType;
            }
            while (classType != null && typeofT.IsAssignableFrom(classType));
            return result;
        }

        private bool RemoveItemByItsInterfaces(T item, Type itemType)
        {
            bool result = false;
            Type[] interfaces = GetInterfaces(itemType);
            for (int i = 0; i < interfaces.Length; ++i)
            {
                IList<object> items;
                if (itemLists.TryGetValue(interfaces[i], out items) && items.Remove(item)) result = true;
            }
            return result;
        }

        private Type[] GetInterfaces(Type itemType)
        {
            Type[] result;
            if (!interfaceLists.TryGetValue(itemType, out result))
            {
                result = itemType.GetInterfaces();
                interfaceLists.Add(itemType, result);
            }
            return result;
        }

        private readonly Dictionary<Type, IList<object>> itemLists = new Dictionary<Type, IList<object>>();
        private readonly Dictionary<Type, Type[]> interfaceLists = new Dictionary<Type, Type[]>();
        private readonly Type typeofT = typeof(T);
    }
}

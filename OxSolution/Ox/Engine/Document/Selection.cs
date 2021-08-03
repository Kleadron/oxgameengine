using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// Raised when the selection has changed.
    /// </summary>
    public delegate void SelectionChanged<T>(T sender, IEnumerable oldSelection);
    /// <summary>
    /// Raised when a selection's property has changed.
    /// </summary>
    public delegate void SelectionPropertyChanged<T>(T sender, object target, string propertyName, object oldValue);

    /// <summary>
    /// An object selection.
    /// </summary>
    public class Selection : IReadIndexable<object>, ICollection<object>
    {
        /// <inheritdoc />
        public object this[int index] { get { return items[index]; } }

        /// <summary>
        /// The first selected item
        /// </summary>
        public object First { get { return items[0]; } }

        /// <summary>
        /// The first selected item, if any.
        /// May return null.
        /// </summary>
        public object FirstOrNull { get { return items.Count != 0 ? items[0] : null; } }

        /// <inheritdoc />
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// See IReadIndexable.Count.
        /// </summary>
        public int Count { get { return items.Count; } }

        /// <summary>
        /// Raised when the selection is changed.
        /// </summary>
        public event SelectionChanged<Selection> SelectionChanged;

        /// <summary>
        /// Raised when a property of the selection has changed.
        /// </summary>
        public event SelectionPropertyChanged<Selection> SelectionPropertyChanged;

        /// <inheritdoc />
        public bool Contains(object item)
        {
            OxHelper.ArgumentNullCheck(item);
            return items.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(object[] array, int arrayIndex)
        {
            OxHelper.ArgumentNullCheck(array);
            items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Get the first item casted to type T.
        /// </summary>
        public T GetFirst<T>() where T : class
        {
            return OxHelper.Cast<T>(First);
        }

        /// <summary>
        /// Get the first item casted to type T, if any.
        /// </summary>
        public T GetFirstOrNull<T>() where T : class
        {
            return OxHelper.Cast<T>(FirstOrNull);
        }

        /// <summary>
        /// Set the selection to a single item.
        /// </summary>
        public void Set(object item)
        {
            OxHelper.ArgumentNullCheck(item);
            ArgumentNotEnumerableCheck(item);
            IEnumerable oldSelection = this.ToArray();
            if (EventlessSet(item) && SelectionChanged != null) SelectionChanged(this, oldSelection);
        }

        /// <summary>
        /// Set the selection to a range of items.
        /// </summary>
        public void SetRange(IEnumerable items)
        {
            OxHelper.ArgumentNullCheck(items);
            IEnumerable oldSelection = this.ToArray();
            if (EventlessSetRange(items) && SelectionChanged != null) SelectionChanged(this, oldSelection);
        }

        /// <inheritdoc />
        public void Add(object item)
        {
            OxHelper.ArgumentNullCheck(item);
            ArgumentNotEnumerableCheck(item);
            IEnumerable oldSelection = this.ToArray();
            if (EventlessAdd(item) && SelectionChanged != null) SelectionChanged(this, oldSelection);
        }

        /// <summary>
        /// Add a range of items.
        /// </summary>
        public void AddRange(IEnumerable items)
        {
            OxHelper.ArgumentNullCheck(items);
            IEnumerable oldSelection = this.ToArray();
            if (EventlessAddRange(items) && SelectionChanged != null) SelectionChanged(this, oldSelection);
        }

        /// <inheritdoc />
        public bool Remove(object item)
        {
            OxHelper.ArgumentNullCheck(item);
            ArgumentNotEnumerableCheck(item);
            bool result = items.Contains(item);
            if (result)
            {
                IEnumerable oldSelection = this.ToArray();
                EventlessRemove(item);
                if (SelectionChanged != null) SelectionChanged(this, oldSelection);
            }
            return result;
        }

        /// <summary>
        /// Remove a range of items.
        /// </summary>
        /// <param name="items">The items to remove.</param>
        /// <returns>True if any items were removed.</returns>
        public bool RemoveRange(IEnumerable items)
        {
            OxHelper.ArgumentNullCheck(items);
            IEnumerable oldSelection = this.ToArray();
            bool result = EventlessRemoveRange(items);
            if (result && SelectionChanged != null) SelectionChanged(this, oldSelection);
            return result;
        }

        /// <inheritdoc />
        public void Clear()
        {
            RemoveRange(items.ToArray());
        }

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return OxHelper.Cast<IEnumerable>(items).GetEnumerator();
        }

        private void item_PropertyChanged(object sender, string propertyName, object oldValue)
        {
            if (SelectionPropertyChanged != null) SelectionPropertyChanged(this, sender, propertyName, oldValue);
        }

        private bool EventlessSet(object item)
        {
            object[] oldSelection = this.ToArray();
            EventlessRemoveRange(oldSelection);
            EventlessAdd(item);
            object[] newSelection = this.ToArray();
            return !Compare(oldSelection, newSelection);
        }

        private bool EventlessSetRange(IEnumerable items)
        {
            object[] oldSelection = this.ToArray();
            EventlessRemoveRange(oldSelection);
            EventlessAddRange(items);
            object[] newSelection = this.ToArray();
            return !Compare(oldSelection, newSelection);
        }

        private bool EventlessAdd(object item)
        {
            bool result = !items.Contains(item);
            if (result)
            {
                InductSelected(item);
                items.Add(item);
            }
            return result;
        }

        private bool EventlessAddRange(IEnumerable items)
        {
            bool result = false;
            foreach (object item in items)
                if (EventlessAdd(item))
                    result = true;
            return result;
        }

        private bool EventlessRemove(object item)
        {
            bool result = items.Remove(item);
            ExpelSelected(item);
            return result;
        }

        private bool EventlessRemoveRange(IEnumerable items)
        {
            bool result = false;
            foreach (object item in items)
                if (EventlessRemove(item))
                    result = true;
            return result;
        }

        private void InductSelected(object selected)
        {
            ItemToken selectedItem = selected as ItemToken;
            if (selectedItem != null) InductItem(selectedItem);
        }

        private void ExpelSelected(object selected)
        {
            ItemToken selectedItem = selected as ItemToken;
            if (selectedItem != null) ExpelItem(selectedItem);
        }

        private void InductItem(ItemToken item)
        {
            item.PropertyChanged += item_PropertyChanged;
        }

        private void ExpelItem(ItemToken item)
        {
            item.PropertyChanged -= item_PropertyChanged;
        }

        private static void ArgumentNotEnumerableCheck(object item)
        {
            if (item as IEnumerable != null) throw new ArgumentException(
                "Use the method with the IEnumerable parameter for collections");
        }

        // TODO: try to find a library method to do this!
        private static bool Compare(object[] first, object[] second)
        {
            int length = first.Length;
            if (length != second.Length) return false;
            for (int i = 0; i < length; ++i)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        private readonly List<object> items = new List<object>();
    }
}

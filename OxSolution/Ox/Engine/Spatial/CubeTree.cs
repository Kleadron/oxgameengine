using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine.Spatial
{
    /// <summary>
    /// Represents a recursive cube-based tree structure that organizes items of type T.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the tree.</typeparam>
    public abstract class CubeTree<T> : Disposable, ISpatialContainer<T>
        where T : TransformableComponent
    {
        /// <summary>
        /// Create a CubeTree.
        /// </summary>
        /// <param name="position">The position of the tree.</param>
        /// <param name="scale">The scale of the tree.</param>
        /// <param name="subdivisions">The number of times the tree is subdivided.</param>
        /// <param name="createCastingCollection">
        /// Factory delegate for creating the internal casting collection.</param>
        public CubeTree(Vector3 position, Vector3 scale, int subdivisions,
            CreateCastingCollection<T> createCastingCollection)
        {
            unboundItems = createCastingCollection();
            items = createCastingCollection();
            root = CreateRootNodeHook(position, scale, createCastingCollection);
            root.Subdivide(subdivisions);
        }

        /// <summary>
        /// The bounding box that encloses the octree.
        /// </summary>
        public BoundingBox BoundingBox { get { return root.BoundingBox; } }

        /// <summary>
        /// The position of the octree.
        /// </summary>
        public Vector3 Position { get { return root.Position; } }

        /// <summary>
        /// The scale of the octree.
        /// </summary>
        public Vector3 Scale { get { return root.Scale; } }

        /// <inheritdoc />
        public void Add(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            item.BoundsChanged += item_BoundsChanged;
            items.Add(item);
            if (!root.TryAdd(item)) unboundItems.Add(item);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            bool result = items.Remove(item);
            if (result)
            {
                if (!unboundItems.Remove(item)) root.Remove(item);
                item.BoundsChanged -= item_BoundsChanged;
            }
            return result;
        }

        /// <inheritdoc />
        public void Clear()
        {
            items.Collect(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i)
                    cachedItems[i].BoundsChanged -= item_BoundsChanged;
            }
            cachedItems.Clear();

            items.Clear();
            root.Clear();
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(BoundingFrustum bounds, IList<U> result)
            where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            unboundItems.Collect(result);
            root.CollectItems(bounds, result);
            return result;
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(BoundingBox bounds, IList<U> result)
            where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            if (bounds.AllEncompassing()) items.Collect(result); // OPTIMIZATION
            else
            {
                unboundItems.Collect(result);
                root.CollectItems(bounds, result);
            }
            return result;
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(IList<U> result)
            where U : class
        {
            return items.Collect(result);
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(Func<U, bool> predicate, BoundingFrustum bounds, IList<U> result)
            where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            unboundItems.Collect(predicate, result);
            root.CollectItems(predicate, bounds, result);
            return result;
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(Func<U, bool> predicate, BoundingBox bounds, IList<U> result)
            where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            if (bounds.AllEncompassing()) items.Collect(predicate, result); // OPTIMIZATION
            else
            {
                unboundItems.Collect(predicate, result);
                root.CollectItems(predicate, bounds, result);
            }
            return result;
        }

        /// <inheritdoc />
        public IList<U> Collect<U>(Func<U, bool> predicate, IList<U> result)
            where U : class
        {
            return items.Collect(predicate, result);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) Clear();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handle creating a root node.
        /// </summary>
        /// <param name="position">The position of the created the node.</param>
        /// <param name="scale">The scale of the created node.</param>
        /// <param name="createCastingCollection">The casting collection factory delegate.</param>
        /// <returns>The created node.</returns>
        protected abstract CubeTreeNode<T> CreateRootNodeHook(
            Vector3 position, Vector3 scale, CreateCastingCollection<T> createCastingCollection);

        private void item_BoundsChanged(TransformableComponent sender)
        {
            OxHelper.ArgumentNullCheck(sender);
            System.Diagnostics.Debug.Assert(sender as T != null, "Logic error.");
            T item = OxHelper.Cast<T>(sender);
            CubeTreeNode<T> node = item.SpatialNodeParent as CubeTreeNode<T>;
            if (node == null)
            {
                if (!item.Boundless &&
                    root.BoundingBox.Contains(item.BoundingBoxWorld) == ContainmentType.Contains)
                    ReorganizeItem(item);
            }
            else
            {
                if (item.Boundless ||
                    node.BoundingBox.Contains(item.BoundingBoxWorld) != ContainmentType.Contains)
                    ReorganizeItem(item);
            }
        }

        private void ReorganizeItem(T item)
        {
            if (!root.Remove(item)) unboundItems.Remove(item);
            if (!root.TryAdd(item)) unboundItems.Add(item);
        }

        private readonly IQueriableCollection<T> unboundItems;
        private readonly IQueriableCollection<T> items;
        private readonly CubeTreeNode<T> root;
        private readonly IList<T> cachedItems = new List<T>();
    }
}

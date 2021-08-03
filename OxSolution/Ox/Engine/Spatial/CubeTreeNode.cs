using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace Ox.Engine.Spatial
{
    /// <summary>
    /// Represents a node on an cube tree.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the tree.</typeparam>
    public abstract class CubeTreeNode<T>
        where T : TransformableComponent
    {
        /// <summary>
        /// Create a CubeTreeNode.
        /// </summary>
        /// <param name="position">The position of the created the node.</param>
        /// <param name="scale">The scale of the created node.</param>
        /// <param name="createCastingCollection">The casting collection factory delegate.</param>
        public CubeTreeNode(Vector3 position, Vector3 scale, CreateCastingCollection<T> createCastingCollection)
        {
            this.createCastingCollection = createCastingCollection;
            this.position = position;
            this.scale = scale;
            items = createCastingCollection();
            boundingBox = new BoundingBox(position, position + scale);
        }

        /// <summary>
        /// The bounding box that represents the space inside the node.
        /// </summary>
        public BoundingBox BoundingBox { get { return boundingBox; } }

        /// <summary>
        /// The position of the node.
        /// </summary>
        public Vector3 Position { get { return position; } }

        /// <summary>
        /// The scale of the node.
        /// </summary>
        public Vector3 Scale { get { return scale; } }

        /// <summary>
        /// Subdivide the node a number of times.
        /// </summary>
        /// <param name="iterations">The number of times to subdivide the node.</param>
        public void Subdivide(int iterations)
        {
            if (iterations == 0) return;
            CreateChildren(ref iterations);
            if (iterations == 0) return;
            SubdivideChildren(iterations);
        }

        /// <summary>
        /// Try to add an item to the node.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the item was added to the node.</returns>
        public bool TryAdd(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            if (item.Boundless || boundingBox.Contains(item.BoundingBoxWorld) == ContainmentType.Contains) return false;
            if (!TryAddToChildren(item)) Add(item);
            return true;
        }

        /// <summary>
        /// Remove an item from the node.
        /// </summary>
        public bool Remove(T item)
        {
            OxHelper.ArgumentNullCheck(item);
            CubeTreeNode<T> node = item.SpatialNodeParent as CubeTreeNode<T>;
            if (node == null) return false;
            return node.RemoveNonRecursively(item);
        }

        /// <summary>
        /// Non-recursively remove an item from the node.
        /// </summary>
        public bool RemoveNonRecursively(T item)
        {
            bool result = items.Remove(item);
            if (result) item.SpatialNodeParent = null;
            return result;
        }

        /// <summary>
        /// Remove all the items in the node.
        /// </summary>
        public void Clear()
        {
            items.Collect<T>(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i) cachedItems[i].SpatialNodeParent = null;
            }
            cachedItems.Clear();

            items.Clear();
            ClearChildren();
        }

        /// <summary>
        /// Collect all items of type U.
        /// </summary>
        public IList<U> CollectItems<U>(BoundingFrustum bounds, IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            return
                CollectItemsFromChildren(
                CollectItemsFromSelf(bounds, result));
        }

        /// <summary>
        /// Collect all items of type U that satisfy a predicate.
        /// </summary>
        public IList<U> CollectItems<U>(BoundingBox bounds, IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            return
                CollectItemsFromChildren(
                CollectItemsFromSelf(bounds, result));
        }

        /// <summary>
        /// Collect all items of type U inside the specified bounds.
        /// </summary>
        public IList<U> CollectItems<U>(IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            return
                CollectItemsFromChildren(
                CollectItemsFromSelf<U>(result));
        }

        /// <summary>
        /// Collect all items of type U inside the specified bounds.
        /// </summary>
        public IList<U> CollectItems<U>(Func<U, bool> predicate, BoundingFrustum bounds, IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            return
                CollectItemsFromChildren(predicate,
                CollectItemsFromSelf(predicate, bounds, result));
        }

        /// <summary>
        /// Collect all items of type U inside the specified bounds that satisfy a predicate.
        /// </summary>
        public IList<U> CollectItems<U>(Func<U, bool> predicate, BoundingBox bounds, IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            return
                CollectItemsFromChildren(predicate,
                CollectItemsFromSelf(predicate, bounds, result));
        }

        /// <summary>
        /// Collect all items of type U inside the specified bounds that satisfy a predicate.
        /// </summary>
        public IList<U> CollectItems<U>(Func<U, bool> predicate, IList<U> result) where U : class
        {
            OxHelper.ArgumentNullCheck(result);
            return
                CollectItemsFromChildren(predicate,
                CollectItemsFromSelf<U>(predicate, result));
        }

        /// <summary>
        /// Handle creating the child nodes.
        /// </summary>
        /// <param name="createCastingCollection">The casting collection factory delegate.</param>
        /// <returns>The created nodes.</returns>
        protected abstract CubeTreeNode<T>[] CreateChildrenHook(CreateCastingCollection<T> createCastingCollection);

        private void Add(T item)
        {
            System.Diagnostics.Debug.Assert(item.SpatialNodeParent == null,
                "Object should not already have a spatial node parent while being added to a spatial node.");
            item.SpatialNodeParent = this;
            items.Add(item);
        }

        private IList<U> CollectItemsFromSelf<U>(IList<U> result) where U : class
        {
            return items.Collect(result);
        }

        private IList<U> CollectItemsFromSelf<U>(BoundingFrustum bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(result)
                .FilterBounds(bounds, originalListCount);
        }

        private IList<U> CollectItemsFromSelf<U>(BoundingBox bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(result)
                .FilterBounds(bounds, originalListCount);
        }

        private IList<U> CollectItemsFromSelf<U>(Func<U, bool> predicate, IList<U> result) where U : class
        {
            return items.Collect(predicate, result);
        }

        private IList<U> CollectItemsFromSelf<U>(Func<U, bool> predicate, BoundingFrustum bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(predicate, result)
                .FilterBounds(bounds, originalListCount);
        }

        private IList<U> CollectItemsFromSelf<U>(Func<U, bool> predicate, BoundingBox bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(predicate, result)
                .FilterBounds(bounds, originalListCount);
        }

        private void CreateChildren(ref int iterations)
        {
            if (children != null) return;
            children = CreateChildrenHook(createCastingCollection);
            --iterations;
        }

        private bool TryAddToChildren(T item)
        {
            if (children == null) return false;
            for (int i = 0; i < children.Length; ++i)
                if (children[i].TryAdd(item))
                    return true;
            return false;
        }

        private void SubdivideChildren(int iterations)
        {
            if (children != null)
                for (int i = 0; i < children.Length; ++i)
                    children[i].Subdivide(iterations);
        }

        private IList<U> CollectItemsFromChildren<U>(IList<U> result) where U : class
        {
            if (children != null)
                for (int i = 0; i < children.Length; ++i)
                    children[i].CollectItems(result);
            return result;
        }

        private IList<U> CollectItemsFromChildren<U>(Func<U, bool> predicate, IList<U> result) where U : class
        {
            if (children != null)
                for (int i = 0; i < children.Length; ++i)
                    children[i].CollectItems(predicate, result);
            return result;
        }

        private void ClearChildren()
        {
            if (children != null)
                for (int i = 0; i < children.Length; ++i)
                    children[i].Clear();
        }

        private readonly CreateCastingCollection<T> createCastingCollection;
        private readonly IQueriableCollection<T> items;
        private readonly IList<T> cachedItems = new List<T>();
        private readonly BoundingBox boundingBox;
        private readonly Vector3 position;
        private readonly Vector3 scale;
        /// <summary>May be null.</summary>
        private CubeTreeNode<T>[] children;
    }
}

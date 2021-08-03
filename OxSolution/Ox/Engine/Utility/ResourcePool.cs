using System.Collections.Generic;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Raised when a resource needs to be created.
    /// </summary>
    public delegate T CreateResource<T>() where T : class;
    /// <summary>
    /// Raised when a resource needs to be cleaned up (disposed, etc).
    /// </summary>
    public delegate void CleanupResource<T>(T resource) where T : class;

    /// <summary>
    /// Keeps a certain number of resources available to avoid heavily allocating and deallocating.
    /// </summary>
    public class ResourcePool<T> : Disposable where T : class
    {
        /// <summary>
        /// Create a ResourcePool.
        /// </summary>
        /// <param name="size">See property Size.</param>
        /// <param name="createResource">
        /// The delegate called to create the resources needed to fill the pool.</param>
        /// <param name="cleanupResource">The delegate called to cleanup each resource.</param>
        public ResourcePool(int size, CreateResource<T> createResource, CleanupResource<T> cleanupResource)
        {
            OxHelper.ArgumentNullCheck(createResource, cleanupResource);
            this.cleanupResource = cleanupResource;
            this.size = size;
            CreateResources(size, createResource, inactiveResources);
        }

        /// <summary>
        /// The amount of resources that are currently allocated from the pool.
        /// </summary>
        public int ActiveCount { get { return activeResources.Count; } }

        /// <summary>
        /// The maximum number of resources that can be allocated from the pool.
        /// </summary>
        public int Size { get { return size; } }

        /// <summary>
        /// Attempt to allocate a resource from the pool.
        /// May return null.
        /// </summary>
        /// <returns>A resource on success, null otherwise.</returns>
        public T Allocate()
        {
            if (inactiveResources.Count == 0) return null;
            T result = inactiveResources.Pop();
            activeResources.Add(result);
            return result;
        }

        /// <summary>
        /// Deallocate a resource.
        /// </summary>
        public void Free(T resource)
        {
            OxHelper.ArgumentNullCheck(resource);
            bool removeSuccessful = activeResources.RemoveUnstable(resource);
            if (removeSuccessful) inactiveResources.Push(resource);
        }

        /// <summary>
        /// Deallocate all the resources.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < activeResources.Count; ++i) inactiveResources.Push(activeResources[i]);
            activeResources.Clear();
        }

        /// <summary>
        /// Collect all the active items.
        /// </summary>
        public IList<T> CollectActiveItems(IList<T> result)
        {
            OxHelper.ArgumentNullCheck(result);
            for (int i = 0; i < activeResources.Count; ++i) result.Add(activeResources[i]);
            return result;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) CleanupResources();
            base.Dispose(disposing);
        }

        private void CleanupResources()
        {
            while (inactiveResources.Count != 0) cleanupResource(inactiveResources.Pop());
        }

        private void CreateResources(int size, CreateResource<T> createResource, Stack<T> destination)
        {
            for (int i = 0; i < size; ++i) destination.Push(createResource());
        }

        private readonly CleanupResource<T> cleanupResource;
        private readonly Stack<T> inactiveResources = new Stack<T>();
        private readonly List<T> activeResources = new List<T>();
        private readonly int size;
    }
}

using System.Collections;
using System.Collections.Generic;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// A "first-in, first-out" resource pool.
    /// </summary>
    public class FifoPool<T> : Disposable where T : class
    {
        /// <summary>
        /// Create a FifoPool.
        /// </summary>
        /// <param name="size">See property Size.</param>
        /// <param name="createResource">
        /// The delegate called to create the resources needed to fill the pool.
        /// </param>
        /// <param name="cleanupResource">The delegate called to cleanup each resource.</param>
        public FifoPool(int size, CreateResource<T> createResource, CleanupResource<T> cleanupResource)
        {
            OxHelper.ArgumentNullCheck(createResource, cleanupResource);
            this.size = size;
            this.cleanupResource = cleanupResource;
            resources = CreateResources(size, createResource);
        }

        /// <summary>
        /// The amount of resources that are currently allocated from the pool.
        /// </summary>
        public int ActiveCount
        {
            get
            {
                int result = end - begin;
                if (result < 0) result = resources.Count - begin + end;
                return result;
            }
        }

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
            if (resources.Count == 0) return null;
            int newEnd = end + 1;                
            if (newEnd == resources.Count) newEnd = 0;
            if (newEnd == begin) return null;
            T result = resources[end];
            end = newEnd;
            return result;
        }

        /// <summary>
        /// Deallocate the first outstanding resource.
        /// </summary>
        public void Free()
        {
            if (begin == end)
            {
                // TODO: I dummied this out so that Ox.Engine.Test could be run silently. Enable this
                // when a way to keep assertions in an nunit test silent is found.
                /*System.Diagnostics.Trace.Fail(
                    "Tried to free more resources than have been allocated!");*/
            }
            else
            {
                int newBegin = begin + 1;
                if (newBegin == resources.Count) newBegin = 0;
                if (newBegin != end) begin = newBegin;
            }
        }

        /// <summary>
        /// Deallocate all the outstanding resources.
        /// </summary>
        public void Clear()
        {
            begin = end;
        }

        /// <summary>
        /// Collect all the active items.
        /// </summary>
        public IList<T> CollectActiveItems(IList<T> result)
        {
            OxHelper.ArgumentNullCheck(result);
            int i = begin;
            while (i != end)
            {
                result.Add(resources[i]);
                ++i;
                if (i == resources.Count) i = 0;
            }
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
            for (int i = 0; i < resources.Count; ++i) cleanupResource(resources[i]);
        }

        private static IList<T> CreateResources(int size, CreateResource<T> createResource)
        {
            IList<T> result = new List<T>();
            for (int i = 0; i < size; ++i) result.Add(createResource());
            return result;
        }

        private readonly CleanupResource<T> cleanupResource;
        private readonly IList<T> resources = new List<T>();
        private readonly int size;
        private int begin;
        private int end;
    }
}

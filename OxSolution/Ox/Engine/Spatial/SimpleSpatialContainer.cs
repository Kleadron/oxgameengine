using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine.Spatial
{
    /// <summary>
    /// A simple 3D container that stores items without spatial organization.
    /// </summary>
    /// <typeparam name="T">The type of items contained.</typeparam>
    public class SimpleSpatialContainer<T> : Disposable, ISpatialContainer<T>
        where T : TransformableComponent
    {
        public SimpleSpatialContainer(CreateCastingCollection<T> createCastingCollection)
        {
            items = createCastingCollection();
        }

        public void Add(T item)
        {
            items.Add(item);
        }

        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public IList<U> Collect<U>(BoundingFrustum bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(result)
                .FilterBounds(bounds, originalListCount);
        }

        public IList<U> Collect<U>(BoundingBox bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(result)
                .FilterBounds(bounds, originalListCount);
        }

        public IList<U> Collect<U>(IList<U> result) where U : class
        {
            return items.Collect(result);
        }

        public IList<U> Collect<U>(Func<U, bool> predicate, BoundingFrustum bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(predicate, result)
                .FilterBounds(bounds, originalListCount);
        }

        public IList<U> Collect<U>(Func<U, bool> predicate, BoundingBox bounds, IList<U> result) where U : class
        {
            int originalListCount = result.Count;
            return items
                .Collect<U>(predicate, result)
                .FilterBounds(bounds, originalListCount);
        }

        public IList<U> Collect<U>(Func<U, bool> predicate, IList<U> result) where U : class
        {
            return items.Collect(predicate, result);
        }

        private readonly IQueriableCollection<T> items;
    }
}

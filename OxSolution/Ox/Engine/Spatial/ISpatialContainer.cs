using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace Ox.Engine.Spatial
{
    /// <summary>
    /// Creates a casting collection.
    /// </summary>
    public delegate IQueriableCollection<T> CreateCastingCollection<T>() where T : class;

    /// <summary>
    /// Represents a 3D container for transformables.
    /// </summary>
    /// <typeparam name="T">The type of 3D items contained.</typeparam>
    public interface ISpatialContainer<T> : IQueriableCollection<T>, IDisposable
        where T : TransformableComponent
    {
        /// <summary>
        /// Collect all items of type U in the specified bounds.
        /// </summary>
        IList<U> Collect<U>(BoundingFrustum bounds, IList<U> result) where U : class;
        /// <summary>
        /// Collect all items of type U in the specified bounds.
        /// </summary>
        IList<U> Collect<U>(BoundingBox bounds, IList<U> result) where U : class;
        /// <summary>
        /// Collect all items of type U in the specified bounds that satisfy a predicate.
        /// </summary>
        IList<U> Collect<U>(Func<U, bool> predicate, BoundingFrustum bounds, IList<U> result) where U : class;
        /// <summary>
        /// Collect all items of type U in the specified bounds that satisfy a predicate.
        /// </summary>
        IList<U> Collect<U>(Func<U, bool> predicate, BoundingBox bounds, IList<U> result) where U : class;
    }
}

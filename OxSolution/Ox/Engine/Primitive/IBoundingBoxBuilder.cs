using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// A builder for hierarchically-transformed bounding boxes.
    /// </summary>
    public interface IBoundingBoxBuilder
    {
        /// <summary>
        /// The bounding box before transformation.
        /// </summary>
        BoundingBox BoundingBoxSource { get; }
        /// <summary>
        /// The bounding box in local space.
        /// </summary>
        BoundingBox BoundingBoxLocal { get; }
        /// <summary>
        /// The bounding box in world space.
        /// </summary>
        BoundingBox BoundingBoxWorld { get; }
        /// <summary>
        /// Raised when the bounding box in world space is changed.
        /// </summary>
        event Action<IBoundingBoxBuilder> BoundingBoxWorldChanged;
        /// <summary>
        /// Try to set one of the bounding boxes directly. Which one depends on implementation.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <returns>True if the bounding box was set.</returns>
        bool TrySetBoundingBox(BoundingBox boundingBox);
        /// <summary>
        /// Get the transform in local space applied to the source bounding box.
        /// </summary>
        void GetTransformLocal(out Matrix transform);
        /// <summary>
        /// Set the transform in local space applied to the source bounding box.
        /// </summary>
        void SetTransformLocal(ref Matrix transform);
        /// <summary>
        /// Get the transform in world space applied to the source bounding box.
        /// </summary>
        void GetTransformWorld(out Matrix transform);
        /// <summary>
        /// Set the transform in world space applied to the source bounding box.
        /// </summary>
        void SetTransformWorld(ref Matrix transform);
    }
}

using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Represents a tranformable primitive.
    /// </summary>
    public interface IPrimitive
    {
        /// <summary>
        /// The bounding box that contains the transformed primitive.
        /// </summary>
        BoundingBox BoundingBox { get; }
        /// <summary>
        /// The area occupied by the primitive.
        /// </summary>
        float Area { get; }
        /// <summary>
        /// Raised when the primitive's bounding box has changed.
        /// </summary>
        event Action<IPrimitive> BoundingBoxChanged;
        /// <summary>
        /// Clone the primitive.
        /// </summary>
        IPrimitive Clone();
        /// <summary>
        /// Try to copy the source.
        /// </summary>
        /// <returns>True if the copy was successful.</returns>
        bool TryCopy(IPrimitive source);
        /// <summary>
        /// Translate the primitive.
        /// </summary>
        void ApplyTranslation(Vector3 translation);
        /// <summary>
        /// Orient the primitive.
        /// </summary>
        void ApplyOrientation(Matrix orientation);
        /// <summary>
        /// Transform the primitive.
        /// </summary>
        void ApplyTransformation(ref Matrix transform);
    }
}

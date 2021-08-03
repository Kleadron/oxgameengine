using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Represents an axis-aligned box.
    /// </summary>
    [TypeConverterAttribute(typeof(BoxConverter))]
    public struct Box : IEquatable<Box>
    {
        /// <summary>
        /// Create a Box.
        /// </summary>
        /// <param name="center">See property Center.</param>
        /// <param name="extent">See property Extent.</param>
        public Box(Vector3 center, Vector3 extent)
        {
            this.Center = center;
            this.Extent = extent;
        }

        /// <summary>
        /// The min of the box.
        /// </summary>
        public Vector3 Min { get { return Center - Extent; } }

        /// <summary>
        /// The max of the box.
        /// </summary>
        public Vector3 Max { get { return Center + Extent; } }

        /// <summary>
        /// Create a box from a BoundingBox.
        /// </summary>
        public Box(BoundingBox boundingBox)
        {
            this.Center = boundingBox.GetCenter();
            this.Extent = boundingBox.GetExtent();
        }

        /// <inheritdoc />
        public bool Equals(Box other)
        {
            return Center == other.Center && Extent == other.Extent;
        }

        /// <summary>
        /// The center of the box.
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// The extent of the box.
        /// </summary>
        public Vector3 Extent;
    }
}

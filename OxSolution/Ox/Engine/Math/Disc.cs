using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Represents a disc.
    /// </summary>
    public struct Disc : IEquatable<Disc>
    {
        /// <summary>
        /// Create a disc.
        /// </summary>
        /// <param name="center">See property Center.</param>
        /// <param name="edgePoint1">See property EdgePoint1.</param>
        /// <param name="edgePoint2">See property EdgePoint2.</param>
        public Disc(Vector3 center, Vector3 edgePoint1, Vector3 edgePoint2)
        {
            this.Center = center;
            this.EdgePoint1 = edgePoint1;
            this.EdgePoint2 = edgePoint2;
        }

        /// <summary>
        /// The normal of the disc.
        /// </summary>
        public Vector3 Normal
        {
            get { return Vector3.Normalize(Vector3.Cross(EdgePoint1 - Center, EdgePoint2 - Center)); }
        }

        /// <summary>
        /// The radius of the disc.
        /// </summary>
        public float Radius { get { return (EdgePoint1 - Center).Length(); } }

        /// <summary>
        /// The diameter of the disc.
        /// </summary>
        public float Diameter { get { return Radius * 2; } }

        /// <inheritdoc />
        public bool Equals(Disc other)
        {
            return
                Center == other.Center &&
                EdgePoint1 == other.EdgePoint1 &&
                EdgePoint2 == other.EdgePoint2;
        }

        /// <summary>
        /// The center of the disc.
        /// </summary>
        public Vector3 Center;
        /// <summary>
        /// A point on the disc's edge.
        /// </summary>
        public Vector3 EdgePoint1;
        /// <summary>
        /// Another point on the disc's edge.
        /// </summary>
        public Vector3 EdgePoint2;
    }
}

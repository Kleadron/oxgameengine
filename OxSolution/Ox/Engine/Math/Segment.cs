using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Represents a line segment.
    /// </summary>
    public struct Segment : IEquatable<Segment>
    {
        /// <summary>
        /// Create a Segment.
        /// </summary>
        /// <param name="start">See property Start.</param>
        /// <param name="end">See property End.</param>
        public Segment(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// The direction of the line segment (normalized).
        /// </summary>
        public Vector3 Direction
        {
            get { return Vector3.Normalize(End - Start); }
        }

        /// <inheritdoc />
        public bool Equals(Segment other)
        {
            return Start == other.Start && End == other.End;
        }

        /// <summary>
        /// The first point on the line segment.
        /// </summary>
        public Vector3 Start;
        /// <summary>
        /// The second point on the line segment.
        /// </summary>
        public Vector3 End;
    }
}

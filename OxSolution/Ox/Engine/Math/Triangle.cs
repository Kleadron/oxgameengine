using Microsoft.Xna.Framework;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Represents a triangle.
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// Create a Triangle.
        /// </summary>
        /// <param name="a">See property A.</param>
        /// <param name="b">See property B.</param>
        /// <param name="c">See property C.</param>
        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        /// <summary>
        /// The normal of the triangle.
        /// </summary>
        public Vector3 Normal
        {
            get { return Vector3.Normalize(Vector3.Cross(B - A, C - B)); }
        }

        /// <summary>
        /// The first point on (and the origin of) the triangle.
        /// </summary>
        public Vector3 A;
        /// <summary>
        /// The second point on the triangle.
        /// </summary>
        public Vector3 B;
        /// <summary>
        /// The third point on the triangle.
        /// </summary>
        public Vector3 C;
    }
}

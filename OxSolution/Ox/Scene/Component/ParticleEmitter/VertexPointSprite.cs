using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Implements the data structure for a point sprite vertex.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexPointSprite
    {
        /// <summary>
        /// Create a VertexPointSprite.
        /// </summary>
        /// <param name="position">See field position.</param>
        /// <param name="pointSize">See field pointSize.</param>
        /// <param name="color">See field color.</param>
        public VertexPointSprite(Vector3 position, float pointSize, Color color)
        {
            Position = position;
            PointSize = pointSize;
            Color = color;
        }

        /// <summary>
        /// The elements that compose the vertex.
        /// </summary>
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.PointSize, 0),
            new VertexElement(0, 16, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0),
        };
        /// <summary>
        /// The size of the vertex in bytes.
        /// </summary>
        public const int SizeInBytes = 20;
        
        [FieldOffset(0)]
        public Vector3 Position;
        [FieldOffset(12)]
        public float PointSize;
        [FieldOffset(16)]
        public Color Color;
    }
}

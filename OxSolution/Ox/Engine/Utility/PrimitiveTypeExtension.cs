using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Helper methods for operations on geometry.
    /// </summary>
    public static class PrimitiveTypeExtension
    {
        /// <summary>
        /// Get the number of primitives inside a number of vertices.
        /// </summary>
        public static int GetPrimitiveCount(this PrimitiveType type, int vertexCount)
        {
            checked
            {
                switch (type)
                {
                    case PrimitiveType.LineList: return vertexCount / 2;
                    case PrimitiveType.LineStrip: return vertexCount - 1;
                    case PrimitiveType.PointList: return vertexCount;
                    case PrimitiveType.TriangleFan: return vertexCount - 2;
                    case PrimitiveType.TriangleList: return vertexCount / 3;
                    case PrimitiveType.TriangleStrip: return vertexCount - 2;
                    default: throw InvalidPrimitiveType(type);
                }
            }
        }

        private static ArgumentException InvalidPrimitiveType(PrimitiveType type)
        {
            return new ArgumentException("No valid primitive count for primitive type " + type.ToString() + ".");
        }
    }
}

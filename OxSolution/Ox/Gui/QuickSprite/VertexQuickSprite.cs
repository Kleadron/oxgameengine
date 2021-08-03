using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace Ox.Gui.QuickSpriteNamespace
{
    /// <summary>
    /// Implements the data structure for a quick sprite vertex.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexQuickSprite
    {
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0),
            new VertexElement(0, 16, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 1),
            new VertexElement(0, 20, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
        };
        public const int SizeInBytes = 24;

        [FieldOffset(0)]
        public Vector3 Position;
        [FieldOffset(12)]
        public Color Color;
        [FieldOffset(16)]
        public Color EffectColor;
        [FieldOffset(20)]
        public Vector2 Texture;
    }
}

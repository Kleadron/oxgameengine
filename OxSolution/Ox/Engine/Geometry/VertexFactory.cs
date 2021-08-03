namespace Ox.Engine.GeometryNamespace
{
    /// <summary>
    /// Creates collections of vertices.
    /// </summary>
    public class VertexFactory
    {
        /// <summary>
        /// Create vertices of a specified format.
        /// May return null.
        /// </summary>
        public IVertices CreateVertices(string vertexFormat, int vertexCount)
        {
            return CreateVerticesHook(vertexFormat, vertexCount);
        }

        /// <summary>
        /// Handle creating vertices of a specified format.
        /// May return null.
        /// </summary>
        protected virtual IVertices CreateVerticesHook(string vertexFormat, int vertexCount)
        {
            switch (vertexFormat)
            {
                case "PositionNormalTexture": return new VerticesPositionNormalTexture(vertexCount);
                case "PositionNormalTextureBinormalTangent": return new VerticesPositionNormalTextureBinormalTangent(vertexCount);
                default: return null;
            }
        }
    }
}

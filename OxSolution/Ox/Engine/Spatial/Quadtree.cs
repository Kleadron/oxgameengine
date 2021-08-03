using Microsoft.Xna.Framework;
using Ox.Engine.Component;

namespace Ox.Engine.Spatial
{
    /// <summary>
    /// Implements a quadtree node.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the Quadtree.</typeparam>
    public class QuadtreeNode<T> : CubeTreeNode<T> where T : TransformableComponent
    {
        /// <summary>
        /// Create a QuadtreeNode.
        /// </summary>
        /// <param name="position">The position of the created the node.</param>
        /// <param name="scale">The scale of the created node.</param>
        /// <param name="createCastingCollection">The casting collection factory delegate.</param>
        public QuadtreeNode(Vector3 position, Vector3 scale, CreateCastingCollection<T> createCastingCollection)
            : base(position, scale, createCastingCollection) { }

        /// <inheritdoc />
        protected override CubeTreeNode<T>[] CreateChildrenHook(CreateCastingCollection<T> createCastingCollection)
        {
            CubeTreeNode<T>[] result = new CubeTreeNode<T>[4];            
            Vector3 halfScale = new Vector3(Scale.X * 0.5f, Scale.Y, Scale.Z * 0.5f);
            result[0] = new QuadtreeNode<T>(Position + new Vector3(0, 0, 0) * halfScale, halfScale, createCastingCollection);
            result[1] = new QuadtreeNode<T>(Position + new Vector3(1, 0, 0) * halfScale, halfScale, createCastingCollection);
            result[2] = new QuadtreeNode<T>(Position + new Vector3(1, 0, 1) * halfScale, halfScale, createCastingCollection);
            result[3] = new QuadtreeNode<T>(Position + new Vector3(0, 0, 1) * halfScale, halfScale, createCastingCollection);
            return result;
        }
    }

    /// <summary>
    /// Implements a Quadtree.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the Quadtree.</typeparam>
    public class Quadtree<T> : CubeTree<T> where T : TransformableComponent
    {
        /// <summary>
        /// Create an Quadtree.
        /// </summary>
        /// <param name="position">The position of the tree.</param>
        /// <param name="scale">The scale of the tree.</param>
        /// <param name="subdivisions">The number of times the tree is subdivided.</param>
        /// <param name="createCastingCollection">
        /// Factory delegate for creating the internal casting collection.</param>
        public Quadtree(Vector3 position, Vector3 scale, int subdivisions,
            CreateCastingCollection<T> createCastingCollection)
            : base(position, scale, subdivisions, createCastingCollection) { }

        /// <inheritdoc />
        protected override CubeTreeNode<T> CreateRootNodeHook(Vector3 position, Vector3 scale,
            CreateCastingCollection<T> createCastingCollection)
        {
            return new QuadtreeNode<T>(position, scale, createCastingCollection);
        }
    }
}

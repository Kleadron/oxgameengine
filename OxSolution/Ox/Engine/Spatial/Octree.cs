using Microsoft.Xna.Framework;
using Ox.Engine.Component;

namespace Ox.Engine.Spatial
{
    /// <summary>
    /// Implements an octree node.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the Octree.</typeparam>
    public class OctreeNode<T> : CubeTreeNode<T> where T : TransformableComponent
    {
        /// <summary>
        /// Create an OctreeNode.
        /// </summary>
        /// <param name="position">The position of the created the node.</param>
        /// <param name="scale">The scale of the created node.</param>
        /// <param name="createCastingCollection">The casting collection factory delegate.</param>
        public OctreeNode(Vector3 position, Vector3 scale, CreateCastingCollection<T> createCastingCollection)
            : base(position, scale, createCastingCollection) { }

        /// <inheritdoc />
        sealed protected override CubeTreeNode<T>[] CreateChildrenHook(CreateCastingCollection<T> createCastingCollection)
        {
            CubeTreeNode<T>[] result = new CubeTreeNode<T>[8];
            Vector3 halfScale = Scale * 0.5f;
            result[0] = new OctreeNode<T>(Position + new Vector3(0, 0, 0) * halfScale, halfScale, createCastingCollection);
            result[1] = new OctreeNode<T>(Position + new Vector3(1, 0, 0) * halfScale, halfScale, createCastingCollection);
            result[2] = new OctreeNode<T>(Position + new Vector3(1, 1, 0) * halfScale, halfScale, createCastingCollection);
            result[3] = new OctreeNode<T>(Position + new Vector3(1, 1, 1) * halfScale, halfScale, createCastingCollection);
            result[4] = new OctreeNode<T>(Position + new Vector3(0, 1, 1) * halfScale, halfScale, createCastingCollection);
            result[5] = new OctreeNode<T>(Position + new Vector3(0, 0, 1) * halfScale, halfScale, createCastingCollection);
            result[6] = new OctreeNode<T>(Position + new Vector3(0, 1, 0) * halfScale, halfScale, createCastingCollection);
            result[7] = new OctreeNode<T>(Position + new Vector3(1, 0, 1) * halfScale, halfScale, createCastingCollection);
            return result;
        }
    }

    /// <summary>
    /// Implements an Octree.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the Octree.</typeparam>
    public class Octree<T> : CubeTree<T> where T : TransformableComponent
    {
        /// <summary>
        /// Create an Octree.
        /// </summary>
        /// <param name="position">The position of the tree.</param>
        /// <param name="scale">The scale of the tree.</param>
        /// <param name="subdivisions">The number of times the tree is subdivided.</param>
        /// <param name="createCastingCollection">
        /// Factory delegate for creating the internal casting collection.</param>
        public Octree(Vector3 position, Vector3 scale, int subdivisions,
            CreateCastingCollection<T> createCastingCollection)
            : base(position, scale, subdivisions, createCastingCollection) { }

        /// <inheritdoc />
        sealed protected override CubeTreeNode<T> CreateRootNodeHook(Vector3 position, Vector3 scale,
            CreateCastingCollection<T> createCastingCollection)
        {
            return new OctreeNode<T>(position, scale, createCastingCollection);
        }
    }
}

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using JigLibX.Utils;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Scene.Component;

namespace PhysicsDemoNamespace
{
    public class PhysicsTerrain : Terrain
    {
        public PhysicsTerrain(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain, quadScale, gridDims, scalingFactor,
            effectFileName, heightMapFileName, diffuseMap0FileName, diffuseMap1FileName,
            diffuseMap2FileName, diffuseMap3FileName, textureRepetition)
        {
            collisionSkin = CreateSkin();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) DestroySkin();
            base.Dispose(disposing);
        }

        private CollisionSkin CreateSkin()
        {
            CollisionSkin result = new CollisionSkin();
            Array2D heightField = new Array2D(HeightMap.GetLength(0), HeightMap.GetLength(1));
            for (int x = 0; x < heightField.Nx; ++x)
                for (int z = 0; z < heightField.Nz; ++z)
                    heightField.SetAt(x, z, HeightMap[x, z] * QuadScale.Y);
            Heightmap heightMap = new Heightmap(heightField, 0, 0, QuadScale.X, QuadScale.Z);
            #region UnitTest
            TestCollisionSkin(heightField, heightMap);
            #endregion
            result.AddPrimitive(heightMap, new MaterialProperties(0.8f, 0.8f, 0.7f));
            PhysicsSystem physicsSystem = Engine.GetService<PhysicsSystem>();
            physicsSystem.CollisionSystem.AddCollisionSkin(result);
            return result;
        }

        private void DestroySkin()
        {
            PhysicsSystem physicsSystem = Engine.GetService<PhysicsSystem>();
            physicsSystem.CollisionSystem.RemoveCollisionSkin(collisionSkin);
        }

        #region UnitTest
        /// <summary>
        /// A unit test to see if the collision skin matches up with the visual model.
        /// </summary>
        private void TestCollisionSkin(Array2D heightField, Heightmap heightMap)
        {
            const float heightEpsilon = 0.01f;
            for (int x = 0; x < heightField.Nx; ++x)
            {
                for (int z = 0; z < heightField.Nz; ++z)
                {
                    Vector2 queryPoint = new Vector2(x * QuadScale.X, z * QuadScale.Z) + GridCenterOffset;
                    float jlxSurfaceHeight = heightMap.Heights.Array[x + z * heightField.Nx];
                    float properSurfaceHeight = HeightMap.GrabHeight(queryPoint);
                    if (System.Math.Abs(jlxSurfaceHeight - properSurfaceHeight) > heightEpsilon)
                        System.Diagnostics.Debug.Fail("Surface height mismatch.");
                }
            }
        }
        #endregion

        private static readonly Vector3 quadScale = new Vector3(8, 64, 8);
        private static readonly Vector2 textureRepetition = new Vector2(16);
        private static readonly Point gridDims = new Point(8, 8);
        private const string effectFileName = "Ox/Effects/oxTerrain";
        private const string heightMapFileName = "Ox/HeightMaps/heightMap";
        private const string diffuseMap0FileName = "Ox/Textures/Sand";
        private const string diffuseMap1FileName = "Ox/Textures/Grass";
        private const string diffuseMap2FileName = "Ox/Textures/Rock";
        private const string diffuseMap3FileName = "Ox/Textures/Snow";
        private const float scalingFactor = 1.0f;

        private readonly CollisionSkin collisionSkin;
    }
}

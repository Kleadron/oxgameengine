using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using JigLibX.Utils;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Utility;
using Ox.Scene.Component;

namespace Ox.Scripts
{
    public class ExampleTerrain : ComponentScript<Terrain>, IGround
    {
        public ExampleTerrain(OxEngine engine, Transfer<OxComponent> component)
            : base(engine, component)
        {
            skin = CreateSkin();
        }

        public bool OwnsCollisionSkin(CollisionSkin skin)
        {
            return this.skin == skin;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) DestroySkin();
            base.Dispose(disposing);
        }

        private CollisionSkin CreateSkin()
        {
            PhysicsSystem physicsSystem = Engine.GetService<PhysicsSystem>();
            Terrain component = Component; // OPTIMIZATION: cache property
            Array2D heightField = new Array2D(
                component.HeightMap.GetLength(0),
                component.HeightMap.GetLength(1));

            for (int x = 0; x < heightField.Nx; ++x)
                for (int z = 0; z < heightField.Nz; ++z)
                    heightField.SetAt(x, z, component.HeightMap[x, z] * component.QuadScale.Y);

            Heightmap heightMap = new Heightmap(heightField, 0, 0, component.QuadScale.X, component.QuadScale.Z);
            CollisionSkin result = new CollisionSkin();
            result.AddPrimitive(heightMap, new MaterialProperties(0.8f, 0.8f, 0.7f));
            physicsSystem.CollisionSystem.AddCollisionSkin(result);
            return result;
        }

        private void DestroySkin()
        {
            PhysicsSystem physicsSystem = Engine.GetService<PhysicsSystem>();
            physicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
        }

        private readonly CollisionSkin skin;
    }
}

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Utility;
using Ox.Scene.Component;

namespace Ox.Scripts
{
    public class ExampleBlock : ComponentScript<StandardModel>, IGround
    {
        public ExampleBlock(OxEngine engine, Transfer<OxComponent> component)
            : base(engine, component)
        {
            body = CreateBody();
        }

        public bool OwnsCollisionSkin(CollisionSkin skin)
        {
            return body.CollisionSkin == skin;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) DestroyBody();
            base.Dispose(disposing);
        }

        private Body CreateBody()
        {
            Vector3 position = Component.PositionWorld;
            Vector3 scale = Component.Scale;
            Matrix orientation = Component.OrientationWorld;
            Body result = new Body();
            result.CollisionSkin = new CollisionSkin(result);
            result.CollisionSkin.AddPrimitive(
                new Box(Vector3.Zero, Matrix.Identity, scale),
                new MaterialProperties(0.8f, 0.8f, 0.7f));
            result.Immovable = true;
            result.MoveTo(position - scale * 0.5f, orientation);
            result.EnableBody();
            return result;
        }

        private void DestroyBody()
        {
            body.DisableBody();
        }

        private readonly Body body;
    }
}

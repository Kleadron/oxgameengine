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
    public class ExampleBall : ComponentScript<StandardModel>, ICollidable
    {
        public ExampleBall(OxEngine engine, Transfer<OxComponent> component)
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

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            UpdateTransform();
        }

        private Body CreateBody()
        {
            Vector3 position = Component.PositionWorld;
            Matrix orientation = Component.OrientationWorld;
            Body result = new Body();
            result.CollisionSkin = new CollisionSkin(result);
            result.CollisionSkin.AddPrimitive(
                new Sphere(Vector3.Zero, radius),
                new MaterialProperties(0.8f, 0.8f, 0.7f));
            result.Mass = mass;
            result.MoveTo(position, orientation);
            result.EnableBody();
            return result;
        }

        private void DestroyBody()
        {
            body.DisableBody();
        }

        private void UpdateTransform()
        {
            Component.Position = body.Position;
            Component.Orientation = body.Orientation;
        }

        private const float radius = 5;
        private const float mass = 100;

        private readonly Body body;
    }
}

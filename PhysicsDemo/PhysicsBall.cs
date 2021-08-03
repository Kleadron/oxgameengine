using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Primitive;
using Ox.Scene.Component;

namespace PhysicsDemoNamespace
{
    public class PhysicsBall : StandardModel
    {
        public PhysicsBall(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain, new SpherePrimitive(Vector3.Zero, modelAssetRadius))
        {
            body = CreateBody(modelAssetRadius, mass);
            body.EnableBody();
            ModelFileName = modelFileName;
        }

        public new Vector3 Position
        {
            get { return body.Position; }
            set { body.MoveTo(value, body.Orientation); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) body.DisableBody();
            base.Dispose(disposing);
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            base.Position = body.Position;
            Matrix orientation = body.Orientation;
            SetOrientation(ref orientation);
        }

        private static Body CreateBody(float radius, float mass)
        {
            Body result = new Body();
            result.CollisionSkin = new CollisionSkin(result);
            result.CollisionSkin.AddPrimitive(
                new Sphere(Vector3.Zero, radius),
                new MaterialProperties(0.8f, 0.8f, 0.7f));
            result.Mass = mass;
            return result;
        }

        private const string modelFileName = "Ox/Models/anonymousBall";
        private const float modelAssetRadius = 5;
        private const float mass = 100;

        private readonly Body body;
    }
}

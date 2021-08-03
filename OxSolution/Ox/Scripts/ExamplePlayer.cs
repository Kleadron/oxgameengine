using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Scene.Component;

namespace Ox.Scripts
{
    public class ExamplePlayer : ComponentScript<StandardModel>, ICollidable
    {
        public ExamplePlayer(OxEngine engine, Transfer<OxComponent> component)
            : base(engine, component)
        {
            camera = Engine.Camera;
            cameraOrientation = new EularOrientation();
            cameraOrientation.OrientationChanged += delegate { UpdateCameraView(); };
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
            UpdateOnGround();
            UpdateFriction();
            UpdateInput(gameTime);
            UpdateCamera();
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

        private void UpdateOnGround()
        {
            onGround = false;
            List<CollisionInfo> collisions = Engine.GetService<PhysicsSystem>().Collisions;
            Engine.CollectScripts<IGround>(grounds);
            {
                for (int i = 0; i < grounds.Count; ++i)
                {
                    for (int j = 0; j < collisions.Count; ++j)
                    {
                        IGround ground = grounds[i];
                        CollisionInfo collisionInfo = collisions[j];
                        CollDetectInfo skinInfo = collisionInfo.SkinInfo;
                        CollisionSkin skin0 = skinInfo.Skin0;
                        CollisionSkin skin1 = skinInfo.Skin1;
                        Vector3 collisionNormal = collisionInfo.DirToBody0;
                        if (IsGroundCollision(ground, skin0, skin1, collisionNormal))
                        {
                            onGround = true;
                            break;
                        }
                    }
                }
            }
            grounds.Clear();
        }

        private void UpdateInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Engine.KeyboardState;
            GamePadState gamePadState = Engine.GamePadState;
            float walkImpulse = onGround ? groundWalkImpulse : airWalkImpulse;
            walkImpulse *= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.Up) || gamePadState.DPad.Up == ButtonState.Pressed) body.ApplyWorldImpulse(Vector3.Forward * walkImpulse);
            if (keyboardState.IsKeyDown(Keys.Down) || gamePadState.DPad.Down == ButtonState.Pressed) body.ApplyWorldImpulse(Vector3.Backward * walkImpulse);
            if (keyboardState.IsKeyDown(Keys.Left) || gamePadState.DPad.Left == ButtonState.Pressed) body.ApplyWorldImpulse(Vector3.Left * walkImpulse);
            if (keyboardState.IsKeyDown(Keys.Right) || gamePadState.DPad.Right == ButtonState.Pressed) body.ApplyWorldImpulse(Vector3.Right * walkImpulse);
            if (keyboardState.IsKeyDown(Keys.Space) || gamePadState.Buttons.A == ButtonState.Pressed) Jump(gameTime);
        }

        private void UpdateFriction()
        {
            if (!onGround) return;
            Vector3 velocityXZ = body.Velocity * new Vector3(1, 0, 1);
            body.ApplyWorldImpulse(-velocityXZ * groundFriction);
        }

        private void UpdateCamera()
        {
            cameraOrientation.Angle1 = cameraAngle;
            UpdateCameraView();
        }

        private void UpdateCameraView()
        {
            camera.SetTransformByLookForward(CalculateCameraPosition(), cameraOrientation.Up, cameraOrientation.Forward);
        }

        private Vector3 CalculateCameraPosition()
        {
            return Component.PositionWorld - cameraOrientation.Forward * 64;
        }

        private void Jump(GameTime gameTime)
        {
            float currentTime = (float)gameTime.TotalGameTime.TotalSeconds;
            float jumpTimeDelta = currentTime - lastJumpTime;
            if (!onGround || jumpTimeDelta < jumpTimeOut) return;
            body.ApplyWorldImpulse(Vector3.Up * jumpImpulse);
            lastJumpTime = currentTime;
        }

        private bool IsGroundCollision(IGround ground, CollisionSkin skin0, CollisionSkin skin1, Vector3 collisionNormal)
        {
            return
                body.CollisionSkin == skin0 &&
                ground.OwnsCollisionSkin(skin1) &&
                Vector3.Dot(collisionNormal, Vector3.Up) > onGroundAngle;
        }

        private const float jumpImpulse = 2200;
        private const float airWalkImpulse = 250;
        private const float groundWalkImpulse = 2500;
        private const float turnSpeed = 0.015f;
        private const float jumpTimeOut = 0.1f;
        private const float groundFriction = 0.75f;
        private const float onGroundAngle = 0.75f;
        private const float cameraAngle = -MathHelper.Pi / 6;
        private const float radius = 5;
        private const float mass = 100;

        private readonly IList<IGround> grounds = new List<IGround>();
        private readonly EularOrientation cameraOrientation;
        private readonly Camera camera;
        private readonly Body body;
        private float lastJumpTime;
        private bool onGround;
    }
}

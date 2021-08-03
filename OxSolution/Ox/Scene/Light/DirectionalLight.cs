using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Scene.Component;
using Ox.Scene.Shadow;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// A directional light in a scene.
    /// </summary>
    public class DirectionalLight : Light
    {
        /// <summary>
        /// Create a DirectionalLight with an orthoganol SpotLightShadow.
        /// </summary>
        /// <param name="engine">The engine depedency.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="castShadows">Does the light cast shadows?</param>
        public DirectionalLight(
            OxEngine engine, string domainName, bool ownedByDomain, bool castShadows)
            : base(engine, domainName, ownedByDomain)
        {
            this.castShadows = castShadows;

            if (castShadows)
            {
                OrthoCamera shadowCamera = new OrthoCamera(engine);
                shadowCamera.Width = SceneConfiguration.DirectionalShadowSize.X;
                shadowCamera.Height = SceneConfiguration.DirectionalShadowSize.Y;
                shadowCamera.NearPlane = 0;
                shadowCamera.FarPlane = SceneConfiguration.DirectionalShadowRange;
                AddGarbage(shadowCamera);
                shadow = new DirectionalShadow(engine, shadowCamera, domainName);
                AddGarbage(shadow);
            }
            else
            {
                shadow = new NullDirectionalShadow(engine);
                AddGarbage(shadow);
            }

            Boundless = true;
        }

        /// <summary>
        /// The direction of the light.
        /// </summary>
        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// The position of the shadow camera. Used only if ShadowCameraRelativeToSceneCamera is
        /// false.
        /// </summary>
        public Vector3 ShadowCameraPosition
        {
            get { return shadowCameraPosition; }
            set { shadowCameraPosition = value; }
        }

        /// <summary>
        /// The directional shadow strategy.
        /// </summary>
        public IDirectionalShadow Shadow
        {
            get { return shadow; }
        }

        /// <summary>
        /// The diffuse color of the light.
        /// </summary>
        public Color DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        /// <summary>
        /// The offset of the shadow camera in the direction of the scene camera's backward vector.
        /// Used only if ShadowCameraRelativeToSceneCamera is true.
        /// </summary>
        public float ShadowCameraOffset
        {
            get { return shadowCameraOffset; }
            set { shadowCameraOffset = value; }
        }

        /// <summary>
        /// The interval to which the relative shadow camera snaps.
        /// </summary>
        public float ShadowCameraSnap
        {
            get { return shadowCameraSnap; }
            set { shadowCameraSnap = value; }
        }

        /// <summary>
        /// Is the position of the shadow camera relative to the scene camera?
        /// </summary>
        public bool ShadowCameraRelativeToViewCamera
        {
            get { return shadowCameraRelativeToViewCamera; }
            set { shadowCameraRelativeToViewCamera = value; }
        }

        /// <summary>
        /// Draw the shadow map.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the scene is viewed.</param>
        public void DrawShadow(GameTime gameTime, Camera camera)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);
            if (!shadow.Enabled) return;
            ConfigureShadowCamera(camera);
            shadow.Draw(gameTime);
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return castShadows ?
                new DirectionalLightWithShadowToken() :
                new DirectionalLightToken();
        }

        private void ConfigureShadowCamera(Camera camera)
        {
            ConfigureShadowCameraView(camera);
        }

        private void ConfigureShadowCameraView(Camera camera)
        {
            Vector3 position = CalculateShadowCameraPosition(camera), right, up;
            direction.GetComplimentaryOrientationVectors(out up, out right);
            shadow.Camera.SetTransformByLookForward(position, up, direction);
        }

        private Vector3 CalculateShadowCameraPosition(Camera camera)
        {
            if (shadowCameraRelativeToViewCamera)
            {
                Vector3 cameraPosition = camera.Position - direction * shadowCameraOffset;
                return shadowCameraSnap != 0 ? cameraPosition.GetSnap(shadowCameraSnap) : cameraPosition;
            }
            return shadowCameraPosition;
        }

        private readonly IDirectionalShadow shadow;
        private Vector3 shadowCameraPosition = Vector3.Up * SceneConfiguration.DirectionalShadowRange * 0.5f;
        private Vector3 direction = Vector3.Down;
        private Color diffuseColor = Color.Gray;
        private Color specularColor = Color.Gray;
        private float shadowCameraOffset = SceneConfiguration.DirectionalShadowRange * 0.5f;
        private float shadowCameraSnap = 4;
        private bool shadowCameraRelativeToViewCamera = true;
        private bool castShadows;
    }
}

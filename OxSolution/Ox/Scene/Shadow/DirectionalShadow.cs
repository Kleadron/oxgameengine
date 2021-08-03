using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.RenderTarget;
using Ox.Engine.Utility;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Shadow
{
    /// <summary>
    /// A vanilla implementation of IDirectionalShadow.
    /// </summary>
    public class DirectionalShadow : Disposable, IDirectionalShadow
    {
        /// <summary>
        /// Create a DirectionalShadow.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="shadowCamera">See property Camera.</param>
        /// <param name="domainName">The domain in which to create the blur effect.</param>
        public DirectionalShadow(OxEngine engine, Camera shadowCamera, string domainName)
        {
            OxHelper.ArgumentNullCheck(engine, shadowCamera, domainName);

            this.engine = engine;
            this.shadowCamera = shadowCamera;

            screenQuad = ScreenQuadGeometry.Create(engine.GraphicsDevice, engine.GetService<VertexFactory>(), "PositionNormalTexture"); // MAGICVALUE
            shadowMapTarget = new ManagedRenderTarget2D(engine, SceneConfiguration.DirectionalShadowMapSize, 1, SurfaceFormat.Single, MultiSampleType.None, 0, 0);
        }

        /// <inheritdoc />
        public Texture2D VolatileShadowMap { get { return shadowMapTarget.VolatileTexture; } }

        /// <inheritdoc />
        public Camera Camera { get { return shadowCamera; } }

        /// <inheritdoc />
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <inheritdoc />
        public void Draw(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            DrawShadowMap(gameTime);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                shadowMapTarget.Dispose();
                screenQuad.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DrawShadowMap(GameTime gameTime)
        {
            SceneSystem sceneSystem = engine.GetService<SceneSystem>();
            IList<BaseSurface> surfaces = sceneSystem.CachedSurfaces;

            shadowMapTarget.Activate();
            {
                engine.GraphicsDevice.Clear(Color.White);
                for (int i = 0; i < surfaces.Count; ++i)
                {
                    BaseSurface surface = surfaces[i];
                    if (IsShadowing(shadowCamera, surface)) surface.Draw(gameTime, shadowCamera, "DirectionalShadow");
                }
            }
            shadowMapTarget.Resolve();

            //shadowMapTarget.VolatileTexture.Save("directionalShadowMap.png", ImageFileFormat.Png);
        }

        private static bool IsShadowing(Camera shadowCamera, BaseSurface surface)
        {
            return
                surface.HasDrawProperties(DrawProperties.Shadowing) &&
                (
                    surface.Boundless ||
                    shadowCamera.Contains(surface.BoundingBoxWorld) != ContainmentType.Disjoint
                );
        }

        private readonly ManagedRenderTarget2D shadowMapTarget;
        private readonly OxEngine engine;
        private readonly Geometry screenQuad;
        private readonly Camera shadowCamera;
        private bool enabled = true;
    }
}

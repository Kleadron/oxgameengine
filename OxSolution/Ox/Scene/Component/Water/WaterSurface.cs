using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.RenderTarget;
using Ox.Engine.Utility;
using Ox.Scene.EffectNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// The surface of Water.
    /// </summary>
    public class WaterSurface : Surface<Water>
    {
        /// <summary>
        /// Create a WaterSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public WaterSurface(OxEngine engine, Water component)
            : base(engine, component, WaterDefaults.EffectFileName)
        {
            AddGarbage(reflectionMapTarget = new ManagedRenderTarget2D(engine, SceneConfiguration.WaterReflectionMapSize, 1, SurfaceFormat.Color, MultiSampleType.None, 0, 0));
            AddGarbage(geometry = QuadGeometry.Create(engine.GraphicsDevice, engine.GetService<VertexFactory>(), "PositionNormalTexture")); // MAGICVALUE
            WaveMap0FileName = WaterDefaults.WaveMap0FileName;
            WaveMap1FileName = WaterDefaults.WaveMap1FileName;
            DrawStyle = WaterDefaults.DrawStyleDefault;
            DrawProperties = DrawProperties.Reflecting;
        }

        /// <summary>
        /// The velocity of the 0th wave normal map.
        /// </summary>
        public Vector2 WaveMap0Velocity
        {
            get { return waveMap0Velocity; }
            set { waveMap0Velocity = value; }
        }
        
        /// <summary>
        /// The velocity of the 1st wave normal map.
        /// </summary>
        public Vector2 WaveMap1Velocity
        {
            get { return waveMap1Velocity; }
            set { waveMap1Velocity = value; }
        }
        
        /// <summary>
        /// The multiplier of the water's color.
        /// </summary>
        public Color ColorMultiplier
        {
            get { return colorMultiplier; }
            set { colorMultiplier = value; }
        }
        
        /// <summary>
        /// The color of the water.
        /// </summary>
        public Color ColorAdditive
        {
            get { return colorAdditive; }
            set { colorAdditive = value; }
        }
        
        /// <summary>
        /// The name of the 0th wave normal map.
        /// </summary>
        public string WaveMap0FileName
        {
            get { return _waveMap0FileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_waveMap0FileName == value) return; // OPTIMIZATION
                Texture2D newWaveMap0 = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _waveMap0 = newWaveMap0;
                _waveMap0FileName = value;
            }
        }
        
        /// <summary>
        /// The name of the 1st wave normal map.
        /// </summary>
        public string WaveMap1FileName
        {
            get { return _waveMap1FileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_waveMap1FileName == value) return; // OPTIMIZATION
                Texture2D newWaveMap1 = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _waveMap1 = newWaveMap1;
                _waveMap1FileName = value;
            }
        }
        
        /// <summary>
        /// The length of the waves.
        /// </summary>
        public float WaveLength
        {
            get { return waveLength; }
            set { waveLength = value; }
        }
        
        /// <summary>
        /// The height of the waves.
        /// </summary>
        public float WaveHeight
        {
            get { return waveHeight; }
            set { waveHeight = value; }
        }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return _effect; } }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return _effectFileName; }
            set
            {
                if (_effectFileName == value) return; // OPTIMIZATION
                Effect newEffect = CreateEffect(value);
                // EXCEPTIONSAFETYLINE
                _effect = newEffect;
                _effectFileName = value;
            }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera)
        {
            // grab dependencies
            GraphicsDevice device = Engine.GraphicsDevice;
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();

            // calculate water height variables
            float waterHeight = PositionWorld.Y;
            float waterHeightTimesTwo = waterHeight * 2;
            
            // cache off original camera transform
            Vector3 originalPosition = camera.Position;
            Vector3 originalLookUp = camera.LookUp;
            Vector3 originalLookForward = camera.LookForward;

            // find reflected camera transform
            Vector3 reflectedPosition = new Vector3(camera.Position.X, waterHeightTimesTwo - camera.Position.Y, camera.Position.Z);
            Vector3 reflectedLookTarget = new Vector3(camera.LookTarget.X, waterHeightTimesTwo - camera.LookTarget.Y, camera.LookTarget.Z);

            // reflect camera
            camera.SetTransformByLookTarget(reflectedPosition, Vector3.Up, reflectedLookTarget);
            {
                // update the reflection view
                camera.GetView(out reflectionView);
                
                // create the reflection plane
                Plane reflectionClipPlane = WaterHelper.CreateWaterReflectionPlane(camera, waterHeight);

                // organize the surfaces for reflection drawing
                OrganizeSurfaces(camera);

                // draw the reflection map
                reflectionMapTarget.Activate();
                {
                    // clear the water reflection target
                    device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1, 0);

                    // draw the objects whose drawing position is dependent on the reflection camera's
                    // position - such as a skybox - before you set the clip plane!
                    // Otherwise the object is drawn around the reflecting camera and might therefore
                    // be underneath the clip plane. This would cause properly visible objects to be
                    // clipped.
                    DrawSurfaces(gameTime, camera, cachedDependantPriors, cachedDependantOpaques,
                        cachedDependantTransparents, sceneSystem.DrawOpaquesNearToFar);

                    // set the clip planes on the water reflection target
                    device.ClipPlanes[0].IsEnabled = true;
                    device.ClipPlanes[0].Plane = reflectionClipPlane;

                    // draw all the camera-independant surfaces
                    DrawSurfaces(gameTime, camera, cachedPriors, cachedOpaques, cachedTransparents,
                        sceneSystem.DrawOpaquesNearToFar);

                    // disable the clip planes
                    device.ClipPlanes[0].IsEnabled = false;
                }
                // resolve the reflection map
                reflectionMapTarget.Resolve();
            }
            // restore camera
            camera.SetTransformByLookForward(originalPosition, originalLookUp, originalLookForward);

            //reflectionMapTarget.VolatileTexture.Save("waterReflectionMap.png", ImageFileFormat.Png);
        }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;
            PopulateEffect(gameTime, camera);

            BeginRenderState();
            {
                BeginEffect();
                {
                    DrawGeometryPasses(gameTime);
                }
                EndEffect();
            }
            EndRenderState();
        }
        
        private Texture2D WaveMap0 { get { return _waveMap0; } }
        
        private Texture2D WaveMap1 { get { return _waveMap1; } }

        private void OrganizeSurfaces(Camera camera)
        {
            ClearSurfaces();
            PopulateSurfaces(camera);
        }

        private void ClearSurfaces()
        {
            cachedDependantPriors.Clear();
            cachedDependantOpaques.Clear();
            cachedDependantTransparents.Clear();
            cachedPriors.Clear();
            cachedOpaques.Clear();
            cachedTransparents.Clear();
        }

        private void PopulateSurfaces(Camera camera)
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            IList<BaseSurface> surfaces = sceneSystem.CachedSurfaces;
            for (int i = 0; i < surfaces.Count; ++i)
            {
                BaseSurface surface = surfaces[i];
                if (IsReflecting(camera, surface) && surface != this)
                {
                    if (surface.HasDrawProperties(DrawProperties.DependantTransform)) OrganizeDependantSurface(surface);
                    else OrganizeSurface(surface);
                }
            }
        }

        private void OrganizeDependantSurface(BaseSurface surface)
        {
            switch (surface.DrawStyle)
            {
                case DrawStyle.Prioritized: cachedDependantPriors.Add(surface); break;
                case DrawStyle.Opaque: cachedDependantOpaques.Add(surface); break;
                case DrawStyle.Transparent: cachedDependantTransparents.Add(surface); break;
            }
        }

        private void OrganizeSurface(BaseSurface surface)
        {
            switch (surface.DrawStyle)
            {
                case DrawStyle.Prioritized: cachedPriors.Add(surface); break;
                case DrawStyle.Opaque: cachedOpaques.Add(surface); break;
                case DrawStyle.Transparent: cachedTransparents.Add(surface); break;
            }
        }

        private void DrawSurfaces(GameTime gameTime, Camera camera, List<BaseSurface> priors,
            List<BaseSurface> opaques, List<BaseSurface> transparents, bool drawOpaquesNearToFar)
        {
            DrawPriors(gameTime, camera, priors);
            DrawOpaques(gameTime, camera, opaques, drawOpaquesNearToFar);
            DrawTransparents(gameTime, camera, transparents);
        }

        private void DrawPriors(GameTime gameTime, Camera camera, List<BaseSurface> priors)
        {
            DrawHelper.PrioritySort(priors);
            for (int i = 0; i < priors.Count; ++i) priors[i].Draw(gameTime, camera, "Normal");
        }

        private void DrawOpaques(GameTime gameTime, Camera camera, List<BaseSurface> opaques, bool drawOpaquesNearToFar)
        {
            if (drawOpaquesNearToFar) DrawHelper.DistanceSort(opaques, camera.Position, SpatialSortOrder.NearToFar);
            for (int i = 0; i < opaques.Count; ++i) opaques[i].Draw(gameTime, camera, "Normal");
        }

        private void DrawTransparents(GameTime gameTime, Camera camera, List<BaseSurface> transparents)
        {
            DrawHelper.DistanceSort(transparents, camera.Position, SpatialSortOrder.FarToNear);
            for (int i = 0; i < transparents.Count; ++i) transparents[i].Draw(gameTime, camera, "Normal");
        }

        private Effect CreateEffect(string effectFileName)
        {
            Effect effectFromDisk = Engine.Load<Effect>(effectFileName, DomainName);
            return new LightReceiverEffect(Engine.GraphicsDevice, effectFromDisk);
        }

        private void PopulateEffect(GameTime gameTime, Camera camera)
        {
            LightReceiverEffect lrEffect = OxHelper.Cast<LightReceiverEffect>(Effect);
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            Texture2D reflectionMap = reflectionMapTarget.VolatileTexture;
            if (reflectionMap == null) return;
            Matrix world = TransformWorldScaled;
            float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            lrEffect.TrySetCurrentTechnique("Normal");
            lrEffect.Parameters["xWaterColorMultiplier"].TrySetValue(ColorMultiplier.ToVector4());
            lrEffect.Parameters["xWaterColorAdditive"].TrySetValue(ColorAdditive.ToVector4());
            lrEffect.Parameters["xReflectionMap"].TrySetValue(reflectionMap);
            lrEffect.Parameters["xReflectionView"].TrySetValue(reflectionView);
            lrEffect.Parameters["xWaveOffset0"].TrySetValue(waveMap0Velocity * totalTime);
            lrEffect.Parameters["xWaveOffset1"].TrySetValue(waveMap1Velocity * totalTime);
            lrEffect.Parameters["xWaveLength"].TrySetValue(waveLength);
            lrEffect.Parameters["xWaveHeight"].TrySetValue(waveHeight);
            lrEffect.Parameters["xWaveMap0"].TrySetValue(WaveMap0);
            lrEffect.Parameters["xWaveMap1"].TrySetValue(WaveMap1);
            lrEffect.PopulateTransform(camera, ref world);
            lrEffect.PopulateFogging(sceneSystem.Fog);
            lrEffect.PopulateLighting(this, sceneSystem.CachedAmbientLights,
                sceneSystem.CachedDirectionalLights, sceneSystem.CachedPointLights);
        }

        private void BeginEffect()
        {
            Effect.Begin();
        }

        private void EndEffect()
        {
            Effect.End();
        }

        private void DrawGeometryPasses(GameTime gameTime)
        {
            EffectPassCollection passes = Effect.CurrentTechnique.Passes;
            for (int i = 0; i < passes.Count; ++i) DrawGeometryPass(gameTime, passes[i]);
        }

        private void DrawGeometryPass(GameTime gameTime, EffectPass pass)
        {
            pass.Begin();
            {
                geometry.Draw(gameTime);
            }
            pass.End();
        }

        private void BeginRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.BeginFaceMode(FaceMode);
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            }
        }

        private void EndRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.DestinationBlend = Blend.Zero;
                device.RenderState.SourceBlend = Blend.One;
                device.RenderState.AlphaBlendEnable = false;
            }
            device.EndFaceMode();
        }

        private static bool IsReflecting(Camera camera, BaseSurface surface)
        {
            return
                surface.HasDrawProperties(DrawProperties.Reflecting) &&
                (
                    surface.Boundless ||
                    camera.Contains(surface.BoundingBoxWorld) != ContainmentType.Disjoint
                );
        }
        
        private readonly List<BaseSurface> cachedDependantPriors = new List<BaseSurface>();
        private readonly List<BaseSurface> cachedDependantOpaques = new List<BaseSurface>();
        private readonly List<BaseSurface> cachedDependantTransparents = new List<BaseSurface>();
        private readonly List<BaseSurface> cachedPriors = new List<BaseSurface>();
        private readonly List<BaseSurface> cachedOpaques = new List<BaseSurface>();
        private readonly List<BaseSurface> cachedTransparents = new List<BaseSurface>();
        /// <summary>May be null.</summary>
        private readonly ManagedRenderTarget2D reflectionMapTarget;
        private readonly Geometry geometry;
        private Vector2 waveMap0Velocity = new Vector2(0.1f, 0);
        private Vector2 waveMap1Velocity = new Vector2(0, 0.1f);
        private Matrix reflectionView;
        private Color colorMultiplier = Color.DarkBlue;
        private Color colorAdditive;
        private float waveLength = 0.05f;
        private float waveHeight = 0.05f;
        private Texture2D _waveMap0;
        private Texture2D _waveMap1;
        private Effect _effect;
        private string _effectFileName;
        private string _waveMap0FileName;
        private string _waveMap1FileName;
    }
}

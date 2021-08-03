using System;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Scene.Component;

namespace Ox.Scene.SurfaceNamespace
{
    /// <summary>
    /// Creates surfaces.
    /// </summary>
    public class SurfaceFactory
    {
        /// <summary>
        /// Create a SurfaceFactory.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public SurfaceFactory(OxEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Create a Surface.
        /// May return null.
        /// </summary>
        public T CreateSurface<T>(SceneComponent component) where T : BaseSurface
        {
            BaseSurface surface = CreateSurfaceHook(component, typeof(T));
            T surfaceAsT = surface as T;
            if (surfaceAsT == null) surface.Dispose();
            return surfaceAsT;
        }

        /// <summary>
        /// Create a TerrainSurface.
        /// Needs extra parameters for efficiency.
        /// </summary>
        public TerrainSurface CreateTerrainSurface(Terrain component,
            Vector3 quadScale, Point gridDims, float smoothingFactor, string effectFileName,
            string heightMapFileName, string diffuseMap0FileName, string diffuseMap1FileName,
            string diffuseMap2FileName, string diffuseMap3FileName, Vector2 textureRepetition)
        {
            return CreateTerrainSurfaceHook(OxHelper.Cast<Terrain>(component), quadScale,
                gridDims, smoothingFactor, effectFileName, heightMapFileName, diffuseMap0FileName,
                diffuseMap1FileName, diffuseMap2FileName, diffuseMap3FileName, textureRepetition);
        }

        /// <summary>
        /// The engine.
        /// </summary>
        protected OxEngine Engine { get { return engine; } }

        /// <summary>
        /// Handle creating a Surface.
        /// May return null.
        /// </summary>
        protected virtual BaseSurface CreateSurfaceHook(SceneComponent component, Type surfaceType)
        {
            if (surfaceType == typeof(AnimatedSurface)) return new AnimatedSurface(engine, OxHelper.Cast<AnimatedModel>(component));
            if (surfaceType == typeof(BasicSurface)) return new BasicSurface(engine, OxHelper.Cast<BasicModel>(component));
            if (surfaceType == typeof(BoundingBoxVisualizerSurface)) return new BoundingBoxVisualizerSurface(engine, OxHelper.Cast<BoundingBoxVisualizer>(component));
            if (surfaceType == typeof(ParticleEmitterSurface)) return new ParticleEmitterSurface(engine, OxHelper.Cast<ParticleEmitter>(component));
            if (surfaceType == typeof(SkyboxSurface)) return new SkyboxSurface(engine, OxHelper.Cast<Skybox>(component));
            if (surfaceType == typeof(StandardSurface)) return new ShadowReceiverSurface(engine, OxHelper.Cast<StandardModel>(component));
            if (surfaceType == typeof(WaterSurface)) return new WaterSurface(engine, OxHelper.Cast<Water>(component));
            return null;
        }

        /// <summary>
        /// Handle creating a TerrainSurface.
        /// </summary>
        protected virtual TerrainSurface CreateTerrainSurfaceHook(Terrain component,
            Vector3 quadScale, Point gridDims, float smoothingFactor, string effectFileName,
            string heightMapFileName, string diffuseMap0FileName, string diffuseMap1FileName,
            string diffuseMap2FileName, string diffuseMap3FileName, Vector2 textureRepetition)
        {
            return new TerrainSurface(engine, component, quadScale, gridDims, smoothingFactor,
                effectFileName, heightMapFileName, diffuseMap0FileName, diffuseMap1FileName,
                diffuseMap2FileName, diffuseMap3FileName, textureRepetition);
        }

        /// <summary>
        /// The engine as T.
        /// </summary>
        protected T GetEngine<T>() where T : OxEngine
        {
            return OxHelper.Cast<T>(engine);
        }

        private readonly OxEngine engine;
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Spatial;
using Ox.Engine.Utility;
using Ox.Scene.Component;
using Ox.Scene.FogNamespace;
using Ox.Scene.LightNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene
{
    /// <summary>
    /// The scene system.
    /// </summary>
    /// <remarks>
    /// The scene system implements generalized spatial culling on entire scene components.
    /// It does not, however, implement specialized culling (such as BSP or occlusion) on the
    /// individual surfaces. Instead, it defers any such implementation to individual scene
    /// components through a 'mini-framework' of overridable methods and properties. The developer
    /// can implement specialized, surface-level culling behavior by correctly overriding
    /// SceneComponent's three CollectSurfaceHook methods and all of TransformableSubcomponent's
    /// transformation properties.
    /// </remarks>
    public class SceneSystem : Disposable
    {
        /// <summary>
        /// Create a SceneSystem.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="collection">The collection to hold the tracked components.</param>
        public SceneSystem(OxEngine engine, Transfer<ISpatialContainer<SceneComponent>> collection)
        {
            OxHelper.ArgumentNullCheck(engine, collection.Value);
            if (collection.Value.Collect<SceneComponent>(new List<SceneComponent>()).Count != 0)
                throw new InvalidOperationException("Cannot add a collection with pre-existing members to a scene.");
            this.engine = engine;
            this.components = collection.Value;
            engine.AddDocumentLoader(SceneConfiguration.SceneDocumentType, new SceneDocumentLoader());
        }

        /// <summary>
        /// A cached collection of the scene's components.
        /// </summary>
        public List<SceneComponent> CachedComponents { get { return cachedComponents; } }

        /// <summary>
        /// A cached collection of the scene components' surfaces.
        /// </summary>
        public List<BaseSurface> CachedSurfaces { get { return cachedSurfaces; } }

        /// <summary>
        /// A cached collection of the scene's visible ambient lights.
        /// </summary>
        public List<AmbientLight> CachedAmbientLights { get { return cachedAmbientLights; } }

        /// <summary>
        /// A cached collection of the scene's visible directional lights.
        /// </summary>
        public List<DirectionalLight> CachedDirectionalLights { get { return cachedDirectionalLights; } }

        /// <summary>
        /// A cached collection of the scene's visible point lights.
        /// </summary>
        public List<PointLight> CachedPointLights { get { return cachedPointLights; } }

        /// <summary>
        /// The bounds within which drawing occurs.
        /// </summary>
        public BoundingBox DrawBounds
        {
            get { return drawBounds; }
            set { drawBounds = value; }
        }

        /// <summary>
        /// The fog.
        /// </summary>
        public Fog Fog { get { return fog; } }

        /// <summary>
        /// How far in the direction of a light source will objects cast shadows?
        /// </summary>
        public float MaxShadowDepth
        {
            get { return maxShadowDepth; }
            set { maxShadowDepth = value; }
        }

        /// <summary>
        /// Should the opaque items be drawn in near-to-far order?
        /// </summary>
        public bool DrawOpaquesNearToFar
        {
            get { return drawOpaquesFrontToBack; }
            set { drawOpaquesFrontToBack = value; }
        }

        /// <summary>
        /// PreDraw the items in the DrawBounds. Must be called once before Draw.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the scene is viewed.</param>
        /// <remarks>
        /// Pre-drawing is done in a different phase than normal drawing. This is because of the
        /// following performance constraint -
        /// 
        /// Intermixing scene drawing with pre-drawing involves switching render targets while
        /// persisting their data. Persisting render target data during render target changes
        /// involves copying the render target buffer to temporary texture buffers on the Xbox 360,
        /// then copying it back. This is just too slow so long as the Xbox 360 or a device with
        /// the same constraint is a deployment target.
        /// 
        /// For more info, please read -
        /// http://blogs.msdn.com/shawnhar/archive/2007/11/21/rendertarget-changes-in-xna-game-studio-2-0.aspx
        /// 
        /// At some point it might make sense to have a PreDrawOrder property so that items can be
        /// consistently pre-drawn in a certain order rather than the random order in which they
        /// are found in the scene.
        /// </remarks>        
        public void PreDraw(GameTime gameTime, Camera camera)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);
            CacheComponents();
            CacheLights();
            CacheSurfaces();
            DrawShadows(gameTime, camera);
            PreDrawSurfaces(gameTime, camera);
        }

        /// <summary>
        /// Draw the items in the DrawingBounds.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the scene is viewed.</param>
        /// <param name="drawMode">The manned in which to draw the scene.</param>
        public void Draw(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawSurfaces(gameTime, camera, drawMode);
            ClearSurfaces();
            ClearLights();
            ClearComponents();
        }

        /// <summary>
        /// Add a component to the scene.
        /// </summary>
        public void AddComponent(SceneComponent component)
        {
            OxHelper.ArgumentNullCheck(component);
            components.Add(component);
        }

        /// <summary>
        /// Remove a component from the scene.
        /// </summary>
        public bool RemoveComponent(SceneComponent component)
        {
            OxHelper.ArgumentNullCheck(component);
            return components.Remove(component);
        }

        /// <summary>
        /// Collect all components of type T in the specified bounds.
        /// </summary>
        public IList<T> CollectComponents<T>(BoundingFrustum bounds, IList<T> result) where T : class
        {
            return components.Collect(bounds, result);
        }

        /// <summary>
        /// Collect all components of type T in the specified bounds.
        /// </summary>
        public IList<T> CollectComponents<T>(BoundingBox bounds, IList<T> result) where T : class
        {
            return components.Collect(bounds, result);
        }

        /// <summary>
        /// Collect all components of type T in the specified bounds that satisfy a predicate.
        /// </summary>
        public IList<T> CollectComponents<T>(Func<T, bool> predicate, BoundingFrustum bounds, IList<T> result) where T : class
        {
            return components.Collect(predicate, bounds, result);
        }

        /// <summary>
        /// Collect all components of type T in the specified bounds that satisfy a predicate.
        /// </summary>
        public IList<T> CollectComponents<T>(Func<T, bool> predicate, BoundingBox bounds, IList<T> result) where T : class
        {
            return components.Collect(predicate, bounds, result);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                engine.RemoveDocumentLoader(SceneConfiguration.SceneDocumentType);
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void CacheComponents()
        {
            CollectComponents<SceneComponent>(visiblePredicate, drawBounds, cachedComponents);
        }

        private void CacheLights()
        {
            CollectComponents<Light>(enabledPredicate, drawBounds, cachedLights);
            OrganizeLights();
        }

        private void CacheSurfaces()
        {
            for (int i = 0; i < cachedComponents.Count; ++i)
                cachedComponents[i].CollectSurfaces(drawBounds, cachedSurfaces);
        }

        private void DrawShadows(GameTime gameTime, Camera camera)
        {
            List<DirectionalLight> directionalLights = CachedDirectionalLights; // OPTIMIZATION: cache
            for (int i = 0; i < directionalLights.Count; ++i)
            {
                DirectionalLight directionalLight = directionalLights[i];
                if (directionalLight.EnabledWorld) directionalLight.DrawShadow(gameTime, camera);
            }
        }

        private void PreDrawSurfaces(GameTime gameTime, Camera camera)
        {
            SurfaceDrawer drawer = engine.GetService<SurfaceDrawer>();
            drawer.PreDrawSurfaces(gameTime, camera, cachedSurfaces);
        }

        private void DrawSurfaces(GameTime gameTime, Camera camera, string drawMode)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);
            SurfaceDrawer drawer = engine.GetService<SurfaceDrawer>();
            engine.GetService<SurfaceDrawer>().DrawSurfaces(gameTime, camera, drawMode, cachedSurfaces);
        }

        private void ClearSurfaces()
        {
            cachedSurfaces.Clear();
        }

        private void ClearLights()
        {
            cachedLights.Clear();
            cachedPointLights.Clear();
            cachedDirectionalLights.Clear();
            cachedAmbientLights.Clear();
        }

        private void ClearComponents()
        {
            cachedComponents.Clear();
        }

        private void OrganizeLights()
        {
            for (int i = 0; i < cachedLights.Count; ++i) OrganizeLight(cachedLights[i]);
        }

        private void OrganizeLight(Light light)
        {
            PointLight pointLight = light as PointLight;
            if (pointLight != null) cachedPointLights.Add(pointLight);
            else
            {
                DirectionalLight directionalLight = light as DirectionalLight;
                if (directionalLight != null) cachedDirectionalLights.Add(directionalLight);
                else
                {
                    AmbientLight ambientLight = light as AmbientLight;
                    if (ambientLight != null) cachedAmbientLights.Add(ambientLight);
                }
            }
        }

        private static readonly Func<SceneComponent, bool> visiblePredicate = x => x.VisibleWorld;
        private static readonly Func<Light, bool> enabledPredicate = x => x.EnabledWorld;

        private readonly ISpatialContainer<SceneComponent> components;
        private readonly List<DirectionalLight> cachedDirectionalLights = new List<DirectionalLight>();
        private readonly List<SceneComponent> cachedComponents = new List<SceneComponent>();
        private readonly List<AmbientLight> cachedAmbientLights = new List<AmbientLight>();
        private readonly List<BaseSurface> cachedSurfaces = new List<BaseSurface>();
        private readonly List<PointLight> cachedPointLights = new List<PointLight>();
        private readonly List<Light> cachedLights = new List<Light>();
        private readonly OxEngine engine;
        private readonly Fog fog = new Fog();
        private BoundingBox drawBounds = BoundingBoxHelper.CreateAllEncompassing;
        private float maxShadowDepth = 512;
        private bool drawOpaquesFrontToBack;
    }
}

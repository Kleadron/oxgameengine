using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.MathNamespace;

namespace Ox.Scene.SurfaceNamespace
{
    /// <summary>
    /// Draws surfaces.
    /// </summary>
    public class SurfaceDrawer
    {
        /// <summary>
        /// Create a SurfaceDrawer.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public SurfaceDrawer(OxEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// PreDraw a single surface.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface is viewed.</param>
        /// <param name="surface">The surface to pre-draw.</param>
        public void PreDrawSurface(GameTime gameTime, Camera camera, BaseSurface surface)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);
            surface.PreDraw(gameTime, camera);
        }

        /// <summary>
        /// Draw a single surface.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface is viewed.</param>
        /// <param name="drawMode">The manner in which to draw the surface.</param>
        /// <param name="surface">The surface to draw.</param>
        public void DrawSurface(GameTime gameTime, Camera camera, string drawMode, BaseSurface surface)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);
            surface.Draw(gameTime, camera, drawMode);
        }

        /// <summary>
        /// Pre-draw multiple surfaces.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface are viewed.</param>
        /// <param name="surfaces">The surfaces to pre-draw.</param>
        public void PreDrawSurfaces(GameTime gameTime, Camera camera, IList<BaseSurface> surfaces)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera, surfaces);
            for (int i = 0; i < surfaces.Count; ++i) surfaces[i].PreDraw(gameTime, camera);
        }

        /// <summary>
        /// Draw multiple surfaces.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the surface are viewed.</param>
        /// <param name="drawMode">The manner in which to draw the surface.</param>
        /// <param name="surfaces">The surfaces to draw.</param>
        public void DrawSurfaces(GameTime gameTime, Camera camera, string drawMode, IList<BaseSurface> surfaces)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera, surfaces);
            OrganizeSurfaces(camera, surfaces);
            DrawSurfaces(gameTime, camera, drawMode);
        }

        private void OrganizeSurfaces(Camera camera, IList<BaseSurface> surfaces)
        {
            for (int i = 0; i < surfaces.Count; ++i) OrganizeSurface(camera, surfaces[i]);
        }

        private void OrganizeSurface(Camera camera, BaseSurface surface)
        {
            if (surface.Boundless || camera.Contains(surface.BoundingBoxWorld) != ContainmentType.Disjoint)
            {
                switch (surface.DrawStyle)
                {
                    case DrawStyle.Prioritized: cachedPriors.Add(surface); break;
                    case DrawStyle.Opaque: cachedOpaques.Add(surface); break;
                    case DrawStyle.Transparent: cachedTransparents.Add(surface); break;
                }
            }
        }

        private void DrawSurfaces(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawPriors(gameTime, camera, drawMode); cachedPriors.Clear();
            DrawOpaques(gameTime, camera, drawMode); cachedOpaques.Clear();
            DrawTransparents(gameTime, camera, drawMode); cachedTransparents.Clear();
        }

        private void DrawPriors(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawHelper.PrioritySort(cachedPriors);
            for (int i = 0; i < cachedPriors.Count; ++i)
                cachedPriors[i].Draw(gameTime, camera, drawMode);
        }

        private void DrawOpaques(GameTime gameTime, Camera camera, string drawMode)
        {
            if (engine.GetService<SceneSystem>().DrawOpaquesNearToFar)
                DrawHelper.DistanceSort(cachedOpaques, camera.Position, SpatialSortOrder.NearToFar);
            for (int i = 0; i < cachedOpaques.Count; ++i)
                cachedOpaques[i].Draw(gameTime, camera, drawMode);
        }

        private void DrawTransparents(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawHelper.DistanceSort(
                cachedTransparents, camera.Position, SpatialSortOrder.FarToNear);
            for (int i = 0; i < cachedTransparents.Count; ++i)
                cachedTransparents[i].Draw(gameTime, camera, drawMode);
        }

        private readonly List<BaseSurface> cachedPriors = new List<BaseSurface>();
        private readonly List<BaseSurface> cachedOpaques = new List<BaseSurface>();
        private readonly List<BaseSurface> cachedTransparents = new List<BaseSurface>();
        private readonly OxEngine engine;
    }
}

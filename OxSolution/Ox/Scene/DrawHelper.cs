using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.MathNamespace;
using Ox.Scene.Component;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene
{
    /// <summary>
    /// Compares the drawing priority of surfaces.
    /// </summary>
    public class DrawPriorityComparer : IComparer<BaseSurface>
    {
        /// <inheritdoc />
        public int Compare(BaseSurface x, BaseSurface y)
        {
            return OxMathHelper.Compare(x.DrawPriority, y.DrawPriority);
        }
    }

    /// <summary>
    /// Compares the distance of surfaces from an origin.
    /// </summary>
    public interface IDrawOrderComparer<T> : IComparer<T>
        where T : BaseSurface
    {
        /// <summary>
        /// The origin from which distance is calculated.
        /// </summary>
        Vector3 Origin { get; set; }
    }

    /// <summary>
    /// Compares the distance of surfaces from an origin, sorting in ascending order.
    /// </summary>
    public class DrawNearToFarComparer<T> : IDrawOrderComparer<T>
        where T : BaseSurface
    {
        public DrawNearToFarComparer(Vector3 origin)
        {
            this.origin = origin;
        }

        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public int Compare(T x, T y)
        {
            OxHelper.ArgumentNullCheck(x, y);
            float distance1 = x.DistanceSquared(origin);
            float distance2 = y.DistanceSquared(origin);
            if (distance1 < distance2) return -1;
            if (distance1 > distance2) return 1;
            if ((int)x.FaceMode < (int)y.FaceMode) return -1;
            if ((int)x.FaceMode > (int)y.FaceMode) return 1;
            return 0;
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Compares the distance of surfaces from an origin, sorting in descending order.
    /// </summary>
    public class DrawFarToNearComparer<T> : IDrawOrderComparer<T>
        where T : BaseSurface
    {
        public DrawFarToNearComparer(Vector3 origin)
        {
            this.origin = origin;
        }

        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public int Compare(T x, T y)
        {
            OxHelper.ArgumentNullCheck(x, y);
            float distance1 = x.DistanceSquared(origin);
            float distance2 = y.DistanceSquared(origin);
            if (distance1 > distance2) return -1;
            if (distance1 < distance2) return 1;
            if ((int)x.FaceMode > (int)y.FaceMode) return -1;
            if ((int)x.FaceMode < (int)y.FaceMode) return 1;
            return 0;
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Helper methods for various drawing tasks in Ox.
    /// </summary>
    public static class DrawHelper
    {
        /// <summary>
        /// Draw geometry using a parameterized shader effect.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="geometry">The geometry to draw.</param>
        /// <param name="effect">The shader effect to draw the geometry with.</param>
        public static void DrawGeometry(GameTime gameTime, Geometry geometry, Effect effect)
        {
            OxHelper.ArgumentNullCheck(gameTime, geometry, effect);

            effect.Begin();
            {
                EffectPassCollection passes = effect.CurrentTechnique.Passes;
                for (int i = 0; i < passes.Count; ++i)
                {
                    EffectPass pass = passes[i]; pass.Begin();
                    {
                        geometry.Draw(gameTime);
                    }
                    pass.End();
                }
            }
            effect.End();
        }

        /// <summary>
        /// Sort a list of surfaces by distance from an origin.
        /// </summary>
        /// <param name="list">The list of surface to be sorted.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <param name="sortOrder">The order in which sorting takes place.</param>
        public static void DistanceSort(this List<BaseSurface> list, Vector3 origin, SpatialSortOrder sortOrder)
        {
            lock (staticLock)
            {
                OxHelper.ArgumentNullCheck(list);
                IDrawOrderComparer<BaseSurface> comparer;
                if (sortOrder == SpatialSortOrder.FarToNear) comparer = farToNearComparer;
                else comparer = nearToFarComparer;                     
                comparer.Origin = origin;
                list.Sort(comparer);
            }
        }

        /// <summary>
        /// Sort a list of surfaces by their Priority property.
        /// </summary>
        /// <param name="list">The list of surfaces to be sorted.</param>
        public static void PrioritySort(this List<BaseSurface> list)
        {
            lock (staticLock)
            {
                OxHelper.ArgumentNullCheck(list);
                list.Sort(priorityComparer);
            }
        }
        
        private static readonly object staticLock = new object();
        private static readonly DrawNearToFarComparer<BaseSurface> nearToFarComparer = new DrawNearToFarComparer<BaseSurface>(Vector3.Zero);
        private static readonly DrawFarToNearComparer<BaseSurface> farToNearComparer = new DrawFarToNearComparer<BaseSurface>(Vector3.Zero);
        private static readonly DrawPriorityComparer priorityComparer = new DrawPriorityComparer();
    }
}

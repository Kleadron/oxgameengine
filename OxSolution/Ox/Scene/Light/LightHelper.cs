using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Scene.Component;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// Helper methods for various lighting tasks.
    /// </summary>
    public static class LightHelper
    {
        /// <summary>
        /// Sort a list of point lights by distance from an origin.
        /// </summary>
        /// <param name="list">The list of point lights to be sorted.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <param name="sortOrder">The order in which sorting takes place.</param>
        public static void DistanceSort(this List<PointLight> list, Vector3 origin, SpatialSortOrder sortOrder)
        {
            lock (lockObject)
            {
                OxHelper.ArgumentNullCheck(list);
                IDistanceComparer<PointLight> comparer =
                    sortOrder == SpatialSortOrder.FarToNear ?
                    farToNearComparer : nearToFarComparer;
                comparer.Origin = origin;
                list.Sort(comparer);
            }
        }
        
        /// <summary>
        /// According to .NET standards, all static methods must be thread-safe. Ergo, this lock
        /// object is used to achieve such.
        /// </summary>
        private static readonly object lockObject = new object();
        private static readonly IDistanceComparer<PointLight> nearToFarComparer = new NearToFarComparer<PointLight>(Vector3.Zero);
        private static readonly IDistanceComparer<PointLight> farToNearComparer = new FarToNearComparer<PointLight>(Vector3.Zero);

    }
}

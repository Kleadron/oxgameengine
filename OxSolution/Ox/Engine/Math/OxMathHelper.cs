using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// The direction in which spatial sorting takes place.
    /// </summary>
    public enum SpatialSortOrder
    {
        NearToFar = 0,
        FarToNear
    }

    /// <summary>
    /// Compares the distance of components from an origin.
    /// </summary>
    public interface IDistanceComparer<T> : IComparer<T>
        where T : TransformableComponent
    {
        /// <summary>
        /// The origin from which distance is calculated.
        /// </summary>
        Vector3 Origin { get; set; }
    }

    /// <summary>
    /// Compares the distance of components from an origin, sorting in ascending order.
    /// </summary>
    public class NearToFarComparer<T> : IDistanceComparer<T>
        where T : TransformableComponent
    {
        public NearToFarComparer(Vector3 origin)
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
            return OxMathHelper.Compare(
                x.DistanceSquared(origin),
                y.DistanceSquared(origin));
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Compares the distance of components from an origin, sorting in descending order.
    /// </summary>
    public class FarToNearComparer<T> : IDistanceComparer<T>
        where T : TransformableComponent
    {
        public FarToNearComparer(Vector3 origin)
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
            return OxMathHelper.InvertedCompare(
                x.DistanceSquared(origin),
                y.DistanceSquared(origin));
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Compares the distance of subcomponents from an origin.
    /// </summary>
    public interface IDistanceComparer2<T> : IComparer<T>
        where T : BaseTransformableSubcomponent
    {
        /// <summary>
        /// The origin from which distance is calculated.
        /// </summary>
        Vector3 Origin { get; set; }
    }

    /// <summary>
    /// Compares the distance of subcomponents from an origin, sorting in ascending order.
    /// </summary>
    public class NearToFarComparer2<T> : IDistanceComparer2<T>
        where T : BaseTransformableSubcomponent
    {
        public NearToFarComparer2(Vector3 origin)
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
            return OxMathHelper.Compare(
                x.DistanceSquared(origin),
                y.DistanceSquared(origin));
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Compares the distance of subcomponents from an origin, sorting in descending order.
    /// </summary>
    public class FarToNearComparer2<T> : IDistanceComparer2<T>
        where T : BaseTransformableSubcomponent
    {
        public FarToNearComparer2(Vector3 origin)
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
            return OxMathHelper.InvertedCompare(
                x.DistanceSquared(origin),
                y.DistanceSquared(origin));
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Compares the distance of component tokens from an origin, sorting in ascending order.
    /// </summary>
    public class NearToFarComparer3<T> : IComparer<T> where T : TransformableComponentToken
    {
        public NearToFarComparer3(Vector3 origin)
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
            return OxMathHelper.Compare(
                x.DistanceSquared(origin),
                y.DistanceSquared(origin));
        }

        private Vector3 origin;
    }

    /// <summary>
    /// Provides various helpful methods for general math problems not already covered by XNA and
    /// .NET's core math libraries.
    /// </summary>
    public static class OxMathHelper
    {
        /// <summary>
        /// Compare the values of two ints.
        /// </summary>
        public static int Compare(int x, int y)
        {
            if (x < y) return -1;
            if (x == y) return 0;
            return 1;
        }

        /// <summary>
        /// Compare the values of two floats.
        /// </summary>
        public static int Compare(float x, float y)
        {
            if (x < y) return -1;
            if (x == y) return 0;
            return 1;
        }

        /// <summary>
        /// Compare the values of two ints in an inverted manner.
        /// </summary>
        public static int InvertedCompare(int x, int y)
        {
            if (x < y) return 1;
            if (x == y) return 0;
            return -1;
        }

        /// <summary>
        /// Compare the values of two floats in an inverted manner.
        /// </summary>
        public static int InvertedCompare(float x, float y)
        {
            if (x < y) return 1;
            if (x == y) return 0;
            return -1;
        }

        /// <summary>
        /// Calculate the component vector of two vectors.
        /// </summary>
        public static Vector3 ComponentVector(Vector3 u, Vector3 v)
        {
            return Vector3.Dot(u, v) * v;
        }

        /// <summary>
        /// Calculate the projection of v onto u.
        /// </summary>
        public static Vector3 Projection(Vector3 u, Vector3 v)
        {
            /* Formula:
             * 
             *   u dot v
             * ( ------- ) u
             *   u dot u
             */
            float uDotV; Vector3.Dot(ref u, ref v, out uDotV);
            float uDotU; Vector3.Dot(ref u, ref u, out uDotU);
            float uDotVOverUDotU = uDotV / uDotU;
            return uDotVOverUDotU * u;
        }

        /// <summary>
        /// Calculate the minimum angle between two vectors.
        /// </summary>
        public static float AngleBetween(Vector3 v1, Vector3 v2)
        {
            float v1DotV2; Vector3.Dot(ref v1, ref v2, out v1DotV2);
            float v1LengthTimesV2Length = v1.Length() * v2.Length();
            return (float)Math.Acos(v1DotV2 / v1LengthTimesV2Length);
        }

        /// <summary>
        /// Are the numbers both positive or both negative?
        /// </summary>
        public static bool SameSign(float a, float b)
        {
            return (a >= 0 && b >= 0) || (a <= 0 && b <= 0);
        }

        /// <summary>
        /// Is a point inside the triangle formed by a, b, and c?
        /// </summary>
        public static bool Inside(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
        {
            // BUG: This code is not working very well and is probably ill-conceived.
            Vector3 ab = b - a;
            Vector3 ap = point - a;
            float apDotAb = Vector3.Dot(ap, ab);
            if (!SameSign(apDotAb, ab.Length())) return false;
            Vector3 bc = c - b;
            Vector3 bp = point - b;
            float bpDotBc = Vector3.Dot(bp, bc);
            if (!SameSign(bpDotBc, bc.Length())) return false;
            Vector3 ca = a - c;
            Vector3 cp = point - c;
            float cpDotCa = Vector3.Dot(cp, ca);
            if (!SameSign(cpDotCa, ca.Length())) return false;
            return true;
        }

        /// <summary>
        /// Is the line segment intersecting the disc?
        /// </summary>
        public static bool Intersection(ref Segment path, ref Disc disc, out Vector3 intersection)
        {
            intersection = Vector3.Zero;
            Vector3 s = path.Start;
            Vector3 e = path.End;
            Vector3 se = e - s;
            Vector3 p = disc.Center;
            float r = disc.Radius;
            if (se == Vector3.Zero) return false;
            Vector3 seNormal = Vector3.Normalize(se);
            Ray ray = new Ray(s, seNormal);
            Plane plane = PlaneHelper.CreateFromPositionAndNormal(disc.Center, disc.Normal);
            bool rayPlaneIntersection = Intersection(ref ray, ref plane, out intersection);
            return
                rayPlaneIntersection &&
                (intersection - s).Length() <= se.Length() &&
                (intersection - p).Length() <= r;
        }

        /// <summary>
        /// Is the ray intersecting the plane?
        /// </summary>
        public static bool Intersection(ref Ray ray, ref Plane plane, out Vector3 intersection)
        {
            intersection = Vector3.Zero;
            Vector3 p1 = ray.Position;
            Vector3 n1 = ray.Direction;
            Vector3 p2 = plane.Position();
            Vector3 n2 = plane.Normal;
            Vector3 d = p2 - p1;
            if (Vector3.Dot(n1, n2) == 0 && p1 == p2) return true;
            float t = Vector3.Dot(p1 - p2, n2) / Vector3.Dot(n1, n2);
            intersection = p1 - n1 * t;
            return true;
        }

        /// <summary>
        /// Is the ray interseting the triangle?
        /// </summary>
        public static bool Intersection(ref Ray ray, ref Triangle triangle, out Vector3 intersection)
        {
            Plane plane = PlaneHelper.CreateFromPositionAndNormal(triangle.A, triangle.Normal);
            bool rayPlaneIntersection = Intersection(ref ray, ref plane, out intersection);
            return
                rayPlaneIntersection &&
                OxMathHelper.Inside(intersection, triangle.A, triangle.B, triangle.C);
        }

        /// <summary>
        /// Sort a list of subcomponents by distance from an origin.
        /// </summary>
        /// <param name="subcomponents">The subcomponents to be sorted.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <param name="sortOrder">The order in which sorting takes place.</param>
        public static void DistanceSort(this List<BaseTransformableSubcomponent> subcomponents,
            Vector3 origin, SpatialSortOrder sortOrder)
        {
            lock (lockObject)
            {
                OxHelper.ArgumentNullCheck(subcomponents);
                IDistanceComparer2<BaseTransformableSubcomponent> comparer;
                if (sortOrder == SpatialSortOrder.FarToNear) comparer = farToNearComparer;
                else comparer = nearToFarComparer;
                comparer.Origin = origin;
                subcomponents.Sort(comparer);
            }
        }

        /// <summary>
        /// Sort a list of component tokens from far to near.
        /// </summary>
        /// <param name="tokens">The component tokens to be sorted.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        public static void FarToNearSort(this List<TransformableComponentToken> tokens, Vector3 origin)
        {
            lock (lockObject)
            {
                OxHelper.ArgumentNullCheck(tokens);
                tokenFarToNearComparer.Origin = origin;
                tokens.Sort(tokenFarToNearComparer);
            }
        }

        /// <summary>
        /// Get the squared distance of a component from an origin.
        /// </summary>
        /// <param name="component">The component to find the distance of.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <returns>The squared distance.</returns>
        public static float DistanceSquared(this TransformableComponent component, Vector3 origin)
        {
            OxHelper.ArgumentNullCheck(component);
            float result;
            Vector3 boxCenter = component.BoundingBoxWorld.GetCenter();
            Vector3.DistanceSquared(ref origin, ref boxCenter, out result);
            return result;
        }

        /// <summary>
        /// Get the squared distance of a subcomponent from an origin.
        /// </summary>
        /// <param name="subcomponent">The subcomponent to find the distance of.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <returns>The squared distance.</returns>
        public static float DistanceSquared(this BaseTransformableSubcomponent subcomponent, Vector3 origin)
        {
            OxHelper.ArgumentNullCheck(subcomponent);
            float result;
            Vector3 boxCenter = subcomponent.BoundingBoxWorld.GetCenter();
            Vector3.DistanceSquared(ref origin, ref boxCenter, out result);
            return result;
        }

        /// <summary>
        /// Get the squared distance of a component token from an origin.
        /// </summary>
        /// <param name="token">The component token to find the distance of.</param>
        /// <param name="origin">The origin from which distance is measured.</param>
        /// <returns>The squared distance.</returns>
        public static float DistanceSquared(this TransformableComponentToken token, Vector3 origin)
        {
            OxHelper.ArgumentNullCheck(token);
            OxHelper.ArgumentNullCheck(token.Instance);
            float result;
            Vector3 boxCenter = token.Instance.BoundingBoxWorld.GetCenter();
            Vector3.DistanceSquared(ref origin, ref boxCenter, out result);
            return result;
        }

        /// <summary>
        /// Return position of the transformable closest to a position.
        /// </summary>
        public static int GetClosest(this IList<TransformableComponent> items, Vector3 position)
        {
            OxHelper.ArgumentNullCheck(items);
            float smallestDistance = float.MaxValue;
            int closestObjIndex = 0;
            for (int i = 0; i < items.Count; ++i)
            {
                TransformableComponent item = items[i];
                float distance = (item.PositionWorld - position).LengthSquared();
                if (distance >= smallestDistance) continue;
                smallestDistance = distance;
                closestObjIndex = i;
            }

            return closestObjIndex;
        }

        /// <summary>
        /// Return position of the subcomponent furthest from a position.
        /// </summary>
        public static int GetFarthest(this IList<BaseTransformableSubcomponent> subcomponents, Vector3 position)
        {
            OxHelper.ArgumentNullCheck(subcomponents);
            float greatestDistance = 0;
            int farthestObjIndex = 0;
            for (int i = 0; i < subcomponents.Count; ++i)
            {
                BaseTransformableSubcomponent subcomponent = subcomponents[i];
                float distance = (subcomponent.PositionWorld - position).LengthSquared();
                if (distance <= greatestDistance) continue;
                greatestDistance = distance;
                farthestObjIndex = i;
            }

            return farthestObjIndex;
        }

        /// <summary>
        /// Find the point on the viewport at the specified point in terms of virtual resolution.
        /// </summary>
        public static Vector2 FromVirtual(this Viewport viewport, Vector2 point)
        {
            Vector2 viewportScale = new Vector2(viewport.Width, viewport.Height);
            return point / OxConfiguration.VirtualScreen.Scale * viewportScale;
        }

        /// <summary>
        /// Find the segment that starts at the near plane and ends at the far plane from the
        /// perspective of a point on the viewport.
        /// </summary>
        public static Segment ToWorld(this Viewport viewport, Camera camera, Vector2 point)
        {
            Vector2 positionClipPlane = new Vector2(
                2.0f * (point.X / viewport.Width),
                2.0f * (point.Y / viewport.Height));
            positionClipPlane.X -= 1.0f;
            positionClipPlane.Y = 1.0f - positionClipPlane.Y;
            Vector3 start = camera.ClipPlaneToWorld(new Vector3(positionClipPlane, 0.0f));
            Vector3 end = camera.ClipPlaneToWorld(new Vector3(positionClipPlane, 1.0f));
            return new Segment(start, end);
        }

        /// <summary>
        /// Get two arbitrary vectors that, given the incoming vector, create a well-formed
        /// orientation matrix.
        /// </summary>
        public static void GetComplimentaryOrientationVectors(this Vector3 v1, out Vector3 v2, out Vector3 v3)
        {
            if (Vector3.Cross(v1, Vector3.Left) != Vector3.Zero)
            {
                v2 = Vector3.Normalize(Vector3.Cross(v1, Vector3.Left));
                v3 = Vector3.Normalize(Vector3.Cross(v1, v2));
            }
            else
            {
                v2 = Vector3.Normalize(Vector3.Cross(v1, Vector3.Down));
                v3 = Vector3.Normalize(Vector3.Cross(v1, v2));
            }
        }

        public static float GetSnap(this float value, float snap)
        {
            if (snap == 0) return value;
            float low = value - value % snap;
            float high = low + snap;
            float halfSnap = snap * 0.5f;
            return value - low < halfSnap ? low : high;
        }

        public static Vector2 GetSnap(this Vector2 value, float snap)
        {
            return value.GetSnap(new Vector2(snap));
        }

        public static Vector2 GetSnap(this Vector2 value, Vector2 snap)
        {
            return new Vector2(
                value.X.GetSnap(snap.X),
                value.Y.GetSnap(snap.Y));
        }

        public static Vector3 GetSnap(this Vector3 value, float snap)
        {
            return value.GetSnap(new Vector3(snap));
        }

        public static Vector3 GetSnap(this Vector3 value, Vector3 snap)
        {
            return new Vector3(
                value.X.GetSnap(snap.X),
                value.Y.GetSnap(snap.Y),
                value.Z.GetSnap(snap.Z));
        }

        public static Vector4 GetSnap(this Vector4 value, float snap)
        {
            return value.GetSnap(new Vector4(snap));
        }

        public static Vector4 GetSnap(this Vector4 value, Vector4 snap)
        {
            return new Vector4(
                value.X.GetSnap(snap.X),
                value.Y.GetSnap(snap.Y),
                value.Z.GetSnap(snap.Z),
                value.W.GetSnap(snap.W));
        }

        /// <summary>
        /// According to .NET standards, all static methods must be thread-safe. Ergo, this lock
        /// object is used to achieve such.
        /// </summary>
        private static readonly object lockObject = new object();
        private static readonly NearToFarComparer2<BaseTransformableSubcomponent> nearToFarComparer = new NearToFarComparer2<BaseTransformableSubcomponent>(Vector3.Zero);
        private static readonly FarToNearComparer2<BaseTransformableSubcomponent> farToNearComparer = new FarToNearComparer2<BaseTransformableSubcomponent>(Vector3.Zero);
        private static readonly NearToFarComparer3<TransformableComponentToken> tokenFarToNearComparer = new NearToFarComparer3<TransformableComponentToken>(Vector3.Zero);
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Helper methods for BoundingBox.
    /// </summary>
    public static class BoundingBoxHelper
    {
        /// <summary>
        /// Create an all-encompassing bounding box.
        /// </summary>
        public static BoundingBox CreateAllEncompassing
        {
            get { return allEncompassing; }
        }

        /// <summary>
        /// Create a bounding box from a box primitive.
        /// </summary>
        public static BoundingBox CreateFromBox(Box box)
        {
            return new BoundingBox(box.Min, box.Max);
        }

        /// <summary>
        /// Create a bounding box that encompasses a set of points. Does not create garbage when
        /// used.
        /// </summary>
        public static BoundingBox GenerateBoundingBox(this IList<Vector3> points)
        {
            OxHelper.ArgumentNullCheck(points);
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);
            if (points.Count == 0)
            {
                min = new Vector3(float.MinValue);
                max = new Vector3(float.MaxValue);
            }
            else
            {
                for (int i = 0; i < points.Count; ++i)
                {
                    Vector3 point = points[i];
                    Vector3.Min(ref min, ref point, out min);
                    Vector3.Max(ref max, ref point, out max);
                }
            }

            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Transform a bounding box.
        /// </summary>
        public static void Transform(ref BoundingBox box, ref Matrix transform, out BoundingBox result)
        {
            /*
             * These are the cube points we're constructing -
             * 
             *  E-----G
             *  |\    |\
             *  | C-----A
             *  F-|---H |
             *   \|    \|
             *    D-----B
             * 
             * A == box.Max
             * F == box.Min
             */

            Vector3 min = box.Min;
            Vector3 max = box.Max;
            Vector3 a = new Vector3(max.X, max.Y, max.Z);
            Vector3 b = new Vector3(max.X, min.Y, max.Z);
            Vector3 c = new Vector3(min.X, max.Y, max.Z);
            Vector3 d = new Vector3(min.X, min.Y, max.Z);
            Vector3 e = new Vector3(min.X, max.Y, min.Z);
            Vector3 f = new Vector3(min.X, min.Y, min.Z);
            Vector3 g = new Vector3(max.X, max.Y, min.Z);
            Vector3 h = new Vector3(max.X, min.Y, min.Z);
            Vector3 a2; Vector3.Transform(ref a, ref transform, out a2);
            Vector3 b2; Vector3.Transform(ref b, ref transform, out b2);
            Vector3 c2; Vector3.Transform(ref c, ref transform, out c2);
            Vector3 d2; Vector3.Transform(ref d, ref transform, out d2);
            Vector3 e2; Vector3.Transform(ref e, ref transform, out e2);
            Vector3 f2; Vector3.Transform(ref f, ref transform, out f2);
            Vector3 g2; Vector3.Transform(ref g, ref transform, out g2);
            Vector3 h2; Vector3.Transform(ref h, ref transform, out h2);
            result.Min = Vector3.Min(a2, Vector3.Min(b2, Vector3.Min(c2, Vector3.Min(d2, Vector3.Min(e2, Vector3.Min(f2, Vector3.Min(g2, h2)))))));
            result.Max = Vector3.Max(a2, Vector3.Max(b2, Vector3.Max(c2, Vector3.Max(d2, Vector3.Max(e2, Vector3.Max(f2, Vector3.Max(g2, h2)))))));
        }

        /// <summary>
        /// Calculate the scale of the bounding box.
        /// </summary>
        public static Vector3 GetScale(this BoundingBox box)
        {
            return box.Max - box.Min;
        }

        /// <summary>
        /// Calculate the center of the bounding box.
        /// </summary>
        public static Vector3 GetCenter(this BoundingBox box)
        {
            return (box.Min + box.Max) * 0.5f;
        }

        /// <summary>
        /// Calculate the extent of the bounding box (the vector from the center to max).
        /// </summary>
        public static Vector3 GetExtent(this BoundingBox box)
        {
            return box.Max - GetCenter(box);
        }

        /// <summary>
        /// Is a bounding box all-encompassing?
        /// </summary>
        public static bool AllEncompassing(this BoundingBox boundingBox)
        {
            return
                boundingBox.Min == new Vector3(float.MinValue) &&
                boundingBox.Max == new Vector3(float.MaxValue);
        }

        /// <summary>
        /// Transform a bounding box.
        /// </summary>
        public static BoundingBox Transform(this BoundingBox box, ref Matrix transform)
        {
            BoundingBox result;
            Transform(ref box, ref transform, out result);
            return result;
        }

        public static bool Intersects(this BoundingBox box, ref Segment segment)
        {
            // this algorithm was straight-up thudged from -
            // http://www.gamasutra.com/features/19991018/Gomez_6.htm

            Vector3 segmentMid = (segment.Start + segment.End) * 0.5f;
            Vector3 segmentDirection = Vector3.Normalize(segment.End - segment.Start);
            float segmentHalfLength = (segment.End - segmentMid).Length();
            Vector3 boxMid = (box.Min + box.Max) * 0.5f;
            Vector3 boxExtent = box.Max - boxMid;
            Vector3 a = boxMid - segmentMid;

            // do any of the principal axes form a separating axis?
            if (Math.Abs(a.X) > boxExtent.X + segmentHalfLength * Math.Abs(segmentDirection.X)) return false;
            if (Math.Abs(a.Y) > boxExtent.Y + segmentHalfLength * Math.Abs(segmentDirection.Y)) return false;
            if (Math.Abs(a.Z) > boxExtent.Z + segmentHalfLength * Math.Abs(segmentDirection.Z)) return false;

            // NOTE: Since the separating axis is perpendicular to the line in these last four
            // cases, the line does not contribute to the projection.

            float b =
                boxExtent.Y * Math.Abs(segmentDirection.Z) +
                boxExtent.Z * Math.Abs(segmentDirection.Y);

            if (Math.Abs(a.Y * segmentDirection.Z - a.Z * segmentDirection.Y) > b) return false;

            b = boxExtent.X * Math.Abs(segmentDirection.Z) +
                boxExtent.Z * Math.Abs(segmentDirection.X);

            if (Math.Abs(a.Z * segmentDirection.X - a.X * segmentDirection.Z) > b) return false;

            b = boxExtent.X * Math.Abs(segmentDirection.Y) +
                boxExtent.Y * Math.Abs(segmentDirection.X);

            if (Math.Abs(a.X * segmentDirection.Y - a.Y * segmentDirection.X) > b) return false;

            return true;
        }

        public static bool Intersects(this BoundingBox box, ref Segment segment, out float tinter)
        {
            // this algorithm was hood-snatched from -
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=338987
            tinter = 0;
            float tenter = 0.0f, texit = 1.0f;
            if (!RaySlabIntersects(box.Min.X, box.Max.X, segment.Start.X, segment.End.X, ref tenter, ref texit)) return false;
            if (!RaySlabIntersects(box.Min.Y, box.Max.Y, segment.Start.Y, segment.End.Y, ref tenter, ref texit)) return false;
            if (!RaySlabIntersects(box.Min.Z, box.Max.Z, segment.Start.Z, segment.End.Z, ref tenter, ref texit)) return false;
            tinter = tenter;
            return true;
        }

        private static bool RaySlabIntersects(float slabmin, float slabmax, float raystart, float rayend, ref float tbenter, ref float tbexit)
        {
            // this algorithm was snucked from -
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=338987

            // calculate the ray direction
            float raydir = rayend - raystart;

            // ray parallel to the slab
            if (Math.Abs(raydir) < 1.0E-9f)
            {
                // ray parallel to the slab, but ray not inside the slab planes
                if (raystart < slabmin || raystart > slabmax) return false;
                // ray parallel to the slab, but ray inside the slab planes
                return true;
            }

            // slab's enter and exit parameters
            float tsenter = (slabmin - raystart) / raydir;
            float tsexit = (slabmax - raystart) / raydir;

            // order the enter / exit values.
            if (tsenter > tsexit) OxHelper.Swap(ref tsenter, ref tsexit);

            // make sure the slab interval and the current box intersection interval overlap
            if (tbenter > tsexit || tsenter > tbexit) return false; // Ray missed the box.
            // yep, the slab and current intersection interval overlap
            else
            {
                // update the intersection interval
                tbenter = MathHelper.Max(tbenter, tsenter);
                tbexit = MathHelper.Min(tbexit, tsexit);
                return true;
            }
        }

        private static readonly BoundingBox allEncompassing =
            new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));
    }
}

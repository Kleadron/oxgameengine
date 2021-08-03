using System;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Represents a transformable line segment.
    /// </summary>
    public class SegmentPrimitive : IPrimitive
    {
        public SegmentPrimitive(Segment segment)
        {
            this.Segment = segment;
        }

        public SegmentPrimitive(Vector3 start, Vector3 end) : this(new Segment(start, end)) { }

        public SegmentPrimitive() { }

        public Segment Segment
        {
            get { return _segment; }
            set
            {
                if (_segment.Equals(value)) return;
                _segment = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }

        public Vector3 Start
        {
            get { return _segment.Start; }
            set
            {
                if (_segment.Start == value) return;
                _segment.Start = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }

        public Vector3 End
        {
            get { return _segment.End; }
            set
            {
                if (_segment.End == value) return;
                _segment.End = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }

        public BoundingBox BoundingBox
        {
            get { return new BoundingBox(Vector3.Min(Start, End), Vector3.Max(Start, End)); }
        }

        public float Area { get { return 0; } }

        public event Action<IPrimitive> BoundingBoxChanged;

        public IPrimitive Clone()
        {
            return new SegmentPrimitive(Segment);
        }

        public bool TryCopy(IPrimitive source)
        {
            OxHelper.ArgumentNullCheck(source);
            SegmentPrimitive sourceSegment = source as SegmentPrimitive;
            if (sourceSegment == null) return false;
            Segment = sourceSegment.Segment;
            return true;
        }

        public void ApplyTranslation(Vector3 translation)
        {
            Matrix transform = Matrix.Identity;
            transform.Translation = translation;
            ApplyTransformation(ref transform);
        }

        public void ApplyOrientation(Matrix orientation)
        {
            Matrix transform = Matrix.Identity;
            transform.Right = orientation.Right;
            transform.Up = orientation.Up;
            transform.Forward = orientation.Forward;
            ApplyTransformation(ref transform);
        }

        public void ApplyTransformation(ref Matrix transform)
        {
            Vector3 start = Start;
            Vector3 end = End;
            Vector3 transformedStart;
            Vector3 transformedEnd;
            Vector3.Transform(ref start, ref transform, out transformedStart);
            Vector3.Transform(ref end, ref transform, out transformedEnd);
            Segment = new Segment(transformedStart, transformedEnd);
        }

        private Segment _segment;
    }
}

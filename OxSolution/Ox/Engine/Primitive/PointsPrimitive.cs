using System;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Represents an array of transformable points.
    /// </summary>
    public class PointsPrimitive : IPrimitive, IIndexable<Vector3>
    {
        public PointsPrimitive(Vector3[] points)
        {
            OxHelper.ArgumentNullCheck(points);
            SetPoints(points);
        }

        public PointsPrimitive(int length)
        {
            _points = new Vector3[length];
        }

        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            private set
            {
                if (_boundingBox == value) return;
                _boundingBox = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public Vector3 this[int index]
        {
            get { return _points[index]; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_points[index] == value) return;
                _points[index] = value;
                UpdateBoundingBox();
            }
        }
        
        public float Area { get { return 0; } }
        
        public int Count { get { return _points.Length; } }

        public event Action<IPrimitive> BoundingBoxChanged;

        public IPrimitive Clone()
        {
            return new PointsPrimitive(_points);
        }

        public bool TryCopy(IPrimitive source)
        {
            OxHelper.ArgumentNullCheck(source);
            PointsPrimitive sourcePoints = source as PointsPrimitive;
            if (sourcePoints == null) return false;
            SetPoints(sourcePoints.GetPointsHack());
            return true;
        }

        public Vector3[] GetPoints()
        {
            Vector3[] result = new Vector3[_points.Length];
            _points.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// HACK: get a reference directly to the primitive's points.
        /// </summary>
        internal Vector3[] GetPointsHack()
        {
            return _points;
        }

        public void SetPoints(Vector3[] points)
        {
            OxHelper.ArgumentNullCheck(points);
            if (_points.Length != points.Length) _points = new Vector3[points.Length];
            points.CopyTo(this._points, 0);
            UpdateBoundingBox();
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
            Vector3.Transform(_points, ref transform, _points);
            UpdateBoundingBox();
        }

        private void UpdateBoundingBox()
        {
            BoundingBox = _points.GenerateBoundingBox();
        }

        private BoundingBox _boundingBox;
        private Vector3[] _points;
    }
}

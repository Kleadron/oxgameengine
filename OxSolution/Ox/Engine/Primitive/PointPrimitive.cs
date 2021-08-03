using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Represents a transformable point.
    /// </summary>
    public class PointPrimitive : IPrimitive
    {
        public PointPrimitive(Vector3 position)
        {
            this.Position = position;
        }

        public PointPrimitive() { }

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }

        public event Action<IPrimitive> BoundingBoxChanged;

        public BoundingBox BoundingBox { get { return new BoundingBox(Position, Position); } }

        public float Area { get { return 0; } }

        public IPrimitive Clone()
        {
            return new PointPrimitive(Position);
        }

        public bool TryCopy(IPrimitive source)
        {
            OxHelper.ArgumentNullCheck(source);
            PointPrimitive sourcePoint = source as PointPrimitive;
            if (sourcePoint == null) return false;
            Position = sourcePoint.Position;
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
            Vector3 position = this.Position;
            Vector3 transformedPosition;
            Vector3.Transform(ref position, ref transform, out transformedPosition);
            this.Position = transformedPosition;
        }

        private Vector3 _position;
    }
}

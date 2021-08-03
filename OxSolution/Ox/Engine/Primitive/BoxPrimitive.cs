using System;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Represents a transformable box.
    /// </summary>
    public class BoxPrimitive : IPrimitive
    {
        public BoxPrimitive(Box box)
        {
            Box = box;
        }

        public BoxPrimitive(BoundingBox boundingBox) : this(new Box(boundingBox)) { }

        public BoxPrimitive(Vector3 center, Vector3 extent) : this(new Box(center, extent)) { }

        public BoxPrimitive() { }

        public Box Box
        {
            get { return _box; }
            set
            {
                if (_box.Equals(value)) return;
                _box = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public Vector3 Center
        {
            get { return _box.Center; }
            set
            {
                if (_box.Center == value) return;
                _box.Center = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public Vector3 Extent
        {
            get { return _box.Extent; }
            set
            {
                if (_box.Extent == value) return;
                _box.Extent = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public Vector3 Min { get { return Center - Extent; } }

        public Vector3 Max { get { return Center + Extent; } }

        public BoundingBox BoundingBox { get { return new BoundingBox(Min, Max); } }

        public float Area
        {
            get
            {
                Vector3 min = Min; // OPTIMIZATION: cache property
                Vector3 max = Max; // OPTIMIZATION: cache property
                float width = max.X - min.X;
                float height = max.Y - min.Y;
                float depth = max.Z - min.Z;
                return width * height * depth;
            }
        }

        public event Action<IPrimitive> BoundingBoxChanged;

        public IPrimitive Clone()
        {
            return new BoxPrimitive(Box);
        }

        public bool TryCopy(IPrimitive source)
        {
            OxHelper.ArgumentNullCheck(source);
            BoxPrimitive sourceBox = source as BoxPrimitive;
            if (sourceBox == null) return false;
            Box = sourceBox.Box;
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
            BoundingBox box = new BoundingBox(Min, Max);
            BoundingBox transformedBox;
            BoundingBoxHelper.Transform(ref box, ref transform, out transformedBox);
            Center = transformedBox.GetCenter();
            Extent = transformedBox.GetExtent();
        }

        private Box _box;
    }
}

using System;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Represents a transformable sphere.
    /// </summary>
    public class SpherePrimitive : IPrimitive
    {
        public SpherePrimitive(BoundingSphere sphere)
        {
            this.Sphere = sphere;
        }

        public SpherePrimitive(Vector3 center, float radius)
            : this(new BoundingSphere(center, radius)) { }

        public SpherePrimitive() { }

        public BoundingSphere Sphere
        {
            get { return _sphere; }
            set
            {
                if (_sphere == value) return;
                _sphere = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }

        public Vector3 Center
        {
            get { return _sphere.Center; }
            set
            {
                if (_sphere.Center == value) return;
                _sphere.Center = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }

        public float Radius
        {
            get { return _sphere.Radius; }
            set
            {
                if (_sphere.Radius == value) return;
                _sphere.Radius = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }

        public BoundingBox BoundingBox { get { return BoundingBox.CreateFromSphere(Sphere); } }

        public float Area { get { return 4 * MathHelper.Pi * Radius * Radius; } }

        public event Action<IPrimitive> BoundingBoxChanged;

        public IPrimitive Clone()
        {
            return new SpherePrimitive(Sphere);
        }

        public bool TryCopy(IPrimitive source)
        {
            OxHelper.ArgumentNullCheck(source);
            SpherePrimitive sourceSphere = source as SpherePrimitive;
            if (sourceSphere == null) return false;
            Sphere = sourceSphere.Sphere;
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
            Vector3 center = Center;
            float radius = Radius;
            Vector3 radius3 = new Vector3(radius);
            BoundingBox box = new BoundingBox(center - radius3, center + radius3);
            BoundingBox transformedBox;
            BoundingBoxHelper.Transform(ref box, ref transform, out transformedBox);
            Vector3 transformedCenter = transformedBox.GetCenter();
            float transformedRadius = (transformedBox.Max - transformedCenter).Length();
            Sphere = new BoundingSphere(transformedCenter, transformedRadius);
        }

        private BoundingSphere _sphere;
    }
}

using System;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Represents a transformable disc.
    /// </summary>
    public class DiscPrimitive : IPrimitive
    {
        public DiscPrimitive(Disc disc)
        {
            this.Disc = disc;
        }

        public DiscPrimitive(Vector3 center, Vector3 edgePoint1, Vector3 edgePoint2)
            : this(new Disc(center, edgePoint1, edgePoint2)) { }

        public DiscPrimitive() { }

        public Disc Disc
        {
            get { return _disc; }
            set
            {
                if (_disc.Equals(value)) return;
                _disc = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public Vector3 Center
        {
            get { return _disc.Center; }
            set
            {
                if (_disc.Center == value) return;
                _disc.Center = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public Vector3 EdgePoint1
        {
            get { return _disc.EdgePoint1; }
            set
            {
                if (_disc.EdgePoint1 == value) return;
                _disc.EdgePoint1 = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public Vector3 EdgePoint2
        {
            get { return _disc.EdgePoint2; }
            set
            {
                if (_disc.EdgePoint2 == value) return;
                _disc.EdgePoint2 = value;
                if (BoundingBoxChanged != null) BoundingBoxChanged(this);
            }
        }
        
        public BoundingBox BoundingBox
        {
            get
            {
                Vector3 position1 = Center - new Vector3(Radius);
                Vector3 position2 = Center + new Vector3(Radius);
                return new BoundingBox(
                    Vector3.Min(position1, position2),
                    Vector3.Max(position1, position2));
            }
        }
        
        public Vector3 Normal { get { return _disc.Normal; } }

        public float Area { get { return MathHelper.Pi * (float)Math.Pow(Radius, 2); } }

        public float Radius {get { return _disc.Radius; }  }

        public float Diameter { get { return Radius * 2; } }

        public event Action<IPrimitive> BoundingBoxChanged;

        public IPrimitive Clone()
        {
            return new DiscPrimitive(Disc);
        }

        public bool TryCopy(IPrimitive source)
        {
            OxHelper.ArgumentNullCheck(source);
            DiscPrimitive sourceDisc = source as DiscPrimitive;
            if (sourceDisc == null) return false;
            Disc = sourceDisc.Disc;
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
            Vector3 center = this.Center;
            Vector3 edgePoint1 = this.EdgePoint1;
            Vector3 edgePoint2 = this.EdgePoint2;
            Vector3.Transform(ref center, ref transform, out center);
            Vector3.Transform(ref edgePoint1, ref transform, out edgePoint1);
            Vector3.Transform(ref edgePoint2, ref transform, out edgePoint2);
            Disc = new Disc(center, edgePoint1, edgePoint2);
        }

        private Disc _disc;
    }
}

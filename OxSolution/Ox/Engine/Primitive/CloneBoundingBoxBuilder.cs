using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Builds bounding boxes by cloning the source bounding box.
    /// </summary>
    public class CloneBoundingBoxBuilder : IBoundingBoxBuilder
    {
        public BoundingBox BoundingBoxSource { get { return boundingBox; } }

        public BoundingBox BoundingBoxLocal { get { return boundingBox; } }

        public BoundingBox BoundingBoxWorld { get { return boundingBox; } }

        public event Action<IBoundingBoxBuilder> BoundingBoxWorldChanged;

        public bool TrySetBoundingBox(BoundingBox boundingBox)
        {
            if (this.boundingBox == boundingBox) return true;
            this.boundingBox = boundingBox;
            if (BoundingBoxWorldChanged != null) BoundingBoxWorldChanged(this);
            return true;
        }

        public void GetTransformLocal(out Matrix transform)
        {
            transform = this.transformLocal;
        }

        public void SetTransformLocal(ref Matrix transform)
        {
            this.transformLocal = transform;
        }

        public void GetTransformWorld(out Matrix transform)
        {
            transform = this.transformWorld;
        }

        public void SetTransformWorld(ref Matrix transform)
        {
            this.transformWorld = transform;
        }

        private BoundingBox boundingBox;
        private Matrix transformLocal = Matrix.Identity;
        private Matrix transformWorld = Matrix.Identity;
    }
}

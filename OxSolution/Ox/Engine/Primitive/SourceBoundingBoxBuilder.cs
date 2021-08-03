using System;
using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Builds hierarchically-transformed bounding boxes from a source bounding box.
    /// </summary>
    public class SourceBoundingBoxBuilder : IBoundingBoxBuilder
    {
        public BoundingBox BoundingBoxSource { get { return source; } }

        public BoundingBox BoundingBoxLocal { get { return local; } }

        public BoundingBox BoundingBoxWorld { get { return world; } }

        public event Action<IBoundingBoxBuilder> BoundingBoxWorldChanged;

        public bool TrySetBoundingBox(BoundingBox boundingBox)
        {
            if (source == boundingBox) return true;
            source = boundingBox;
            BoundingBoxHelper.Transform(ref source, ref transformLocal, out local);
            BoundingBoxHelper.Transform(ref source, ref transformWorld, out world);
            if (BoundingBoxWorldChanged != null) BoundingBoxWorldChanged(this);
            return true;
        }

        public void GetTransformLocal(out Matrix transform)
        {
            transform = transformLocal;
        }

        public void SetTransformLocal(ref Matrix transform)
        {
            if (transformLocal == transform) return;
            transformLocal = transform;
            BoundingBoxHelper.Transform(ref source, ref transformLocal, out local);
            if (BoundingBoxWorldChanged != null) BoundingBoxWorldChanged(this);
        }

        public void GetTransformWorld(out Matrix transform)
        {
            transform = transformWorld;
        }

        public void SetTransformWorld(ref Matrix transform)
        {
            if (transformWorld == transform) return;
            transformWorld = transform;
            BoundingBoxHelper.Transform(ref source, ref transformWorld, out world);
            if (BoundingBoxWorldChanged != null) BoundingBoxWorldChanged(this);
        }

        private BoundingBox source;
        private BoundingBox local;
        private BoundingBox world;
        private Matrix transformLocal = Matrix.Identity;
        private Matrix transformWorld = Matrix.Identity;
    }
}

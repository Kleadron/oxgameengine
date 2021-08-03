using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.Primitive
{
    /// <summary>
    /// Builds hierarchically-transformed bounding boxes from a source primitive's bounding box.
    /// </summary>
    public class PrimitiveBoundingBoxBuilder : IBoundingBoxBuilder
    {
        public PrimitiveBoundingBoxBuilder(IPrimitive primitive)
        {
            OxHelper.ArgumentNullCheck(primitive);
            PrimitiveSource = primitive;
        }

        public BoundingBox BoundingBoxSource { get { return PrimitiveSource.BoundingBox; } }

        public BoundingBox BoundingBoxLocal { get { return _primitiveLocal.BoundingBox; } }

        public BoundingBox BoundingBoxWorld { get { return _primitiveWorld.BoundingBox; } }

        public IPrimitive PrimitiveSource
        {
            get { return _primitiveSource; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_primitiveSource == value) return;
                if (_primitiveSource != null) _primitiveSource.BoundingBoxChanged -= source_BoundingBoxChanged;
                _primitiveSource = value;
                _primitiveLocal = value.Clone();
                _primitiveWorld = value.Clone();
                // BUG: there is no matching -= source_BoundingBoxChanged in this class.
                _primitiveSource.BoundingBoxChanged += source_BoundingBoxChanged;
                UpdateTransform();
            }
        }

        public IPrimitive PrimitiveLocal { get { return _primitiveLocal; } }

        public IPrimitive PrimitiveWorld { get { return _primitiveWorld; } }

        public event Action<IBoundingBoxBuilder> BoundingBoxWorldChanged;

        public bool TrySetBoundingBox(BoundingBox boundingBox)
        {
            return false;
        }

        public void GetTransformLocal(out Matrix transform)
        {
            transform = _transformLocal;
        }

        public void SetTransformLocal(ref Matrix transform)
        {
            if (_transformLocal == transform) return;
            _transformLocal = transform;
            PrimitiveLocal.TryCopy(PrimitiveSource);
            PrimitiveLocal.ApplyTransformation(ref transform);
        }

        public void GetTransformWorld(out Matrix transform)
        {
            transform = _transformWorld;
        }

        public void SetTransformWorld(ref Matrix transform)
        {
            if (_transformWorld == transform) return;
            _transformWorld = transform;
            PrimitiveWorld.TryCopy(PrimitiveSource);
            PrimitiveWorld.ApplyTransformation(ref transform);
            if (BoundingBoxWorldChanged != null) BoundingBoxWorldChanged(this);
        }

        private void source_BoundingBoxChanged(IPrimitive sender)
        {
            UpdateTransform();
            if (BoundingBoxWorldChanged != null) BoundingBoxWorldChanged(this);
        }

        private void UpdateTransform()
        {
            UpdateTransformLocal();
            UpdateTransformWorld();
        }

        private void UpdateTransformLocal()
        {
            PrimitiveLocal.TryCopy(PrimitiveSource);
            Matrix transformLocal;
            GetTransformLocal(out transformLocal);
            PrimitiveLocal.ApplyTransformation(ref transformLocal);
        }

        private void UpdateTransformWorld()
        {
            PrimitiveWorld.TryCopy(PrimitiveSource);
            Matrix transformWorld;
            GetTransformWorld(out transformWorld);
            PrimitiveWorld.ApplyTransformation(ref transformWorld);
        }

        private IPrimitive _primitiveSource;
        private IPrimitive _primitiveLocal;
        private IPrimitive _primitiveWorld;
        private Matrix _transformLocal = Matrix.Identity;
        private Matrix _transformWorld = Matrix.Identity;
    }
}

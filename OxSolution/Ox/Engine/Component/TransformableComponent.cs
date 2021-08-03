using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.Primitive;

namespace Ox.Engine.Component
{
    /// <summary>
    /// A component that can be transformed.
    /// </summary>
    public class TransformableComponent : UpdateableComponent
    {
        /// <summary>
        /// Create a TransformableComponent that is Boundless.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public TransformableComponent(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            BoundingBoxBuilder = new SourceBoundingBoxBuilder();
            Boundless = true;
        }

        /// <summary>
        /// Create a TransformableComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="primitive">The transformable primitive that describes the space occupied by the component.</param>
        public TransformableComponent(OxEngine engine, string domainName, bool ownedByDomain, IPrimitive primitive)
            : base(engine, domainName, ownedByDomain)
        {
            OxHelper.ArgumentNullCheck(primitive);
            BoundingBoxBuilder = new PrimitiveBoundingBoxBuilder(primitive);
        }

        /// <summary>
        /// Create a TransformableComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="boxBuilder">Describes the space occupied by the component.</param>
        public TransformableComponent(OxEngine engine, string domainName, bool ownedByDomain, IBoundingBoxBuilder boxBuilder)
            : base(engine, domainName, ownedByDomain)
        {
            OxHelper.ArgumentNullCheck(boxBuilder);
            BoundingBoxBuilder = boxBuilder;
        }
        
        /// <summary>
        /// The object that builds and updates the components bounding box.
        /// </summary>
        public IBoundingBoxBuilder BoundingBoxBuilder
        {
            get { return _boxBuilder; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_boxBuilder == value) return;
                if (_boxBuilder != null) _boxBuilder.BoundingBoxWorldChanged -= boxBuilder_BoundingBoxWorldChanged;
                _boxBuilder = value;
                _boxBuilder.SetTransformLocal(ref _transformScaled);
                _boxBuilder.SetTransformWorld(ref _transformWorldScaled);
                _boxBuilder.BoundingBoxWorldChanged += boxBuilder_BoundingBoxWorldChanged;
            }
        }
        
        /// <summary>
        /// The bounding box.
        /// </summary>
        public BoundingBox BoundingBox { get { return BoundingBoxBuilder.BoundingBoxLocal; } }

        /// <summary>
        /// The bounding box in world space.
        /// </summary>
        public BoundingBox BoundingBoxWorld { get { return BoundingBoxBuilder.BoundingBoxWorld; } }

        /// <summary>
        /// The bounding box before spatial transformation.
        /// </summary>
        public BoundingBox BoundingBoxSource { get { return BoundingBoxBuilder.BoundingBoxSource; } }

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                UpdateTransform();
            }
        }

        /// <summary>
        /// The position in world space.
        /// </summary>
        public Vector3 PositionWorld
        {
            get { return _positionWorld; }
            set
            {
                Matrix worldToLocal; GetParentWorldToLocal(out worldToLocal);
                Vector3 position; Vector3.Transform(ref value, ref worldToLocal, out position);
                Position = position;
            }
        }

        /// <summary>
        /// The scale.
        /// </summary>
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return;
                _scale = value;
                UpdateTransform();
            }
        }

        /// <summary>
        /// The right vector's orientation.
        /// </summary>
        public Vector3 Right { get { return _orientation.Right; } }

        /// <summary>
        /// The right vector's orientation in world space.
        /// </summary>
        public Vector3 RightWorld { get { return _orientationWorld.Right; } }

        /// <summary>
        /// The up vector's orientation.
        /// </summary>
        public Vector3 Up { get { return _orientation.Up; } }

        /// <summary>
        /// The up vector's orientation in world space.
        /// </summary>
        public Vector3 UpWorld { get { return _orientationWorld.Up; } }

        /// <summary>
        /// The forward vector's orientation.
        /// </summary>
        public Vector3 Forward { get { return _orientation.Forward; } }

        /// <summary>
        /// The forward vector's orientation in world space.
        /// </summary>
        public Vector3 ForwardWorld { get { return _orientationWorld.Forward; } }
        
        /// <summary>
        /// The orientation.
        /// </summary>
        public Matrix Orientation
        {
            get { return _orientation; }
            set { SetOrientation(ref value); }
        }
        
        /// <summary>
        /// The orientation in world space.
        /// </summary>
        public Matrix OrientationWorld
        {
            get { return _orientationWorld; }
            set { SetOrientationWorld(ref value); }
        }

        /// <summary>
        /// The transform with scale applied.
        /// </summary>
        public Matrix TransformScaled { get { return _transformScaled; } }

        /// <summary>
        /// The transform without scale applied.
        /// </summary>
        public Matrix TransformUnscaled { get { return _transformUnscaled; } }

        /// <summary>
        /// The transform in world space with scale applied.
        /// </summary>
        public Matrix TransformWorldScaled { get { return _transformWorldScaled; } }

        /// <summary>
        /// The transform in world space without scale applied.
        /// </summary>
        public Matrix TransformWorldUnscaled { get { return _transformWorldUnscaled; } }
        
        /// <summary>
        /// A matrix to convert from local space to world space (disregards scale).
        /// </summary>
        public Matrix LocalToWorld { get { return _transformWorldUnscaled; } }
        
        /// <summary>
        /// A matrix to convert from world space to local space (disregards scale).
        /// </summary>
        public Matrix WorldToLocal
        {
            get
            {
                Matrix result;
                GetWorldToLocal(out result);
                return result;
            }
        }
        
        /// <summary>
        /// The spatial node that contains this object.
        /// This is an optimization detail.
        /// May be null.
        /// </summary>
        public object SpatialNodeParent
        {
            get { return spatialNodeParent; }
            set { spatialNodeParent = value; }
        }
        
        /// <summary>
        /// Is the component omnipresent?
        /// </summary>
        public bool Boundless
        {
            get { return _boundless; }
            set
            {
                if (_boundless == value) return;
                _boundless = value;
                if (BoundsChanged != null) BoundsChanged(this);
            }
        }

        /// <summary>
        /// The desired point on which to mount.
        /// </summary>
        public int MountPoint
        {
            get { return _mountPoint; }
            set
            {
                if (_mountPoint == value) return;
                int oldMountPoint = _mountPoint;
                _mountPoint = value;
                UpdateTransform();
            }
        }

        /// <summary>
        /// Raised when the bounds are changed.
        /// </summary>
        public event Action<TransformableComponent> BoundsChanged;

        /// <summary>
        /// Raised when the transform is changed.
        /// </summary>
        public event Action<TransformableComponent> TransformChanged;

        /// <summary>
        /// Raised when the transform in world space is changed.
        /// </summary>
        public event Action<TransformableComponent> TransformWorldChanged;

        /// <summary>
        /// Try to set the bounding box.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <returns>True if the bounding box was set.</returns>
        public bool TrySetBoundingBox(BoundingBox boundingBox)
        {
            return BoundingBoxBuilder.TrySetBoundingBox(boundingBox);
        }

        /// <summary>
        /// Get the orientation.
        /// </summary>
        public void GetOrientation(out Matrix orientation)
        {
            orientation = this._orientation;
        }

        /// <summary>
        /// Set the orientation.
        /// </summary>
        public void SetOrientation(ref Matrix orientation)
        {
            if (this._orientation == orientation) return;
            this._orientation = orientation;
            UpdateTransform();
        }

        /// <summary>
        /// Set the orientation.
        /// </summary>
        public void SetOrientation(ref Vector3 right, ref Vector3 up, ref Vector3 forward)
        {
            Matrix orientation = Matrix.Identity;
            orientation.Right = right;
            orientation.Up = up;
            orientation.Forward = forward;
            SetOrientation(ref orientation);
        }

        /// <summary>
        /// Get the orientation in world space.
        /// </summary>
        public void GetOrientationWorld(out Matrix orientation)
        {
            orientation = this._orientationWorld;
        }

        /// <summary>
        /// Set the orientation in world space.
        /// </summary>
        public void SetOrientationWorld(ref Matrix orientation)
        {
            Matrix worldToLocal; GetParentWorldToLocal(out worldToLocal);
            Matrix orientation_; Matrix.Multiply(ref orientation, ref worldToLocal, out orientation_);
            SetOrientation(ref orientation_);
        }

        /// <summary>
        /// Set the orientation in world space.
        /// </summary>
        public void SetOrientationWorld(ref Vector3 right, ref Vector3 up, ref Vector3 forward)
        {
            Matrix orientation = Matrix.Identity;
            orientation.Right = right;
            orientation.Up = up;
            orientation.Forward = forward;
            SetOrientationWorld(ref orientation);
        }

        /// <summary>
        /// Get the transform with scale applied.
        /// </summary>
        public void GetTransformScaled(out Matrix transform)
        {
            transform = this._transformScaled;
        }

        /// <summary>
        /// Get the transform without scale applied.
        /// </summary>
        public void GetTransformUnscaled(out Matrix transform)
        {
            transform = this._transformUnscaled;
        }

        /// <summary>
        /// Get the transform in world space with scale applied.
        /// </summary>
        public void GetTransformWorldScaled(out Matrix transform)
        {
            transform = this._transformWorldScaled;
        }

        /// <summary>
        /// Get the transform in world space without scale applied.
        /// </summary>
        public void GetTransformWorldUnscaled(out Matrix transform)
        {
            transform = this._transformWorldUnscaled;
        }

        /// <summary>
        /// Get a matrix to convert from local space to world space (disregards scale).
        /// </summary>
        public void GetLocalToWorld(out Matrix transform)
        {
            transform = this._transformWorldUnscaled;
        }

        /// <summary>
        /// Get a matrix to convert from world space to local space (disregards scale).
        /// </summary>
        public void GetWorldToLocal(out Matrix transform)
        {
            Matrix.Invert(ref _transformWorldUnscaled, out transform);
        }

        /// <summary>
        /// Get the world transform of a specified mount point.
        /// </summary>
        public void GetMountPointTransform(int mountPoint, out Matrix transform)
        {
            GetMountPointTransformHook(mountPoint, out transform);
        }

        /// <summary>
        /// Handle looking up the transform of the specified mount point.
        /// </summary>
        protected virtual void GetMountPointTransformHook(int mountPoint, out Matrix transform)
        {
            transform = _transformWorldUnscaled;
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new TransformableComponentToken();
        }

        /// <inheritdoc />
        protected override bool IsValidChildHook(OxComponent child)
        {
            return child is TransformableComponent;
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertyHook(string property)
        {
            base.UpdateWorldPropertyHook(property);
            if (property == "Transform") UpdateTransformWorld();
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertiesHook()
        {
            base.UpdateWorldPropertiesHook();
            UpdateTransformWorld();
        }

        private void boxBuilder_BoundingBoxWorldChanged(IBoundingBoxBuilder sender)
        {
            if (BoundsChanged != null) BoundsChanged(this);
        }

        private void GetParentWorldToLocal(out Matrix transform)
        {
            TransformableComponent parent = GetParent<TransformableComponent>();
            if (parent == null) transform = Matrix.Identity;
            else parent.GetWorldToLocal(out transform);
        }

        private void UpdateTransformWorld()
        {
            TransformableComponent parent = GetParent<TransformableComponent>();
            if (parent == null) UpdateTransformWorldWithoutParent();
            else UpdateTransformWorldWithParent();
        }

        private void UpdateTransformWorldWithoutParent()
        {
            // populate world data by copying local data
            _positionWorld = _position;
            _orientationWorld = _orientation;
            _transformWorldScaled = _transformScaled;
            _transformWorldUnscaled = _transformUnscaled;

            // update bounds
            BoundingBoxBuilder.SetTransformWorld(ref _transformScaled);

            // update transform recursively
            UpdateWorldPropertyOfChildren("Transform");

            RaiseTransformWorldChanged();
        }

        private void UpdateTransformWorldWithParent()
        {
            TransformableComponent parent = GetParent<TransformableComponent>();

            System.Diagnostics.Debug.Assert(parent != null,
                "Control shouldn't reach here if parent is not an TransformableComponent.");

            // get parent transform
            Matrix parentTransform;
            parent.GetMountPointTransform(_mountPoint, out parentTransform);

            // populate world position
            Vector3.Transform(ref _position, ref parentTransform, out _positionWorld);

            // populate world orientation
            Matrix parentOrientation;
            parent.GetOrientationWorld(out parentOrientation);
            Matrix.Multiply(ref _orientation, ref parentOrientation, out _orientationWorld);

            // populate world transform unscaled
            Matrix.Multiply(ref _transformUnscaled, ref parentTransform, out _transformWorldUnscaled);

            // populate world transform scaled
            // TODO: check if this line can leverage calculations in the previous line
            Matrix.Multiply(ref _transformScaled, ref parentTransform, out _transformWorldScaled);

            // update bounding box
            BoundingBoxBuilder.SetTransformWorld(ref _transformWorldScaled);

            // update recursively
            UpdateWorldPropertyOfChildren("Transform");

            RaiseTransformWorldChanged();
        }

        private void UpdateTransform()
        {
            // update transform
            Matrix positionMatrix; Matrix.CreateTranslation(ref _position, out positionMatrix);
            Matrix.Multiply(ref _orientation, ref positionMatrix, out _transformUnscaled);
            Matrix scaleMatrix; Matrix.CreateScale(ref _scale, out scaleMatrix);
            Matrix.Multiply(ref scaleMatrix, ref _transformUnscaled, out _transformScaled);

            // update bounds
            BoundingBoxBuilder.SetTransformLocal(ref _transformScaled);

            // put self in a valid state before firing transform events
            UpdateTransformWorld();

            RaiseTransformChanged();
        }

        private void RaiseTransformChanged()
        {
            if (TransformChanged != null) TransformChanged(this);
        }

        private void RaiseTransformWorldChanged()
        {
            if (TransformWorldChanged != null) TransformWorldChanged(this);
        }

        /// <summary>May be null.</summary>
        private object spatialNodeParent;
        private IBoundingBoxBuilder _boxBuilder;
        private Vector3 _position;
        private Vector3 _positionWorld;
        private Vector3 _scale = Vector3.One;
        private Matrix _orientation = Matrix.Identity;
        private Matrix _orientationWorld = Matrix.Identity;
        private Matrix _transformScaled = Matrix.Identity;
        private Matrix _transformUnscaled = Matrix.Identity;
        private Matrix _transformWorldScaled = Matrix.Identity;
        private Matrix _transformWorldUnscaled = Matrix.Identity;
        private bool _boundless;
        private int _mountPoint = 0;
    }

    /// <summary>
    /// An extension method class for lists that may contain transformable components.
    /// </summary>
    public static class TransformableComponentListExtension
    {
        /// <summary>
        /// Filter out all transformable components from a list that are out of bounds. Modifies
        /// the list in-place.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="bounds">The bounds about which to filter.</param>
        /// <param name="startingIndex">The index in the list at which to begin filtering.</param>
        /// <returns>A reference to the modified list.</returns>
        public static IList<U> FilterBounds<U>(this IList<U> list, BoundingFrustum bounds, int startingIndex) where U : class
        {
            for (int i = startingIndex; i < list.Count; ++i)
            {
                U item = list[i];
                TransformableComponent itemAsT = item as TransformableComponent;
                if (itemAsT != null && !itemAsT.Boundless && bounds.Contains(itemAsT.BoundingBoxWorld) == ContainmentType.Disjoint)
                {
                    list.RemoveAt(i);
                    --i;
                }
            }
            return list;
        }

        /// <summary>
        /// Filter out all transformable components from a list that are out of bounds. Modifies
        /// the list in-place.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="bounds">The bounds about which to filter.</param>
        /// <param name="startingIndex">The index in the list at which to begin filtering.</param>
        /// <returns>A reference to the modified list.</returns>
        public static IList<U> FilterBounds<U>(this IList<U> list, BoundingBox bounds, int startingIndex) where U : class
        {
            for (int i = startingIndex; i < list.Count; ++i)
            {
                U item = list[i];
                TransformableComponent itemAsT = item as TransformableComponent;
                if (itemAsT != null && !itemAsT.Boundless && bounds.Contains(itemAsT.BoundingBoxWorld) == ContainmentType.Disjoint)
                {
                    list.RemoveAt(i);
                    --i;
                }
            }
            return list;
        }
    }
}

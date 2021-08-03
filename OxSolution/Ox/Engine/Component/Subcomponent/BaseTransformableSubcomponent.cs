using Microsoft.Xna.Framework;

namespace Ox.Engine.Component
{
    /// <summary>
    /// A transformable subcomponent that gets its transformation values from its parent component
    /// by default.
    /// </summary>
    public class BaseTransformableSubcomponent : BaseUpdateableSubcomponent
    {
        /// <summary>
        /// Create a BaseTransformableSubcomponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The component to augment.</param>
        public BaseTransformableSubcomponent(OxEngine engine, TransformableComponent component)
            : base(engine, component)
        {
            this.component = component;
        }

        /// <summary>
        /// The bounding box.
        /// </summary>
        public virtual BoundingBox BoundingBox { get { return component.BoundingBox; } }

        /// <summary>
        /// The bounding box in world space.
        /// </summary>
        public virtual BoundingBox BoundingBoxWorld { get { return component.BoundingBoxWorld; } }

        /// <summary>
        /// The bounding box before spatial transformation.
        /// </summary>
        public virtual BoundingBox BoundingBoxSource { get { return component.BoundingBoxSource; } }

        /// <summary>
        /// The position.
        /// </summary>
        public virtual Vector3 Position { get { return component.Position; } }

        /// <summary>
        /// The position in world space.
        /// </summary>
        public virtual Vector3 PositionWorld { get { return component.PositionWorld; } }

        /// <summary>
        /// The scale.
        /// </summary>
        public virtual Vector3 Scale { get { return component.Scale; } }

        /// <summary>
        /// The right vector's orientation.
        /// </summary>
        public virtual Vector3 Right { get { return component.Right; } }

        /// <summary>
        /// The right vector's orientation in world space.
        /// </summary>
        public virtual Vector3 RightWorld { get { return component.RightWorld; } }

        /// <summary>
        /// The up vector's orientation.
        /// </summary>
        public virtual Vector3 Up { get { return component.Up; } }

        /// <summary>
        /// The up vector's orientation in world space.
        /// </summary>
        public virtual Vector3 UpWorld { get { return component.UpWorld; } }

        /// <summary>
        /// The forward vector's orientation.
        /// </summary>
        public virtual Vector3 Forward { get { return component.Forward; } }

        /// <summary>
        /// The forward vector's orientation in world space.
        /// </summary>
        public virtual Vector3 ForwardWorld { get { return component.ForwardWorld; } }

        /// <summary>
        /// The orientation.
        /// </summary>
        public virtual Matrix Orientation { get { return component.Orientation; } }

        /// <summary>
        /// The orientation in world space.
        /// </summary>
        public virtual Matrix OrientationWorld { get { return component.OrientationWorld; } }

        /// <summary>
        /// The transform with scale applied.
        /// </summary>
        public virtual Matrix TransformScaled { get { return component.TransformScaled; } }

        /// <summary>
        /// The transform without scale applied.
        /// </summary>
        public virtual Matrix TransformUnscaled { get { return component.TransformUnscaled; } }

        /// <summary>
        /// The transform in world space with scale applied.
        /// </summary>
        public virtual Matrix TransformWorldScaled { get { return component.TransformWorldScaled; } }

        /// <summary>
        /// The transform in world space without scale applied.
        /// </summary>
        public virtual Matrix TransformWorldUnscaled { get { return component.TransformWorldUnscaled; } }

        /// <summary>
        /// A matrix to convert from local space to world space (disregards scale).
        /// </summary>
        public virtual Matrix LocalToWorld { get { return component.LocalToWorld; } }

        /// <summary>
        /// A matrix to convert from world space to local space (disregards scale).
        /// </summary>
        public virtual Matrix WorldToLocal { get { return component.WorldToLocal; } }

        /// <summary>
        /// Is the object omnipresent?
        /// </summary>
        public virtual bool Boundless { get { return component.Boundless; } }

        /// <summary>
        /// Get the orientation.
        /// </summary>
        public virtual void GetOrientation(out Matrix orientation) { component.GetOrientation(out orientation); }

        /// <summary>
        /// Get the orientation in world space.
        /// </summary>
        public virtual void GetOrientationWorld(out Matrix orientation) { component.GetOrientationWorld(out orientation); }

        /// <summary>
        /// Get the transform with scale applied.
        /// </summary>
        public virtual void GetTransformScaled(out Matrix transform) { component.GetTransformScaled(out transform); }

        /// <summary>
        /// Get the transform without scale applied.
        /// </summary>
        public virtual void GetTransformUnscaled(out Matrix transform) { component.GetTransformUnscaled(out transform); }

        /// <summary>
        /// Get the transform in world space with scale applied (disregards scale).
        /// </summary>
        public virtual void GetTransformWorldScaled(out Matrix transform) { component.GetTransformWorldScaled(out transform); }

        /// <summary>
        /// Get the transform in world space without scale applied (disregards scale).
        /// </summary>
        public virtual void GetTransformWorldUnscaled(out Matrix transform) { component.GetTransformWorldUnscaled(out transform); }

        /// <summary>
        /// Get a matrix to convert from local space to world space.
        /// </summary>
        public virtual void GetLocalToWorld(out Matrix transform) { component.GetLocalToWorld(out transform); }

        /// <summary>
        /// Get a matrix to convert from world space to local space.
        /// </summary>
        public virtual void GetWorldToLocal(out Matrix transform) { component.GetWorldToLocal(out transform); }

        /// <summary>
        /// The augmented component.
        /// </summary>
        protected new TransformableComponent Component { get { return component; } }

        private TransformableComponent component;
    }
}

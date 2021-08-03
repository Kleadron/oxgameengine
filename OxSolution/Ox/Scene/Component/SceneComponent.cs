using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.Primitive;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A component that can be added to a scene.
    /// </summary>
    public class SceneComponent : TransformableComponent
    {
        /// <summary>
        /// Create a SceneComponent that is Boundless.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public SceneComponent(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            Initialize();
        }

        /// <summary>
        /// Create a SceneComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="primitive">The transformable primitive that describes the space occupied by the component.</param>
        public SceneComponent(OxEngine engine, string domainName, bool ownedByDomain, IPrimitive primitive)
            : base(engine, domainName, ownedByDomain, primitive)
        {
            Initialize();
        }

        /// <summary>
        /// Create a SceneComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="boxBuilder">Describes the space occupied by the component.</param>
        public SceneComponent(OxEngine engine, string domainName, bool ownedByDomain, IBoundingBoxBuilder boxBuilder)
            : base(engine, domainName, ownedByDomain, boxBuilder)
        {
            Initialize();
        }

        /// <summary>
        /// The visibility.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return;
                _visible = value;
                UpdateVisibleWorld(); // put self in valid state before firing events
                RaiseVisibleChanged();
            }
        }

        /// <summary>
        /// The cumulative visibility.
        /// </summary>
        public bool VisibleWorld
        {
            get { return _visibleWorld; }
        }

        /// <summary>
        /// Raised when visibility is changed.
        /// </summary>
        public event Action<SceneComponent> VisibleChanged;

        /// <summary>
        /// Raised when cumulative visibility is changed.
        /// </summary>
        public event Action<SceneComponent> VisibleWorldChanged;

        /// <summary>
        /// Draw the component in a scene using the specified drawing mode.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the component is viewed.</param>
        /// <param name="drawMode">The manner in which to draw the component.</param>
        public void Draw(GameTime gameTime, Camera camera, string drawMode)
        {
            CollectSubcomponents(cachedSurfaces);
            {
                Engine.GetService<SurfaceDrawer>().DrawSurfaces(gameTime, camera, drawMode, cachedSurfaces);
            }
            cachedSurfaces.Clear();
        }

        /// <summary>
        /// Try to generate a bounding box from the visible areas.
        /// </summary>
        public bool TryGenerateBoundingBox(out BoundingBox boundingBox)
        {
            return TryGenerateBoundingBoxHook(out boundingBox);
        }

        /// <summary>
        /// Collect all surfaces of type T.
        /// </summary>
        public IList<T> CollectSurfaces<T>(IList<T> result) where T : BaseSurface
        {
            return CollectSurfacesHook(result);
        }

        /// <summary>
        /// Collect all surfaces of type T in the specified bounds.
        /// </summary>
        public IList<T> CollectSurfaces<T>(BoundingFrustum bounds, IList<T> result) where T : BaseSurface
        {
            return CollectSurfacesHook(bounds, result);
        }

        /// <summary>
        /// Collect all surfaces of type T in the specified bounds.
        /// </summary>
        public IList<T> CollectSurfaces<T>(BoundingBox bounds, IList<T> result) where T : BaseSurface
        {
            return CollectSurfacesHook(bounds, result);
        }

        /// <summary>
        /// Handle generating a bounding box if applicable.
        /// </summary>
        /// <param name="boundingBox">The generated bounding box.</param>
        /// <returns>True if a bounding box was generated.</returns>
        protected virtual bool TryGenerateBoundingBoxHook(out BoundingBox boundingBox)
        {
            boundingBox = default(BoundingBox);
            return false;
        }

        /// <summary>
        /// Handle collecting the surfaces of type T.
        /// Override these surface collection methods if you want to cull them in a specialized way.
        /// </summary>
        protected virtual IList<T> CollectSurfacesHook<T>(IList<T> result) where T : BaseSurface
        {
            return CollectSubcomponents(result);
        }

        /// <summary>
        /// Handle collecting the surfaces of type T within a specified bounds.
        /// Override these surface collection methods if you want to cull them in a specialized way.
        /// </summary>
        protected virtual IList<T> CollectSurfacesHook<T>(BoundingFrustum bounds, IList<T> result) where T : BaseSurface
        {
            OxHelper.ArgumentNullCheck(result);

            CollectSubcomponents(cachedTransformableSubcomponents);
            {
                FilterBounds(cachedTransformableSubcomponents, bounds, result);
            }
            cachedTransformableSubcomponents.Clear();

            return result;
        }

        /// <summary>
        /// Handle collecting the surfaces of type T within a specified bounds.
        /// Override these surface collection methods if you want to cull them in a specialized way.
        /// </summary>
        protected virtual IList<T> CollectSurfacesHook<T>(BoundingBox bounds, IList<T> result) where T : BaseSurface
        {
            OxHelper.ArgumentNullCheck(result);

            CollectSubcomponents(cachedTransformableSubcomponents);
            {
                FilterBounds(cachedTransformableSubcomponents, bounds, result);
            }
            cachedTransformableSubcomponents.Clear();

            return result;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) Engine.GetService<SceneSystem>().RemoveComponent(this);
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new SceneComponentToken();
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertyHook(string property)
        {
            base.UpdateWorldPropertyHook(property);
            if (property == "Visible") UpdateVisibleWorld();
        }

        /// <inheritdoc />
        protected override void UpdateWorldPropertiesHook()
        {
            base.UpdateWorldPropertiesHook();
            UpdateVisibleWorld();
        }

        private void Initialize()
        {
            Engine.GetService<SceneSystem>().AddComponent(this);
        }

        private void UpdateVisibleWorld()
        {
            _visibleWorld = CalculateVisibleWorld();
            UpdateWorldPropertyOfChildren("Visible");
            RaiseVisibleWorldChanged();
        }

        private bool CalculateVisibleWorld()
        {
            SceneComponent parent = GetParent<SceneComponent>();
            if (parent == null) return Visible;
            return Visible & parent.VisibleWorld;
        }

        private void RaiseVisibleChanged()
        {
            if (VisibleChanged != null) VisibleChanged(this);
        }

        private void RaiseVisibleWorldChanged()
        {
            if (VisibleWorldChanged != null) VisibleWorldChanged(this);
        }

        private IList<U> FilterBounds<U>(IList<BaseTransformableSubcomponent> source, BoundingFrustum bounds, IList<U> result)
            where U : BaseTransformableSubcomponent
        {
            for (int i = 0; i < source.Count; ++i)
            {
                U item = OxHelper.Cast<U>(source[i]);
                if (item.Boundless || bounds.Contains(item.BoundingBoxWorld) != ContainmentType.Disjoint) result.Add(item);
            }
            return result;
        }

        private IList<U> FilterBounds<U>(IList<BaseTransformableSubcomponent> source, BoundingBox bounds, IList<U> result)
            where U : BaseTransformableSubcomponent
        {
            for (int i = 0; i < source.Count; ++i)
            {
                U item = OxHelper.Cast<U>(source[i]);
                if (item.Boundless || bounds.Contains(item.BoundingBoxWorld) != ContainmentType.Disjoint) result.Add(item);
            }
            return result;
        }

        private readonly IList<BaseTransformableSubcomponent> cachedTransformableSubcomponents = new List<BaseTransformableSubcomponent>();
        private readonly IList<BaseSurface> cachedSurfaces = new List<BaseSurface>();
        private bool _visible = true;
        private bool _visibleWorld = true;
    }
}

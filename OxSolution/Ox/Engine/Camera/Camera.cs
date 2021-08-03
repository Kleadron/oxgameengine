using Microsoft.Xna.Framework;
using Ox.Engine.MathNamespace;
using Ox.Engine.ServicesNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine.CameraNamespace
{
    /// <summary>
    /// A camera in 3D space. Defers projection calculation to subclass.
    /// </summary>
    public abstract class Camera : Disposable
    {
        /// <summary>
        /// Create a Camera.
        /// </summary>
        /// <param name="engine">The engine</param>
        public Camera(OxEngine engine)
        {
            OxHelper.ArgumentNullCheck(engine);
            this.engine = engine;
            UpdateProjection();
            engine.ResolutionManager.ResolutionChanged += resolutionManager_ResolutionChanged;
        }

        /// <summary>
        /// The bounding box of the viewing area.
        /// </summary>
        public BoundingBox BoundingBox { get { return _boundingBox; } }

        /// <summary>
        /// The camera's position.
        /// </summary>
        public Vector3 Position { get { return _position; } }
        
        /// <summary>
        /// The camera's look up vector.
        /// </summary>
        public Vector3 LookUp { get { return _lookUp; } }

        /// <summary>
        /// The camera's look forward.
        /// </summary>
        public Vector3 LookForward { get { return _lookForward; } }

        /// <summary>
        /// The camera's look target.
        /// </summary>
        public Vector3 LookTarget { get { return _lookTarget; } }
        
        /// <summary>
        /// The view matrix.
        /// </summary>
        public Matrix View { get { return _view; } }

        /// <summary>
        /// The projection matrix.
        /// </summary>
        public Matrix Projection { get { return _projection; } }

        /// <summary>
        /// The view * projection matrix.
        /// </summary>
        public Matrix ViewProjection
        {
            get
            {
                Matrix result;
                GetViewProjection(out result);
                return result;
            }
        }

        /// <summary>
        /// The near plane of the bounding frustum.
        /// </summary>
        public float NearPlane
        {
            get { return _nearPlane; }
            set
            {
                if (_nearPlane == value) return; // OPTIMIZATION
                _nearPlane = value;
                UpdateProjection();
            }
        }

        /// <summary>
        /// The far plane of the bounding frustum.
        /// </summary>
        public float FarPlane
        {
            get { return _farPlane; }
            set
            {
                if (_farPlane == value) return; // OPTIMIZATION
                _farPlane = value;
                UpdateProjection();
            }
        }

        /// <summary>
        /// Get the view matrix.
        /// </summary>
        public void GetView(out Matrix view)
        {
            view = _view;
        }

        /// <summary>
        /// Set the camera transform with a look forward algorithm.
        /// </summary>
        public void SetTransformByLookForward(Vector3 position, Vector3 lookUp, Vector3 lookForward)
        {
            _position = position;
            _lookUp = lookUp;
            _lookForward = lookForward;
            _lookTarget = position + lookForward;
            Vector3 lookTarget = position + lookForward;
            Matrix.CreateLookAt(ref position, ref lookTarget, ref lookUp, out _view);
            UpdateBounds();
        }
        
        /// <summary>
        /// Set the camera transform with a look target algorithm.
        /// </summary>
        public void SetTransformByLookTarget(Vector3 position, Vector3 lookUp, Vector3 lookTarget)
        {
            _position = position;
            _lookUp = lookUp;
            _lookForward = Vector3.Normalize(lookTarget - position);
            _lookTarget = lookTarget;
            Matrix.CreateLookAt(ref position, ref lookTarget, ref lookUp, out _view);
            UpdateBounds();
        }

        /// <summary>
        /// Get the projection matrix.
        /// </summary>
        public void GetProjection(out Matrix projection)
        {
            projection = this._projection;
        }

        /// <summary>
        /// Get the view * projection matrix.
        /// </summary>
        public void GetViewProjection(out Matrix viewProjection)
        {
            Matrix.Multiply(ref _view, ref _projection, out viewProjection);
        }

        /// <summary>
        /// Does this view frustom contain a boundingBox?
        /// </summary>
        public ContainmentType Contains(BoundingBox boundingBox)
        {
            // OPTIMIZATION: check for intersection with the bounding box before checking for
            // intersection with the bounding frustum itself.
            return
                _boundingBox.Contains(boundingBox) != ContainmentType.Disjoint ?
                _boundingFrustum.Contains(boundingBox) :
                ContainmentType.Disjoint;
        }
        /// <summary>
        /// Find a point on a clip plane in world space.
        /// </summary>
        public Vector3 ClipPlaneToWorld(Vector3 point)
        {
            Matrix viewProjection; GetViewProjection(out viewProjection);
            Matrix inverseViewProjection; Matrix.Invert(ref viewProjection, out inverseViewProjection);
            Vector4 positionHom = new Vector4(point, 1.0f);
            Vector4 positionWorld; Vector4.Transform(ref positionHom, ref inverseViewProjection, out positionWorld);
            positionWorld /= positionWorld.W;
            return new Vector3(positionWorld.X, positionWorld.Y, positionWorld.Z);
        }

        /// <summary>
        /// The engine.
        /// </summary>
        protected OxEngine Engine { get { return engine; } }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) engine.ResolutionManager.ResolutionChanged -= resolutionManager_ResolutionChanged;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Update the projection matrix.
        /// </summary>
        protected void UpdateProjection()
        {
            _projection = CalculateProjectionHook();
            UpdateBounds();
        }

        /// <summary>
        /// Handle calculating the project matrix.
        /// </summary>
        /// <returns></returns>
        protected abstract Matrix CalculateProjectionHook();

        private void resolutionManager_ResolutionChanged(ResolutionManager sender, Point resolution)
        {
            OxHelper.ArgumentNullCheck(sender);
            UpdateProjection();
        }

        private void UpdateBounds()
        {
            Matrix viewProjection;
            GetViewProjection(out viewProjection);
            _boundingFrustum.Matrix = viewProjection;
            _boundingFrustum.GetCorners(cachedFrustumCorners);
            _boundingBox = cachedFrustumCorners.GenerateBoundingBox();
        }

        private readonly Vector3[] cachedFrustumCorners = new Vector3[8];
        private readonly OxEngine engine;
        private readonly BoundingFrustum _boundingFrustum = new BoundingFrustum(Matrix.Identity);
        private BoundingBox _boundingBox;
        private Vector3 _position;
        private Vector3 _lookUp;
        private Vector3 _lookForward;
        private Vector3 _lookTarget;
        private Matrix _view = Matrix.Identity;
        private Matrix _projection = Matrix.Identity;
        private float _nearPlane = 1;
        private float _farPlane = 1024;
    }
}

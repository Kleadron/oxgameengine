using Microsoft.Xna.Framework;

namespace Ox.Engine.CameraNamespace
{
    /// <summary>
    /// A camera that uses field-of-view projection.
    /// </summary>
    public class FovCamera : Camera
    {
        /// <summary>
        /// Create a FovCamera.
        /// </summary>
        public FovCamera(OxEngine engine) : base(engine)
        {
            NearPlane = 1;
            FarPlane = 1024;
        }

        /// <summary>
        /// The camera's field of view.
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set
            {
                if (_fieldOfView == value) return; // OPTIMIZATION
                _fieldOfView = value;
                UpdateProjection();
            }
        }

        /// <inheritdoc />
        protected override Matrix CalculateProjectionHook()
        {
            float aspectRatio = Engine.GraphicsDevice.Viewport.AspectRatio;
            return Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearPlane, FarPlane);
        }

        private float _fieldOfView = MathHelper.PiOver4;
    }
}

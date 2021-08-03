using Microsoft.Xna.Framework;

namespace Ox.Engine.CameraNamespace
{
    /// <summary>
    /// A camera that uses orthographic projection.
    /// </summary>
    public class OrthoCamera : Camera
    {
        public OrthoCamera(OxEngine engine)
            : base(engine)
        {
            NearPlane = 0;
            FarPlane = 1024;
        }

        /// <summary>
        /// The width of the camera's viewing area.
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                if (_width == value) return; // OPTIMIZATION
                _width = value;
                UpdateProjection();
            }
        }

        /// <summary>
        /// The height of the camera's viewing area.
        /// </summary>
        public float Height
        {
            get { return _height; }
            set
            {
                if (_height == value) return; // OPTIMIZATION
                _height = value;
                UpdateProjection();
            }
        }

        /// <inheritdoc />
        protected override Matrix CalculateProjectionHook()
        {
            return Matrix.CreateOrthographic(Width, Height, NearPlane, FarPlane);
        }

        private float _width = 160;
        private float _height = 90;
    }
}

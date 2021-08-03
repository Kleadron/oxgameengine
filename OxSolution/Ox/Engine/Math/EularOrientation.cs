using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// The axis of an angle.
    /// </summary>
    public enum Axis { X = 0, Y, Z }

    /// <summary>
    /// Raised when the orientation of an IEularOrientation has changed.
    /// </summary>
    public delegate void OrientationChanged(EularOrientation sender, Matrix orientation);

    /// <summary>
    /// A configurable eular orientation.
    /// </summary>
    public class EularOrientation
    {
        /// <summary>
        /// Create an EularOrientation.
        /// </summary>
        public EularOrientation()
        {
            UpdateOrientation();
        }

        /// <summary>
        /// The calculated orientation.
        /// </summary>
        public Matrix Orientation { get { return _orientation; } }

        /// <summary>
        /// The orientation right vector.
        /// </summary>
        public Vector3 Right { get { return _orientation.Right; } }

        /// <summary>
        /// The orientation up vector.
        /// </summary>
        public Vector3 Up { get { return _orientation.Up; } }

        /// <summary>
        /// The orientation forward vector.
        /// </summary>
        public Vector3 Forward { get { return _orientation.Forward; } }

        /// <summary>
        /// The axis of the first rotation applied.
        /// </summary>
        public Axis Axis1
        {
            get { return _axis1; }
            set
            {
                _axis1 = value;
                UpdateOrientation();
            }
        }

        /// <summary>
        /// The axis of the second rotation applied.
        /// </summary>
        public Axis Axis2
        {
            get { return _axis2; }
            set
            {
                _axis2 = value;
                UpdateOrientation();
            }
        }

        /// <summary>
        /// The axis of the third rotation applied.
        /// </summary>
        public Axis Axis3
        {
            get { return _axis3; }
            set
            {
                _axis3 = value;
                UpdateOrientation();
            }
        }

        /// <summary>
        /// The angle of the first rotation.
        /// </summary>
        public float Angle1
        {
            get { return _angle1; }
            set
            {
                if (_angle1 == value) return;
                _angle1 = value;
                UpdateOrientation();
            }
        }

        /// <summary>
        /// The angle of the second rotation.
        /// </summary>
        public float Angle2
        {
            get { return _angle2; }
            set
            {
                if (_angle2 == value) return;
                _angle2 = value;
                UpdateOrientation();
            }
        }

        /// <summary>
        /// The angle of the third rotation.
        /// </summary>
        public float Angle3
        {
            get { return _angle3; }
            set
            {
                if (_angle3 == value) return;
                _angle3 = value;
                UpdateOrientation();
            }
        }

        /// <summary>
        /// Raised when the orientation is changed.
        /// </summary>
        public event OrientationChanged OrientationChanged;

        /// <summary>
        /// Get the orientation in matrix form.
        /// </summary>
        public void GetOrientation(out Matrix orientation)
        {
            orientation = _orientation;
        }

        /// <summary>
        /// Clear the axis specifications to X, Y, and Z.
        /// </summary>
        public void ClearAxes()
        {
            Axis1 = Axis.X;
            Axis2 = Axis.Y;
            Axis3 = Axis.Z;
        }

        /// <summary>
        /// Clear the angles.
        /// </summary>
        public void ClearAngles()
        {
            Angle1 = Angle2 = Angle3 = 0;
        }

        /// <summary>
        /// Clear the angles and axis specifications.
        /// </summary>
        public void Clear()
        {
            ClearAxes();
            ClearAngles();
        }

        private void UpdateOrientation()
        {
            Matrix rotation1; CreateRotation(Angle1, Axis1, out rotation1);
            Matrix rotation2; CreateRotation(Angle2, Axis2, out rotation2);
            Matrix rotation3; CreateRotation(Angle3, Axis3, out rotation3);
            _orientation = rotation1 * rotation2 * rotation3;
            if (OrientationChanged != null) OrientationChanged(this, _orientation);
        }

        private static void CreateRotation(float angle, Axis axis, out Matrix rotation)
        {
            switch (axis)
            {
                case Axis.X: Matrix.CreateRotationX(angle, out rotation); break;
                case Axis.Y: Matrix.CreateRotationY(angle, out rotation); break;
                default: Matrix.CreateRotationZ(angle, out rotation); break;
            }
        }

        private Matrix _orientation = Matrix.Identity;
        private Axis _axis1 = Axis.X;
        private Axis _axis2 = Axis.Y;
        private Axis _axis3 = Axis.Z;
        private float _angle1;
        private float _angle2;
        private float _angle3;
    }
}

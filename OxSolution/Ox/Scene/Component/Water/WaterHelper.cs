using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.MathNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Helper methods for water.
    /// </summary>
    public static class WaterHelper
    {
        /// <summary>
        /// Create a clip plane for drawing the water reflection from the reflection camera's view.
        /// </summary>
        public static Plane CreateWaterReflectionPlane(Camera reflectionCamera, float waterHeight)
        {
            OxHelper.ArgumentNullCheck(reflectionCamera);
            Vector4 planeCoefficients = new Vector4(Vector3.Up, -waterHeight);
            Matrix inverseCamera = Matrix.Transpose(Matrix.Invert(reflectionCamera.ViewProjection));
            Vector4 transformedPlaneCoefficients = Vector4.Transform(planeCoefficients, inverseCamera);
            return new Plane(transformedPlaneCoefficients);
        }
    }
}

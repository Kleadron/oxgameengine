using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.CameraNamespace;

namespace Ox.Engine.EffectNamespace
{
    /// <summary>
    /// An extension method class for BasicEffect.
    /// </summary>
    public static class BasicEffectExtension
    {
        /// <summary>
        /// Populate the transform parameters of a BasicEffect.
        /// </summary>
        /// <param name="camera">The camera from which to extract the view and projection.</param>
        /// <param name="world">The world transform.</param>
        /// <param name="effect">The effect to populate.</param>
        public static void PopulateTransform(this BasicEffect effect, Camera camera, ref Matrix world)
        {
            OxHelper.ArgumentNullCheck(camera);
            Matrix view;
            Matrix projection;
            camera.GetView(out view);
            camera.GetProjection(out projection);
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
        }
    }
}

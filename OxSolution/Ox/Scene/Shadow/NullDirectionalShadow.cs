using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Utility;
using Ox.Engine;

namespace Ox.Scene.Shadow
{
    /// <summary>
    /// A null implementation of IDirectionalShadow.
    /// </summary>
    public class NullDirectionalShadow : Disposable, IDirectionalShadow
    {
        /// <summary>
        /// Create a NullDirectionalShadow.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public NullDirectionalShadow(OxEngine engine)
        {
            camera = new OrthoCamera(engine);
        }

        /// <inheritdoc />
        public Texture2D VolatileShadowMap { get { return null; } }

        /// <inheritdoc />
        public Camera Camera { get { return camera; } }

        /// <inheritdoc />
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <inheritdoc />
        public void Draw(GameTime gameTime) { }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) camera.Dispose();
            base.Dispose(disposing);
        }

        private readonly Camera camera;
        private bool enabled;
    }
}

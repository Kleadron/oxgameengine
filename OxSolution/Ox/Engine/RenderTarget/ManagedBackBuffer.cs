using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.Utility;

namespace Ox.Engine.RenderTarget
{
    /// <summary>
    /// An implementation of IManagedRenderTarget that draws to the back buffer.
    /// </summary>
    public class ManagedBackBuffer : Disposable, IManagedRenderTarget
    {
        /// <summary>
        /// Create a ManagedBackBuffer.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="renderTargetIndex">See property RenderTargetIndex.</param>
        public ManagedBackBuffer(OxEngine engine, int renderTargetIndex)
        {
            OxHelper.ArgumentNullCheck(engine);
            this.engine = engine;
            this.renderTargetIndex = renderTargetIndex;
        }

        /// <inheritdoc />
        public Texture2D VolatileTexture { get { return null; } }

        /// <inheritdoc />
        public float SurfaceToScreenRatio
        {
            get { return 1; }
            set { }
        }

        /// <inheritdoc />
        public Point Resolution
        {
            get
            {
                PresentationParameters presentation = engine.GraphicsDevice.PresentationParameters;
                return new Point(presentation.BackBufferWidth, presentation.BackBufferHeight);
            }
        }

        /// <inheritdoc />
        public int RenderTargetIndex { get { return renderTargetIndex; } }

        /// <inheritdoc />
        public void Activate()
        {
            engine.GraphicsDevice.SetRenderTarget(renderTargetIndex, null);
        }

        /// <inheritdoc />
        public void Resolve() { }

        private readonly OxEngine engine;
        private readonly int renderTargetIndex;
    }
}

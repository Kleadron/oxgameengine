using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.Utility;

namespace Ox.Engine.RenderTarget
{
    /// <summary>
    /// A managed render target with a 2D drawing surface.
    /// </summary>
    public class ManagedRenderTarget2D : Disposable
    {
        /// <summary>
        /// Create a ManagedRenderTarget2D.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="surfaceToScreenRatio">See property SurfaceToScreenRatio.</param>
        /// <param name="levelCount">The number of mip-map levels of the texture produced by the render target.</param>
        /// <param name="format">The format of the render target.</param>
        /// <param name="msType">The multi-sampling type of the texture produced by the render target.</param>
        /// <param name="msQuality">The multi-sampling quality of the texture produced by the render target.</param>
        /// <param name="renderTargetIndex">See property RenderTargetIndex.</param>
        public ManagedRenderTarget2D(OxEngine engine, float surfaceToScreenRatio, int levelCount,
            SurfaceFormat format, MultiSampleType msType, int msQuality, int renderTargetIndex)
        {
            OxHelper.ArgumentNullCheck(engine);
            this.engine = engine;
            this.levelCount = levelCount;
            this.format = format;
            this.msType = msType;
            this.msQuality = msQuality;
            this.renderTargetIndex = renderTargetIndex;
            engine.GraphicsDevice.DeviceReset += new EventHandler(device_DeviceReset);
            SurfaceToScreenRatio = surfaceToScreenRatio;
        }

        /// <summary>
        /// The drawing surface as a 2D texture.
        /// May be null.
        /// </summary>
        public Texture2D VolatileTexture { get { return texture; } }

        /// <summary>
        /// The resolution of the drawing surface.
        /// </summary>
        public Point Resolution { get { return resolution; } }

        /// <summary>
        /// The drawing surface size as a ratio of the screen size.
        /// </summary>
        public float SurfaceToScreenRatio
        {
            get { return _surfaceToScreenRatio; }
            set
            {
                if (_surfaceToScreenRatio == value && renderTarget != null) return; // OPTIMIZATION
                _surfaceToScreenRatio = value;
                RecreateRenderTarget();
            }
        }

        /// <summary>
        /// The render target's index on the GraphicsDevice.
        /// </summary>
        public int RenderTargetIndex { get { return renderTargetIndex; } }

        /// <inheritdoc />
        public void Activate()
        {
            engine.GraphicsDevice.SetRenderTarget(renderTargetIndex, renderTarget);
        }

        /// <inheritdoc />
        public void Resolve()
        {
            if (!IsRenderTargetSameAsDeviceRenderTarget) System.Diagnostics.Trace.Fail(
                "Tried to resolve a render target that was not activated.");
            else
            {
                if (resolved) return; // OPTIMIZATION
                engine.GraphicsDevice.SetRenderTarget(0, null);
                texture = renderTarget.GetTexture();
                resolved = true;
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                engine.GraphicsDevice.DeviceReset -= device_DeviceReset;
                renderTarget.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IsRenderTargetSameAsDeviceRenderTarget
        {
            get { return engine.GraphicsDevice.GetRenderTarget(renderTargetIndex) == renderTarget; }
        }

        private void device_DeviceReset(object sender, EventArgs e)
        {
            OxHelper.ArgumentNullCheck(sender, e);
            RecreateRenderTarget();
        }

        private void RecreateRenderTarget()
        {
            if (renderTarget != null) renderTarget.Dispose();
            resolution = CalculateResolution(engine.GraphicsDevice, SurfaceToScreenRatio);
            resolved = false;
            texture = null;
            renderTarget = new RenderTarget2D(
                engine.GraphicsDevice, resolution.X, resolution.Y, levelCount, format, msType, msQuality);
        }

        private static Point CalculateResolution(GraphicsDevice graphicsDevice, float surfaceToScreenRatio)
        {
            PresentationParameters presentation = graphicsDevice.PresentationParameters;
            return new Point(
                (int)(presentation.BackBufferWidth * surfaceToScreenRatio),
                (int)(presentation.BackBufferHeight * surfaceToScreenRatio));
        }

        private readonly MultiSampleType msType;
        private readonly SurfaceFormat format;
        private readonly OxEngine engine;
        private readonly int renderTargetIndex;
        private readonly int levelCount;
        private readonly int msQuality;
        private RenderTarget2D renderTarget;
        /// <summary>May be null.</summary>
        private Texture2D texture;
        private Point resolution;
        private bool resolved;
        private float _surfaceToScreenRatio;
    }
}

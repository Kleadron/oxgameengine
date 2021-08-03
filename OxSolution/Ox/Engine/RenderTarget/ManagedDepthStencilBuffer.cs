using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.Utility;

namespace Ox.Engine.RenderTarget
{
    /// <summary>
    /// A managed depth / stencil buffer.
    /// </summary>
    public class ManagedDepthStencilBuffer : Disposable
    {
        /// <summary>
        /// Create a ManagedDepthStencilBuffer.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="surfaceToScreenRatio">See property SurfaceToScreenRatio.</param>
        /// <param name="format">The format of the stencil buffer.</param>
        /// <param name="msType">The multi-sampling type of the texture produced by the render target.</param>
        /// <param name="msQuality">The multi-sampling quality of the texture produced by the render target.</param>
        public ManagedDepthStencilBuffer(OxEngine engine, float surfaceToScreenRatio,
            DepthFormat format, MultiSampleType msType, int msQuality)
        {
            OxHelper.ArgumentNullCheck(engine);
            this.engine = engine;
            this.format = format;
            this.msType = msType;
            this.msQuality = msQuality;
            engine.GraphicsDevice.DeviceReset += new EventHandler(device_DeviceReset);
            SurfaceToScreenRatio = surfaceToScreenRatio;
        }
        
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
                if (_surfaceToScreenRatio == value && depthStencilBuffer != null) return; // OPTIMIZATION
                _surfaceToScreenRatio = value;
                RecreateDepthStencilBuffer();
            }
        }

        /// <summary>
        /// Activate the depth stencil buffer.
        /// </summary>
        public void Activate()
        {
            engine.GraphicsDevice.DepthStencilBuffer = depthStencilBuffer;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                engine.GraphicsDevice.DeviceReset -= device_DeviceReset;
                depthStencilBuffer.Dispose();
            }
            base.Dispose(disposing);
        }

        private void device_DeviceReset(object sender, EventArgs e)
        {
            OxHelper.ArgumentNullCheck(sender, e);
            RecreateDepthStencilBuffer();
        }

        private void RecreateDepthStencilBuffer()
        {
            if (depthStencilBuffer != null) depthStencilBuffer.Dispose();
            resolution = CalculateResolution(engine.GraphicsDevice, SurfaceToScreenRatio);
            depthStencilBuffer = new DepthStencilBuffer(engine.GraphicsDevice, resolution.X, resolution.Y, format, msType, msQuality);
        }

        private static Point CalculateResolution(GraphicsDevice graphicsDevice, float surfaceToScreenRatio)
        {
            PresentationParameters presentation = graphicsDevice.PresentationParameters;
            return new Point(
                (int)(presentation.BackBufferWidth * surfaceToScreenRatio),
                (int)(presentation.BackBufferHeight * surfaceToScreenRatio));
        }

        private readonly MultiSampleType msType;
        private readonly DepthFormat format;
        private readonly OxEngine engine;
        private readonly int msQuality;
        private DepthStencilBuffer depthStencilBuffer;
        private Point resolution;
        private float _surfaceToScreenRatio;
    }
}

using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// Raised when the game's resolution is changed.
    /// </summary>
    public delegate void ResolutionChanged(ResolutionManager sender, Point resolution);

    /// <summary>
    /// Invokes resolution changes and observes resolution changes by machinery other than its own.
    /// </summary>
    public class ResolutionManager : OxUpdateable
    {
        /// <summary>
        /// Create a ResolutionManager.
        /// </summary>
        /// <param name="deviceManager">The graphics device manager.</param>
        public ResolutionManager(GraphicsDeviceManager deviceManager)
        {
            OxHelper.ArgumentNullCheck(deviceManager);
            this.deviceManager = deviceManager;
            resolution = new Point(
                deviceManager.PreferredBackBufferWidth,
                deviceManager.PreferredBackBufferHeight);
        }

        /// <summary>
        /// The game's resolution.
        /// </summary>
        public Point Resolution
        {
            get { return resolution; }
            set
            {
                value.X = (int)MathHelper.Max(OxConfiguration.MinimumResolution.X, value.X); // VALIDATION
                value.Y = (int)MathHelper.Max(OxConfiguration.MinimumResolution.Y, value.Y); // VALIDATION
                if (resolution == value) return;
                deviceManager.PreferredBackBufferWidth = value.X;
                deviceManager.PreferredBackBufferHeight = value.Y;
                deviceManager.ApplyChanges();
                UpdateResolution();
            }
        }

        /// <summary>
        /// Raised when the game's resolution is changed.
        /// </summary>
        public event ResolutionChanged ResolutionChanged;

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            Resolution = new Point(
                deviceManager.PreferredBackBufferWidth,
                deviceManager.PreferredBackBufferHeight);
        }

        private void UpdateResolution()
        {
            if (resolution.X == deviceManager.PreferredBackBufferWidth &&
                resolution.Y == deviceManager.PreferredBackBufferHeight)
                return;
            resolution = new Point(
                deviceManager.PreferredBackBufferWidth,
                deviceManager.PreferredBackBufferHeight);
            if (ResolutionChanged != null) ResolutionChanged(this, resolution);
        }

        private readonly GraphicsDeviceManager deviceManager;
        private Point resolution;
    }
}

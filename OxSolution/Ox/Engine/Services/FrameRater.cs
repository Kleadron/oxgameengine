using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// Tracks the frame rate of a game.
    /// </summary>
    public class FrameRater
    {
        /// <summary>
        /// The current frame rate of the game.
        /// </summary>
        public int FrameRate { get { return frameRate; } }

        /// <summary>
        /// Register the occurance of one frame draw.
        /// </summary>
        public void RegisterFrame(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            frameCountElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameCountElapsedTime > 1) ResetFrameCount();            
            ++frameCount;
        }

        private void ResetFrameCount()
        {
            frameRate = frameCount;
            frameCount = 0;
            frameCountElapsedTime = 0;
        }

        private float frameCountElapsedTime;
        private int frameCount;
        private int frameRate;
    }
}

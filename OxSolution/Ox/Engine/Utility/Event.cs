using Microsoft.Xna.Framework;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Raised when something time-based happens in the game.
    /// </summary>
    /// <typeparam name="T">The type of the object that emits the event.</typeparam>
    /// <param name="sender">The object that emits the event.</param>
    /// <param name="gameTime">The current game time.</param>
    public delegate void TimeAction<T>(T sender, GameTime gameTime);
}

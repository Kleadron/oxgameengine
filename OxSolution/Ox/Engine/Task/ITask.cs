using Microsoft.Xna.Framework;

namespace Ox.Engine.TaskNamespace
{
    /// <summary>
    /// Represents a task to be accomplished.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Process the task. Returns true if task is expired.
        /// </summary>
        bool Process(GameTime gameTime);
    }
}

using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.TaskNamespace
{
    /// <summary>
    /// A one-shot task that happens after a delay.
    /// </summary>
    public class SingleTask : ITask
    {
        /// <summary>
        /// Create a SingleTask.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="delay">The amount of seconds to wait before running the task.</param>
        public SingleTask(Action action, double delay)
        {
            OxHelper.ArgumentNullCheck(action);
            if (delay < 0) throw new ArgumentException("Parameter delay cannot be less than 0.");
            this.action = action;
            this.delay = delay;
        }

        /// <inheritdoc />
        public bool Process(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime < delay) return false;
            action();
            return true;
        }

        private Action action;
        private double elapsedTime;
        private double delay;
    }
}

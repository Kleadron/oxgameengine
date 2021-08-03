using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.TaskNamespace
{
    /// <summary>
    /// An action that is performed over a range of time.
    /// </summary>
    public delegate void RangeAction(double elapsedTime, double percentElapsed);

    /// <summary>
    /// A task the triggers every game cycle over a range of time.
    /// </summary>
    public class RangeTask : ITask
    {
        /// <summary>
        /// Create a RangeTask.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="startTime">The time to start running the task.</param>
        /// <param name="endTime">The time to stop running the task.</param>
        public RangeTask(RangeAction action, double startTime, double endTime)
        {
            OxHelper.ArgumentNullCheck(action);
            if (startTime < 0) throw new ArgumentException("Parameter startTime cannot be less than 0.");
            if (endTime <= 0) throw new ArgumentException("Parameter endTime cannot be less than or equal to 0.");
            if (endTime <= startTime) throw new ArgumentException("Parameter endTime cannot be less than startTime.");
            this.action = action;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        /// <inheritdoc />
        public bool Process(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime > endTime) return true;
            if (elapsedTime >= startTime) action(ElapsedTimeAfterStart, PercentTimeAfterStart);
            return false;
        }

        private double PercentTimeAfterStart { get { return ElapsedTimeAfterStart / TotalTime; } }

        private double ElapsedTimeAfterStart { get { return elapsedTime - startTime; } }

        private double TotalTime { get { return endTime - startTime; } }

        private RangeAction action;
        private double elapsedTime;
        private double startTime;
        private double endTime;
    }
}

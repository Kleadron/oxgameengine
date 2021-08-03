using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Ox.Engine.TaskNamespace
{
    /// <summary>
    /// Processing tasks.
    /// </summary>
    public class TaskProcessor
    {
        /// <summary>
        /// Update the task processor, processing all tasks.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);

            updating = true;
            {
                for (int i = 0; i < tasks.Count; ++i)
                {
                    ITask task = tasks[i];
                    if (task.Process(gameTime))
                    {
                        tasks.Remove(task);
                        --i;
                    }
                }
            }
            updating = false;
        }

        /// <summary>
        /// Add a task.
        /// </summary>
        /// <param name="task">The task to process.</param>
        public void AddTask(ITask task)
        {
            if (updating) throw new InvalidOperationException("Cannot add a task while a task is executing.");
            tasks.Add(task);
        }

        /// <summary>
        /// Remove a task.
        /// </summary>
        /// <param name="task">The task to remove.</param>
        public bool RemoveTask(ITask task)
        {
            if (updating) throw new InvalidOperationException("Cannot remove a task while a task is executing.");
            return tasks.Remove(task);
        }

        /// <summary>
        /// Remove all the tasks.
        /// </summary>
        public void ClearTasks()
        {
            tasks.Clear();
        }

        private readonly IList<ITask> tasks = new List<ITask>();
        private bool updating;
    }
}

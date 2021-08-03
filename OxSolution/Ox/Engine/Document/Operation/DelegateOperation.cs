using System;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A set of redoable / undoable delegate operations.
    /// </summary>
    public class DelegateOperation : IOperation
    {
        /// <summary>
        /// Create a DelegateOperation.
        /// </summary>
        public DelegateOperation(string name, Guid group, Action undo, Action redo)
        {
            OxHelper.ArgumentNullCheck(name, undo, redo);
            this.name = name;
            this.undo = undo;
            this.redo = redo;
            this.group = group;
        }

        /// <inheritdoc />
        public Guid Handle { get { return handle; } }

        /// <inheritdoc />
        public Guid Group { get { return group; } }

        /// <inheritdoc />
        public string Name { get { return name; } }

        /// <inheritdoc />
        public bool Done { get { return done; } }

        /// <inheritdoc />
        public void Redo()
        {
            if (done) return;
            done = true;
            redo();
        }

        /// <inheritdoc />
        public void Undo()
        {
            if (!done) return;
            done = false;
            undo();
        }

        private readonly Action redo;
        private readonly Action undo;
        private readonly Guid handle = Guid.NewGuid();
        private readonly Guid group;
        private readonly string name;
        private bool done = true;
    }
}

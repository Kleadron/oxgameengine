using System;
using System.Collections.Generic;
using System.Linq;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A recorder for doable / undoable operations.
    /// </summary>
    public class OperationRecorder
    {
        /// <summary>
        /// Is there an undo / redo operation in progress?
        /// </summary>
        public bool Winding { get { return winding; } }

        /// <summary>
        /// Are operations being ignored?
        /// </summary>
        public bool Paused { get { return pauseDepth != 0; } }

        /// <summary>
        /// Are operations being grouped?
        /// </summary>
        public bool Grouped { get { return groupDepth != 0; } }

        /// <summary>
        /// Push a pause state.
        /// </summary>
        public void PushPause()
        {
            ++pauseDepth;
        }

        /// <summary>
        /// Pop a pause state.
        /// </summary>
        public void PopPause()
        {
            if (pauseDepth == 0) System.Diagnostics.Trace.Fail("Cannot pop more pauses than have been pushed.");
            else --pauseDepth;
        }

        /// <summary>
        /// Push a grouping state.
        /// </summary>
        public void PushGroup()
        {
            if (Paused) throw new InvalidOperationException("Cannot PushGroup while Paused!");
            if (groupDepth == 0) _currentGroup = Guid.NewGuid();
            ++groupDepth;
        }

        /// <summary>
        /// Pop a grouping state.
        /// </summary>
        public void PopGroup()
        {
            if (Paused) throw new InvalidOperationException("Cannot PopGroup while Paused!");
            if (groupDepth == 0) System.Diagnostics.Trace.Fail("Cannot pop more groups than have been pushed.");
            else --groupDepth;
        }

        /// <summary>
        /// Record an operation.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="undo">The undo action.</param>
        /// <param name="redo">The redo action.</param>
        public void Record(string name, Action undo, Action redo)
        {
            if (RecordingBlocked) return;
            IOperation operation = new DelegateOperation(name, CalculatedGroup, undo, redo);
            Record(operation);
        }

        /// <summary>
        /// Record an operation.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="target">The target of the operation.</param>
        /// <param name="propertyName">The affected property name.</param>
        /// <param name="oldValue">The previous value of the affected property.</param>
        public void Record(string name, object target, string propertyName, object oldValue)
        {
            if (RecordingBlocked) return;
            IOperation operation = new SetOperation(name, CalculatedGroup, target, propertyName, oldValue);
            Record(operation);
        }

        /// <summary>
        /// Undo the previous operation.
        /// </summary>
        public void Undo()
        {
            if (WindingBlocked) return;

            winding = true;
            {
                var groups = Split(undoables);
                var firstGroup = groups.FirstOrDefault();
                if (firstGroup != null)
                {
                    foreach (var operation in firstGroup)
                    {
                        undoables.Pop();
                        operation.Undo();
                        redoables.Push(operation);
                    }
                }
            }
            winding = false;
        }

        /// <summary>
        /// Undo across the specified operation.
        /// </summary>
        /// <param name="handle">The handle of the operation.</param>
        public void Undo(Guid handle)
        {
            if (WindingBlocked) return;
            IOperation undoable = GetOperation(undoables, handle);
            while (undoables.Peek() != undoable) Undo();
            Undo();
        }

        /// <summary>
        /// Redo the next operation.
        /// </summary>
        public void Redo()
        {
            if (WindingBlocked) return;

            winding = true;
            {
                var groups = Split(redoables);
                var firstGroup = groups.FirstOrDefault();
                if (firstGroup != null)
                {
                    foreach (var operation in firstGroup)
                    {
                        redoables.Pop();
                        operation.Redo();
                        undoables.Push(operation);
                    }
                }
            }
            winding = false;
        }

        /// <summary>
        /// Redo across the specified operation.
        /// </summary>
        /// <param name="handle">The handle of the operation.</param>
        public void Redo(Guid handle)
        {
            if (WindingBlocked) return;
            IOperation redoable = GetOperation(redoables, handle);
            while (redoables.Peek() != redoable) Redo();
            Redo();
        }

        /// <summary>
        /// Clear all the operations.
        /// </summary>
        public void Clear()
        {
            if (WindingBlocked) return;
            undoables.Clear();
            redoables.Clear();
        }

        /// <summary>
        /// Collect all undoable operations.
        /// </summary>
        public IList<Guid> CollectUndoables(IList<Guid> result)
        {
            return CollectOperationHandles(undoables, result);
        }

        /// <summary>
        /// Collect all redoable operations.
        /// </summary>
        public IList<Guid> CollectRedoables(IList<Guid> result)
        {
            return CollectOperationHandles(redoables, result);
        }

        /// <summary>
        /// Get the name of an undo operation.
        /// </summary>
        /// <param name="handle">The handle of the undo operation.</param>
        public string GetUndoOperationName(Guid handle)
        {
            return GetOperation(undoables, handle).Name;
        }

        /// <summary>
        /// Get the name of a redo operation.
        /// </summary>
        /// <param name="handle">The handle of the redo operation.</param>
        public string GetRedoOperationName(Guid handle)
        {
            return GetOperation(redoables, handle).Name;
        }

        private Guid CalculatedGroup { get { return Grouped ? _currentGroup : Guid.NewGuid(); } }

        private bool WindingBlocked { get { return RecordingBlocked || Grouped; } }

        private bool RecordingBlocked { get { return Winding || Paused; } }

        private void Record(IOperation operation)
        {
            undoables.Push(operation);
            redoables.Clear();
        }

        private static IList<Guid> CollectOperationHandles(Stack<IOperation> operations, IList<Guid> result)
        {
            var groups = Split(operations);
            foreach (var group in groups)
            {
                IOperation firstOperation = group.First();
                result.Add(firstOperation.Handle);
            }
            return result;
        }

        private static IOperation GetOperation(IEnumerable<IOperation> operations, Guid handle)
        {
            return operations.First<IOperation>(x => x.Handle == handle);
        }

        private static IEnumerable<IGrouping<Guid, IOperation>> Split(Stack<IOperation> stack)
        {
            return stack.GroupBy<IOperation, Guid>(x => x.Group);
        }

        private readonly Stack<IOperation> undoables = new Stack<IOperation>();
        private readonly Stack<IOperation> redoables = new Stack<IOperation>();
        private bool winding;
        private int pauseDepth;
        private int groupDepth;
        private Guid _currentGroup;
    }
}

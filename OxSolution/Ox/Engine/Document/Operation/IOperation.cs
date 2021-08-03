using System;
using System.Collections.Generic;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// Represents a doable / undoable operation.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// The Guid.
        /// </summary>
        Guid Handle { get; }
        /// <summary>
        /// The group to which the operation belongs.
        /// </summary>
        Guid Group { get; }
        /// <summary>
        /// The operation's name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Is the operation done?
        /// </summary>
        bool Done { get; }
        /// <summary>
        /// Do the operation.
        /// </summary>
        void Redo();
        /// <summary>
        /// Undo the operation.
        /// </summary>
        void Undo();
    }

    /// <summary>
    /// Compare the groups of two IOperations.
    /// </summary>
    public class OperationGroupComparer : IEqualityComparer<IOperation>
    {
        /// <inheritdoc />
        public bool Equals(IOperation x, IOperation y)
        {
            // QUESTION: is this implemented right?
            return GetHashCode(x) == GetHashCode(y);
        }

        /// <inheritdoc />
        public int GetHashCode(IOperation obj)
        {
            return obj.Group.GetHashCode();
        }
    }
}

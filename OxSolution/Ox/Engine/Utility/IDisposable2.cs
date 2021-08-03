using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Augments .NET's IDisposable interface with the ability to track "garbage" object that are
    /// disposed when Dispose() is called. The garbage objects are disposed first thing, meaning
    /// that you must not interface with them inside the Dispose(bool disposing) call.
    /// Additionally, the garbage objects be disposed in the opposite order that they are added.
    /// </summary>
    public interface IDisposable2 : IDisposable
    {
        /// <summary>
        /// Has the object been disposed?
        /// </summary>
        bool Disposed { get; }
        /// <summary>
        /// Raised when the object is disposing.
        /// </summary>
        event Action<IDisposable2> Disposing;
        /// <summary>
        /// Add an IDisposable object to this instance's garbage.
        /// </summary>
        void AddGarbage(IDisposable garbage);
        /// <summary>
        /// Remove an IDisposable object from this instance's garbage.
        /// </summary>
        bool RemoveGarbage(IDisposable garbage);
    }
}

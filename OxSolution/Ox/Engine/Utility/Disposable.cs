using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Implements the standard .NET disposal pattern with a warning mechanism for objects that
    /// were not manually disposed.
    /// </summary>
    public class Disposable : IDisposable
    {
        /// <summary>
        /// Handle forgotten disposal.
        /// </summary>
        ~Disposable()
        {
            System.Diagnostics.Trace.Fail(
                "Finalizer called.",
                "It appears that you forgot to call Dispose() on this object.");
            if (!disposed)
            {
                Dispose(false);
                disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing) { }

        private bool disposed;
    }
}

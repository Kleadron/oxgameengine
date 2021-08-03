using System;
using System.Collections.Generic;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Implements the standard .NET disposal pattern with a vanilla implementation of
    /// IDisposable2's capabilities.
    /// </summary>
    public class Disposable2 : IDisposable2
    {
        /// <summary>
        /// Handle forgotten disposal.
        /// </summary>
        ~Disposable2()
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
        public bool Disposed { get { return disposed; } }

        /// <inheritdoc />
        public event Action<IDisposable2> Disposing;

        /// <inheritdoc />
        public void Dispose()
        {
            if (!disposed)
            {
                RaiseDisposing();
                DisposeBin();
                Dispose(true);
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void AddGarbage(IDisposable garbage)
        {
            garbageBin.Add(garbage);
        }

        /// <inheritdoc />
        public bool RemoveGarbage(IDisposable garbage)
        {
            return garbageBin.Remove(garbage);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing) { }

        private void RaiseDisposing()
        {
            if (Disposing != null) Disposing(this);
        }

        private void DisposeBin()
        {
            for (int i = garbageBin.Count - 1; i > -1; --i) garbageBin[i].Dispose();
        }

        private readonly IList<IDisposable> garbageBin = new List<IDisposable>();
        private bool disposed;
    }
}

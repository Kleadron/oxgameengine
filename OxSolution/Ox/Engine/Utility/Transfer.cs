using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Adds an ownership transfer semantic to a disposable object. Whoever last receives the
    /// object with the Transfer semantic is obligated to handle its disposal. The semantic is not
    /// necessary when ownership transfer is explicit like with factory methods that are prefixed
    /// with "Create".
    /// </summary>
    public struct Transfer<T> where T : IDisposable
    {
        /// <summary>
        /// Create a new Transfer struct using type inference.
        /// TODO: remove this if type inference is implemented on constructors in C#.
        /// </summary>
        public static Transfer<T> Create(T value)
        {
            return new Transfer<T>(value);
        }

        /// <summary>
        /// Create a transfer semantic.
        /// </summary>
        /// <param name="value">See property Value.</param>
        public Transfer(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The object whose ownership is being transferred.
        /// </summary>
        public readonly T Value;
    }
}

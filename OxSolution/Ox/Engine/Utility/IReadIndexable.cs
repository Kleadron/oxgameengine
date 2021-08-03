namespace Ox.Engine.Utility
{
    /// <summary>
    /// Enables an object to be indexed into in a readonly fashion.
    /// </summary>
    public interface IReadIndexable<T>
    {
        /// <summary>
        /// Get an indexed item.
        /// </summary>
        T this[int index] { get; }
        /// <summary>
        /// The number of indexable items.
        /// </summary>
        int Count { get; }
    }
}

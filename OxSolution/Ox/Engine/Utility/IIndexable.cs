namespace Ox.Engine.Utility
{
    /// <summary>
    /// Enables an object to be indexed in a read / write fashion.
    /// </summary>
    public interface IIndexable<T> : IReadIndexable<T>
    {
        /// <summary>
        /// Get / set an indexed item.
        /// </summary>
        new T this[int index] { get; set; }
    }
}

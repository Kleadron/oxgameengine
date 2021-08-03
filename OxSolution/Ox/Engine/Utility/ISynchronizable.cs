namespace Ox.Engine.Utility
{
    /// <summary>
    /// Represents an object that has synchronize operations.
    /// </summary>
    public interface ISynchronizable
    {
        /// <summary>
        /// Synchronize to the target.
        /// </summary>
        void SynchronizeTo();
        /// <summary>
        /// Synchronize from the target.
        /// </summary>
        void SynchronizeFrom();
    }
}

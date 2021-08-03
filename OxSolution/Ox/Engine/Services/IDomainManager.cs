using System;
using Ox.Engine.Utility;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// Manages the lifetimes of game domains.
    /// </summary>
    public interface IDomainManager : IDisposable
    {
        /// <summary>
        /// Get a domain, creating it if it doesn't exist.
        /// </summary>
        Domain this[string domainName] { get; }
        /// <summary>
        /// Create a domain with the specified name.
        /// </summary>
        void CreateDomain(string domainName);
        /// <summary>
        /// Destroy a domain and all the objects it owns.
        /// </summary>
        void DestroyDomain(string domainName);
        /// <summary>
        /// Does a domain exist?
        /// </summary>
        bool ContainsDomain(string domainName);
        /// <summary>
        /// Load content from the specified domain.
        /// </summary>
        T Load<T>(string fileName, string domainName);
    }
}

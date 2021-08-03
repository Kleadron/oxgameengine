using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// Manages the lifetimes of game domains.
    /// </summary>
    public class DomainManager : Disposable
    {
        /// <summary>
        /// Create a DomainManaged.
        /// </summary>
        /// <param name="services">The XNA-style services.</param>
        public DomainManager(GameServiceContainer services)
        {
            this.services = services;
        }

        /// <summary>
        /// Get a domain, creating it if it doesn't exist.
        /// </summary>
        public Domain this[string domainName]
        {
            get
            {
                CreateDomain(domainName);
                return domains[domainName];
            }
        }

        /// <summary>
        /// Create a domain with the specified name.
        /// </summary>
        public void CreateDomain(string domainName)
        {
            Domain domain;
            if (domains.TryGetValue(domainName, out domain)) return;
            domains.Add(domainName, new Domain(services));
        }

        /// <summary>
        /// Destroy a domain and all the objects it owns.
        /// </summary>
        public void DestroyDomain(string domainName)
        {
            Domain domain;
            if (!domains.TryGetValue(domainName, out domain)) return;
            domain.Dispose();
            domains.Remove(domainName);
        }

        /// <summary>
        /// Does a domain exist?
        /// </summary>
        public bool ContainsDomain(string domainName)
        {
            return domains.ContainsKey(domainName);
        }

        /// <summary>
        /// Load content from the specified domain.
        /// </summary>
        public T Load<T>(string fileName, string domainName)
        {
            return this[domainName].Load<T>(fileName);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) System.Diagnostics.Trace.Assert(domains.Count == 0,
                "You forgot to destroy the following domains: " + DomainNames);
            base.Dispose(disposing);
        }

        private string DomainNames
        {
            get
            {
                StringBuilder result = new StringBuilder();
                domains.Keys.Select(x => result.Append(x + ", "));
                return result.ToString();
            }
        }

        private readonly Dictionary<string, Domain> domains = new Dictionary<string, Domain>();
        private readonly GameServiceContainer services;
    }
}

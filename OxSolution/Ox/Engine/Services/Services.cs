using System;
using System.Collections.Generic;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// A collection of game services.
    /// </summary>
    public class Services
    {
        /// <summary>
        /// Add a service of type T.
        /// </summary>
        public void Add<T>(T service) where T : class
        {
            dictionary.Add(typeof(T), service);
        }

        /// <summary>
        /// Remove a service of type T.
        /// </summary>
        public bool Remove<T>() where T : class
        {
            return dictionary.Remove(typeof(T));
        }

        /// <summary>
        /// Query if a service of type T is present.
        /// </summary>
        public bool Contains<T>() where T : class
        {
            return dictionary.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Get a service of type T.
        /// </summary>
        public T Get<T>() where T : class
        {
            return OxHelper.Cast<T>(dictionary[typeof(T)]);
        }

        private readonly Dictionary<Type, object> dictionary = new Dictionary<Type, object>();
    }
}

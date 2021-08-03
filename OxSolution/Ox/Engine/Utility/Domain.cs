using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ox.Engine.Component;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// A domain whose children can be collectively acted upon.
    /// </summary>
    public class Domain : Disposable
    {
        /// <summary>
        /// Create a Domain.
        /// </summary>
        /// <param name="services">The XNA-style services.</param>
        public Domain(GameServiceContainer services)
        {
            contentManager = new ContentManager(services, OxConfiguration.ContentRootDirectory);
        }

        /// <summary>
        /// Add a script.
        /// </summary>
        public void AddScript(BaseComponentScript script)
        {
            scripts.Add(script);
        }

        /// <summary>
        /// Remove a script.
        /// </summary>
        public bool RemoveScript(BaseComponentScript script)
        {
            return scripts.Remove(script);
        }

        /// <summary>
        /// Collect child scripts.
        /// </summary>
        public IList<BaseComponentScript> CollectScripts(IList<BaseComponentScript> result)
        {
            result.AddRange(this.scripts);
            return result;
        }

        /// <summary>
        /// Add a component.
        /// </summary>
        public void AddComponent(OxComponent component)
        {
            components.Add(component);
        }

        /// <summary>
        /// Remove a component.
        /// </summary>
        public bool RemoveComponent(OxComponent component)
        {
            return components.Remove(component);
        }

        /// <summary>
        /// Collect child components.
        /// </summary>
        public IList<OxComponent> CollectComponents(IList<OxComponent> result)
        {
            result.AddRange(this.components);
            return result;
        }

        /// <summary>
        /// Load content from a file.
        /// </summary>
        public T Load<T>(string fileName)
        {
            return contentManager.Load<T>(fileName);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DestroyScripts();
                DestroyComponents();
                DestroyContentManager();
            }
            base.Dispose(disposing);
        }

        private void DestroyScripts()
        {
            scripts.ForEachInReverseOnCopy(x => x.Dispose()); // MEMORYCHURN
        }

        private void DestroyComponents()
        {
            components.ForEachInReverseOnCopy(x => { if (x.OwnedByDomain) x.Dispose(); }); // MEMORYCHURN
        }

        private void DestroyContentManager()
        {
            contentManager.Dispose();
        }

        private readonly IList<BaseComponentScript> scripts = new List<BaseComponentScript>();
        private readonly IList<OxComponent> components = new List<OxComponent>();
        private readonly ContentManager contentManager;
    }
}

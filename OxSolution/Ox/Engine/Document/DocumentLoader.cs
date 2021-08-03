using System.Collections.Generic;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// Can load documents of a certain type.
    /// </summary>
    public abstract class DocumentLoader
    {
        /// <summary>
        /// Load a document.
        /// </summary>
        /// <param name="engine">The engine to which the document items are destined.</param>
        /// <param name="fileName">The name of the document file.</param>
        /// <param name="domainName">The domain that will own the document items.</param>
        /// <exception cref="LoadDocumentException" />
        public void Load(OxEngine engine, string fileName, string domainName)
        {
            OxHelper.ArgumentNullCheck(engine, fileName, domainName);
            Document document = CreateDocument(engine);
            LoadDocument(fileName, document);
            SetUpDocument(engine, domainName, document);
        }

        /// <summary>
        /// Handle loading a document.
        /// </summary>
        protected abstract Document CreateDocument(OxEngine engine);

        private void LoadDocument(string fileName, Document document)
        {
            document.Load(fileName);
        }

        private void SetUpDocument(OxEngine engine, string domainName, Document document)
        {
            IList<ComponentToken> components = document.Collect(new List<ComponentToken>());
            SetUpComponents(engine, domainName, components);
            SetUpInstances(engine, document, components);
            SetUpScripts(engine, components);
            NotifyDocumentLoaded(components);
            ClearInstances(components);
        }

        private void SetUpComponents(OxEngine engine, string domainName, IList<ComponentToken> components)
        {
            for (int i = 0; i < components.Count; ++i) SetUpComponent(engine, domainName, components[i]);
        }

        private void SetUpInstances(OxEngine engine, Document document, IList<ComponentToken> components)
        {
            for (int i = 0; i < components.Count; ++i) SetUpInstance(engine, document, components[i]);
        }

        private void SetUpScripts(OxEngine engine, IList<ComponentToken> components)
        {
            for (int i = 0; i < components.Count; ++i) SetUpScript(engine, components[i]);
        }

        private void NotifyDocumentLoaded(IList<ComponentToken> components)
        {
            for (int i = 0; i < components.Count; ++i) components[i].Instance.DocumentLoaded();
        }

        private void ClearInstances(IList<ComponentToken> components)
        {
            for (int i = 0; i < components.Count; ++i) components[i].Instance = null;
        }

        private static void SetUpComponent(OxEngine engine, string domainName, ComponentToken component)
        {
            component.CreateInstance(engine, domainName, component.ScriptClass.IsEmpty());
        }

        private static void SetUpInstance(OxEngine engine, Document document, ComponentToken component)
        {
            if (component.ParentGuid == null) component.Instance.Parent = engine.Root;
            else
            {
                ComponentToken parent = document.Find<ComponentToken>(component.ParentGuid.Value);
                if (parent != null) component.Instance.Parent = parent.Instance;
                else component.Instance.Parent = engine.Root;
            }
        }

        private static void SetUpScript(OxEngine engine, ComponentToken component)
        {
            if (component.ScriptClass.IsEmpty()) return;
            string name = component.Instance.Name; // cache before mutation
            if (OxConfiguration.EncapsulateScriptedComponents) component.Instance.Encapsulate();
            CreateScript(engine, component, name);
        }

        private static void CreateScript(OxEngine engine, ComponentToken component, string name)
        {
            Transfer<OxComponent> instance = new Transfer<OxComponent>(component.Instance);
            BaseComponentScript script = engine.CreateScript(component.ScriptClass, instance);
            script.Name = name;
        }
    }
}

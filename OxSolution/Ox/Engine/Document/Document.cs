using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// The result of a delete operation.
    /// </summary>
    public enum DeleteResult
    {
        Success = 0,
        FailHasChildren,
        FailNotFound
    }

    /// <summary>
    /// The relationship between two items.
    /// </summary>
    public enum Relationship
    {
        None = 0,
        Sibling,
        Ascending,
        Descending
    }

    /// <summary>
    /// The style in which the document item was created. Affects what type of setup work takes
    /// place on it.
    /// </summary>
    public enum ItemCreationStyle
    {
        Normal = 0,
        Paste,
        Clone,
        Undelete,
        Replacement,
        Load,
        External
    }

    /// <summary>
    /// The base token to which an IDocument will serialize.
    /// </summary>
    public class DocumentToken
    {
        public DocumentToken() { }

        public DocumentToken(List<ComponentToken> components)
        {
            Components = components;
        }

        public List<ComponentToken> Components { get; set; }
    }

    /// <summary>
    /// A manipulable document composed of ComponentTokens.
    /// </summary>
    public abstract class Document
    {
        /// <summary>
        /// Create a Document.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public Document(OxEngine engine)
        {
            OxHelper.ArgumentNullCheck(engine);
            this.engine = engine;
        }

        /// <summary>
        /// The current selection.
        /// </summary>
        public Selection Selection { get { return selection; } }

        /// <summary>
        /// Raised when the structure of the document has changed.
        /// </summary>
        public event Action<Document> StructureChanged;

        /// <summary>
        /// Create a component of componentType.
        /// </summary>
        public ComponentToken CreateComponent(string componentType)
        {
            OxHelper.ArgumentNullCheck(componentType);
            ConstructionToken constructionInfo = ConstructionDictionary[componentType];
            ComponentToken component = constructionInfo.Construct<ComponentToken>();
            if (!VerifyComponentHook(component)) throw new ArgumentException("Component of type " + componentType + " is not allowed in this document.");
            InductComponent(component, ItemCreationStyle.Normal, false, false);
            return component;
        }

        /// <summary>
        /// Create a component from an external instance.
        /// </summary>
        public ComponentToken CreateComponentFrom(OxComponent instance)
        {
            OxHelper.ArgumentNullCheck(instance);
            ComponentToken component = instance.CreateToken();
            if (!VerifyComponentHook(component)) throw new ArgumentException("Component of type " + component.GetType() + " is not allowed in this document.");
            InductComponent(component, ItemCreationStyle.External, false, false);
            SynchronizeRelationshipsFrom();
            return component;
        }

        /// <summary>
        /// Create components from external instances.
        /// </summary>
        public IList<ComponentToken> CreateComponentsFrom<T>(IList<T> instances)
            where T : OxComponent
        {
            IList<ComponentToken> components = new List<ComponentToken>();
            instances.ForEach(x => components.Add(CreateComponentFrom(x)));
            return components;
        }

        /// <summary>
        /// Create a component for the purpose of replacing another one.
        /// </summary>
        public ComponentToken CreateReplacementComponent(string componentType)
        {
            OxHelper.ArgumentNullCheck(componentType);
            ConstructionToken constructionInfo = ConstructionDictionary[componentType];
            ComponentToken result = constructionInfo.Construct<ComponentToken>();
            if (!VerifyComponentHook(result)) throw new ArgumentException("Component of type " + componentType + " is not allowed in this document.");
            InductComponent(result, ItemCreationStyle.Replacement, false, false);
            return result;
        }

        /// <summary>
        /// Create a component by pasting a copy of the source.
        /// </summary>
        public ComponentToken PasteComponent(ComponentToken source)
        {
            OxHelper.ArgumentNullCheck(source);
            if (!VerifyComponentHook(source)) throw new ArgumentException("Component of type " + source.ItemType + " is not allowed in this document.");
            ComponentToken result = OxHelper.Cast<ComponentToken>(source.Duplicate());
            InductComponent(result, ItemCreationStyle.Paste, false, false);
            return result;
        }

        /// <summary>
        /// Create a component by cloning the source.
        /// May return null.
        /// </summary>
        public ComponentToken CloneComponent(ComponentToken source)
        {
            OxHelper.ArgumentNullCheck(source);
            if (!VerifyComponentHook(source)) throw new ArgumentException("Component of type " + source.ItemType + " is not allowed in this document.");
            if (!AllowCloneHook(source)) return null;
            ComponentToken result = OxHelper.Cast<ComponentToken>(source.Duplicate());
            InductComponent(result, ItemCreationStyle.Clone, false, false);
            return result;
        }

        /// <summary>
        /// Can the component be deleted from the document?
        /// </summary>
        public DeleteResult CanDeleteComponent(ComponentToken component)
        {
            if (HasChildren(component.Guid)) return DeleteResult.FailHasChildren;
            if (!components.Contains(component)) return DeleteResult.FailNotFound;
            return DeleteResult.Success;
        }

        /// <summary>
        /// Delete a component from the document.
        /// </summary>
        public bool DeleteComponent(ComponentToken component)
        {
            bool result = CanDeleteComponent(component) == DeleteResult.Success;
            if (result) ExpelComponent(component);
            return result;
        }

        /// <summary>
        /// Revive a component that has been deleted.
        /// </summary>
        /// <param name="component">The component to be revived.</param>
        /// <param name="wasSelected">
        /// Was the component selected at time of deletion?
        /// BUG: this flag is not enough to restore selection state. What is needed is the
        /// selection context itself.
        /// </param>
        public void UndeleteComponent(ComponentToken component, bool wasSelected)
        {
            OxHelper.ArgumentNullCheck(component);
            if (!VerifyComponentHook(component)) throw new ArgumentException("Component " + component.Name + " is not allowed in this document.");
            InductComponent(component, ItemCreationStyle.Undelete, wasSelected, false);
        }

        /// <summary>
        /// Revive a replacement component that has been deleted.
        /// </summary>
        /// <param name="component">The component to be revived.</param>
        /// <param name="wasSelected">
        /// Was the component selected at time of deletion?
        /// BUG: see comments on UndeleteComponent.
        /// </param>
        public void UndeleteReplacementComponent(ComponentToken component, bool wasSelected)
        {
            OxHelper.ArgumentNullCheck(component);
            if (!VerifyComponentHook(component)) throw new ArgumentException("Component " + component.Name + " is not allowed in this document.");
            InductComponent(component, ItemCreationStyle.Undelete, wasSelected, true);
        }

        /// <summary>
        /// Get the familial relationship between the two items.
        /// </summary>
        public Relationship GetRelationship(ItemToken first, ItemToken second)
        {
            OxHelper.ArgumentNullCheck(first, second);
            if (AreSiblings(first, second)) return Relationship.Sibling;
            if (IsDescending(first, second)) return Relationship.Descending;
            if (IsDescending(second, first)) return Relationship.Ascending;
            return Relationship.None;
        }

        /// <summary>
        /// Are the items siblings?
        /// </summary>
        public bool AreSiblings(ItemToken first, ItemToken second)
        {
            OxHelper.ArgumentNullCheck(first, second);
            return first.ParentGuid == second.ParentGuid;
        }

        /// <summary>
        /// Is second a descendant of first?
        /// </summary>
        public bool IsDescending(ItemToken first, ItemToken second)
        {
            OxHelper.ArgumentNullCheck(first, second);
            Guid? walk = second.Guid;
            while (walk != null)
            {
                if (first.Guid == walk.Value) return true;
                walk = Find<ItemToken>(walk.Value).ParentGuid;
            }
            return false;
        }

        /// <summary>
        /// Find the item of type T with the matching guid.
        /// </summary>
        public T Find<T>(Guid guid) where T : ItemToken
        {
            T result = null;

            Collect(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i)
                {
                    T item = cachedItems[i] as T;
                    if (item != null && item.Guid == guid)
                    {
                        result = item;
                        break;
                    }
                }
            }
            cachedItems.Clear();

            return result;
        }

        /// <summary>
        /// Collect all items of type T.
        /// </summary>
        public IList<T> Collect<T>(IList<T> result) where T : ItemToken
        {
            PreCollectHook(result);
            {
                for (int i = 0; i < components.Count; ++i)
                {
                    T component = components[i] as T;
                    if (component != null) result.Add(component);
                }
            }
            PostCollectHook(result);

            return result;
        }

        /// <summary>
        /// Collect all items of type T that satisfy a predicate.
        /// </summary>
        public IList<T> Collect<T>(Func<T, bool> predicate, IList<T> result) where T : ItemToken
        {
            Collect(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i)
                {
                    T item = cachedItems[i] as T;
                    if (item != null && predicate(item)) result.Add(item);
                }
            }
            cachedItems.Clear();

            return result;
        }

        /// <summary>
        /// Collect all items of type T with the matching parent guid.
        /// </summary>
        public IList<T> Collect<T>(Guid? parentGuid, IList<T> result) where T : ItemToken
        {
            Collect(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i)
                {
                    T item = cachedItems[i] as T;
                    if (item != null && item.ParentGuid == parentGuid) result.Add(item);
                }
            }
            cachedItems.Clear();

            return result;
        }

        /// <summary>
        /// Collect all items of type T with the matching parent guid that satisfy a predicate.
        /// </summary>
        public IList<T> Collect<T>(Guid? parentGuid, Func<T, bool> predicate, IList<T> result) where T : ItemToken
        {
            Collect(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i)
                {
                    T item = cachedItems[i] as T;
                    if (item != null && item.ParentGuid == parentGuid && predicate(item)) result.Add(item);
                }
            }
            cachedItems.Clear();

            return result;
        }

        /// <summary>
        /// Clear the document.
        /// </summary>
        public void Clear()
        {
            ExpelComponents();
            ClearHook();
        }

        /// <summary>
        /// Save the document to a file.
        /// </summary>
        /// <exception cref="SaveDocumentException" />
        public void Save(string fileName)
        {
            OxHelper.ArgumentNullCheck(fileName);            
            try
            {
                Type[] componentTypes = ConstructionDictionary.GetConstructedTypes();
                XmlSerializer serializer = new XmlSerializer(DocumentTokenTypeHook, componentTypes);
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(fileName, writerSettings))
                {
                    DocumentToken documentToken = CreateDocumentTokenHook();
                    serializer.Serialize(writer, documentToken);
                }
            }
            catch (InvalidOperationException e)
            {
                // if e is a shell for XmlException, IOException, or UnauthorizedAccessException,
                // throw a DocumentLoadException so that the exception will be specific enough to
                // handle.
                if (e.InnerException is XmlException ||
                    e.InnerException is IOException ||
                    e.InnerException is UnauthorizedAccessException)
                    throw new SaveDocumentException("Error saving document " + fileName + ".", e);
                throw;
            }
        }

        /// <summary>
        /// Load the document from a file.
        /// </summary>
        /// <exception cref="LoadDocumentException" />
        public void Load(string fileName)
        {
            OxHelper.ArgumentNullCheck(fileName);
            Clear();
            DocumentToken documentToken;
            try
            {
                Type[] componentTypes = ConstructionDictionary.GetConstructedTypes();
                XmlSerializer serializer = new XmlSerializer(DocumentTokenTypeHook, componentTypes);
                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    documentToken = OxHelper.Cast<DocumentToken>(serializer.Deserialize(reader));
                }
            }
            catch (InvalidOperationException e)
            {
                // if e is a shell for XmlException, IOException, UnauthorizedAccessException, or
                // (sadly) InvalidOperationException, throw a DocumentLoadException so that the
                // exception will be specific enough to handle.
                if (e.InnerException is XmlException ||
                    e.InnerException is IOException ||
                    e.InnerException is UnauthorizedAccessException ||
                    e.InnerException is InvalidOperationException)
                    throw new LoadDocumentException("Error loading document " + fileName + ".", e);
                throw;
            }
            Load(documentToken);
        }

        /// <inheritdoc />
        public void SynchronizeTo()
        {
            SynchronizeComponentsTo();
            SynchronizeRelationshipsTo();
        }

        /// <inheritdoc />
        public void SynchronizeFrom()
        {
            SynchronizeComponentsFrom();
            SynchronizeRelationshipsFrom();
        }

        /// <summary>
        /// The document's components.
        /// </summary>
        protected List<ComponentToken> Components { get { return components; } }

        /// <summary>
        /// The engine.
        /// </summary>
        protected OxEngine Engine { get { return engine; } }

        /// <summary>
        /// The construction dictionary used to construct components.
        /// </summary>
        protected abstract ConstructionDictionary ConstructionDictionary { get; }

        /// <summary>
        /// The type of token that the document serializes to.
        /// </summary>
        protected abstract Type DocumentTokenTypeHook { get; }

        /// <summary>
        /// The engine as T.
        /// </summary>
        protected T GetEngine<T>() where T : class
        {
            return OxHelper.Cast<T>(engine);
        }

        /// <summary>
        /// Raise the StructureChanged event.
        /// </summary>
        protected void RaiseStructureChanged()
        {
            if (StructureChanged != null) StructureChanged(this);
        }

        /// <summary>
        /// Does the item with the speicifed parent guid have children?
        /// </summary>
        protected bool HasChildren(Guid parentGuid)
        {
            bool result = false;

            Collect(cachedItems);
            {
                for (int i = 0; i < cachedItems.Count; ++i)
                {
                    if (cachedItems[i].ParentGuid == parentGuid)
                    {
                        result = true;
                        break;
                    }
                }
            }
            cachedItems.Clear();

            return result;
        }

        /// <summary>
        /// Induct an item into the document.
        /// </summary>
        protected void InductItem(ItemToken item, ItemCreationStyle creationStyle, bool wasSelected, bool wasReplacement)
        {
            ItemToken firstSelected = Selection.GetFirstOrNull<ItemToken>();
            if (creationStyle == ItemCreationStyle.Undelete)
            {
                if (wasReplacement) selection.Add(item);
                else if (wasSelected) selection.Add(item);
            }
            else if (creationStyle == ItemCreationStyle.Replacement)
            {
                selection.Add(item);
            }
            else if (creationStyle != ItemCreationStyle.Load && creationStyle != ItemCreationStyle.External)
            {
                selection.Set(item);
            }
            InductItemHook(item, firstSelected, creationStyle, wasSelected);
            item.PropertyChanged += item_PropertyChanged;
        }

        /// <summary>
        /// Expel an item from a document.
        /// </summary>
        /// <param name="item"></param>
        protected void ExpelItem(ItemToken item)
        {
            selection.Remove(item);
            item.PropertyChanged -= item_PropertyChanged;
        }

        /// <summary>
        /// Handle verifying that a component can be added to the document.
        /// </summary>
        protected abstract bool VerifyComponentHook(ComponentToken component);

        /// <summary>
        /// Handle verifying that a component is allowed to be cloned.
        /// </summary>
        protected abstract bool AllowCloneHook(ComponentToken component);

        /// <summary>
        /// Handle creating a document token that the document can serialize to.
        /// </summary>
        protected abstract DocumentToken CreateDocumentTokenHook();

        /// <summary>
        /// Handle loading the document from a document token.
        /// </summary>
        protected abstract void LoadHook(DocumentToken documentToken);

        /// <summary>
        /// Handle clearing the items from a document.
        /// </summary>
        protected abstract void ClearHook();

        /// <summary>
        /// Handle inducting an item into the document.
        /// </summary>
        /// <param name="item">The item to induct.</param>
        /// <param name="firstSelected">The first currently selected item.</param>
        /// <param name="creationStyle">The manner in which the item was created.</param>
        /// <param name="wasSelected">Was the item selected in its past life in the document?</param>
        protected abstract void InductItemHook(ItemToken item, ItemToken firstSelected, ItemCreationStyle creationStyle, bool wasSelected);

        /// <summary>
        /// Handle collecting items of T before the normal collection algorithm takes place.
        /// </summary>
        protected abstract void PreCollectHook<T>(IList<T> result) where T : ItemToken;

        /// <summary>
        /// Handle collecting items of T after the normal collection algorithm takes place.
        /// </summary>
        protected abstract void PostCollectHook<T>(IList<T> result) where T : ItemToken;

        private void item_PropertyChanged(object sender, string propertyName, object oldValue)
        {
            if (propertyName == "ParentGuid" || propertyName == "Name") RaiseStructureChanged();
        }

        private void Load(DocumentToken documentToken)
        {
            LoadHook(documentToken);
            InductComponents(documentToken);
        }

        private void InductComponents(DocumentToken documentToken)
        {
            for (int i = 0; i < documentToken.Components.Count; ++i)
                InductComponent(documentToken.Components[i], ItemCreationStyle.Load, false, false);
        }

        private void InductComponent(ComponentToken component, ItemCreationStyle creationStyle,
            bool wasSelected, bool wasReplacement)
        {
            InductItem(component, creationStyle, wasSelected, wasReplacement);
            components.Add(component);
            RaiseStructureChanged();
        }

        private void ExpelComponents()
        {
            while (components.Count != 0) ExpelComponent(components[0]);
        }

        private void ExpelComponent(ComponentToken component)
        {
            components.Remove(component);
            ExpelItem(component);
            RaiseStructureChanged();
        }

        private void SynchronizeRelationshipsFrom()
        {
            for (int i = 0; i < components.Count; ++i) SynchronizeParentFrom(components[i]);
        }

        private void SynchronizeRelationshipsTo()
        {
            for (int i = 0; i < components.Count; ++i) SynchronizeParentTo(components[i]);
        }

        private void SynchronizeComponentsFrom()
        {
            for (int i = 0; i < components.Count; ++i) components[i].SynchronizeFrom();
        }

        private void SynchronizeComponentsTo()
        {
            // NOTE: redundant since components automatically synchronize to.
            for (int i = 0; i < components.Count; ++i) components[i].SynchronizeTo();
        }

        private void SynchronizeParentFrom(ComponentToken component)
        {
            ComponentToken parent = TryFindParentFrom(component);
            if (parent != null) component.ParentGuid = parent.Guid;
            else component.ParentGuid = null;
        }

        private void SynchronizeParentTo(ComponentToken component)
        {
            ComponentToken parent = TryFindParentTo(component);
            if (parent != null) component.ParentGuid = parent.Guid;
            else component.ParentGuid = null;
        }

        private ComponentToken TryFindParentFrom(ComponentToken component)
        {
            IList<ComponentToken> candidates = Components;
            for (int i = 0; i < candidates.Count; ++i)
            {
                ComponentToken candidate = candidates[i];
                if (component.Instance.Parent == candidate.Instance) return candidate;
            }
            return null;
        }

        private ComponentToken TryFindParentTo(ComponentToken component)
        {
            IList<ComponentToken> candidates = Components;
            for (int i = 0; i < candidates.Count; ++i)
            {
                ComponentToken candidate = candidates[i];
                if (component.ParentGuid == candidate.Guid) return candidate;
            }
            return null;
        }

        private readonly IList<ItemToken> cachedItems = new List<ItemToken>();
        private readonly List<ComponentToken> components = new List<ComponentToken>();
        private readonly Selection selection = new Selection();
        private readonly OxEngine engine;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Editor
{
    public abstract class EditorController : UpdateableComponent
    {
        public EditorController(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            document = CreateDocumentHook();
            document.StructureChanged += document_StructureChanged;
            Selection.SelectionChanged += selection_SelectionChanged;
        }

        public OperationRecorder OperationRecorder { get { return operationRecorder; } }

        public Selection Selection { get { return document.Selection; } }

        public event Action<EditorController> StructureChanged;

        public T Find<T>(Guid guid) where T : ItemToken
        {
            return document.Find<T>(guid);
        }

        public IEnumerable<T> GetItems<T>() where T : ItemToken
        {
            return document.Collect(new List<T>());
        }

        public IEnumerable<T> GetItems<T>(Func<T, bool> predicate) where T : ItemToken
        {
            return document.Collect(predicate, new List<T>());
        }

        public IEnumerable<T> GetItems<T>(Guid? parentGuid) where T : ItemToken
        {
            return document.Collect(parentGuid, new List<T>());
        }

        public IEnumerable<T> GetItems<T>(Guid? parentGuid, Func<T, bool> predicate) where T : ItemToken
        {
            return document.Collect(parentGuid, predicate, new List<T>());
        }

        public ComponentToken CreateComponent(string componentType)
        {
            OxHelper.ArgumentNullCheck(componentType);
            ComponentToken result = DoCreateComponent(componentType);
            Action redo = delegate { UndeleteComponent(result, true); };
            Action undo = delegate { DoDeleteComponent(result); };
            operationRecorder.Record("Create Component", undo, redo);
            return result;
        }

        public void Freeze()
        {
            OperationRecorder.PushGroup();
            {
                Selection.OfType<ComponentToken>().ForEach(x => x.Frozen = true);
            }
            OperationRecorder.PopGroup();
        }

        public void ToggleFreezing()
        {
            OperationRecorder.PushGroup();
            {
                Selection.OfType<ComponentToken>().ForEach(x => x.Frozen = !x.Frozen);
            }
            OperationRecorder.PopGroup();
        }

        public DeleteResult Delete()
        {
            DeleteResult result = CanDelete();
            if (result != DeleteResult.Success) return result;

            OperationRecorder.PushGroup();
            {
                Selection.ForEachOnCopy(x => DeleteItem(x));
            }
            OperationRecorder.PopGroup();

            return result;
        }

        public void Copy()
        {
            if (Selection.Count != 1) copiedObject = null;
            else
            {
                operationRecorder.PushPause();
                {
                    copiedObject = Selection.GetFirst<ItemToken>().Duplicate();
                }
                operationRecorder.PopPause();
            }
        }

        /// <summary>May return null.</summary>
        public object Paste()
        {
            if (Selection.Count > 1) return null;
            if (copiedObject == null) return null;
            return PasteItem(copiedObject);
        }

        public void ChangeType(ComponentToken component, string newType)
        {
            OxHelper.ArgumentNullCheck(component, newType);
            if (newType == component.ItemType) return;
            if (CanDeleteItem(component) != DeleteResult.Success) return;
            ComponentToken replacement;
            if (!ChangeComponentType(component, newType, out replacement)) return;
            Action redo = delegate { ReplaceComponent(component, replacement); };
            Action undo = delegate { ReplaceComponent(replacement, component); };
            operationRecorder.Record("Change Type", undo, redo);
        }

        public void SelectAll()
        {
            Selection.SetRange(GetItems<ComponentToken>());
        }

        public void SelectFamily()
        {
            if (Selection.Count != 1) return;
            var item = Selection.First as ItemToken;
            if (item == null) return;
            var components = GetDescendants<ComponentToken>(item.Guid).ToList();
            var component = item as ComponentToken;
            if (component != null) components.Add(component);
            Selection.SetRange(components);
        }

        public void SelectFamilyAcross()
        {
            if (Selection.Count != 1) return;
            var item = Selection.First as ItemToken;
            if (item == null) return;
            var components = GetDescendants<ComponentToken>(item.ParentGuid);
            Selection.SetRange(components);
        }

        public void SelectSiblings()
        {
            if (Selection.Count != 1) return;
            var item = Selection.First as ItemToken;
            if (item == null) return;
            var siblings = GetItems<ItemToken>(item.ParentGuid);
            var components = siblings.OfType<ComponentToken>();
            Selection.SetRange(components);
        }

        public void SelectSameType()
        {
            if (Selection.Count != 1) return;
            var component = Selection.First as ComponentToken;
            if (component == null) return;
            var matches = GetItems<ComponentToken>(x => x.GetType() == component.GetType());
            Selection.SetRange(matches);
        }

        public void SelectSameScript()
        {
            if (Selection.Count != 1) return;
            var component = Selection.First as ComponentToken;
            if (component == null) return;
            var matches = GetItems<ComponentToken>(x => x.ScriptClass == component.ScriptClass);
            Selection.SetRange(matches);
        }

        /// <summary>May return null.</summary>
        public object Clone()
        {
            if (Selection.Count != 1) return null;
            return CloneItem(Selection.FirstOrNull);
        }

        public void NewDocument()
        {
            operationRecorder.PushPause();
            {
                TearDownDocument();
                document.Clear();
            }
            operationRecorder.PopPause();

            operationRecorder.Clear();

            NewDocumentHook();
        }

        /// <summary>
        /// Save the current document.
        /// </summary>
        /// <exception cref="SaveDocumentException" />
        public void SaveDocument(string fileName)
        {
            SaveDocumentHook();
            document.Save(fileName);
        }

        /// <summary>
        /// Load a document.
        /// </summary>
        /// <exception cref="LoadDocumentException" />
        public void LoadDocument(string fileName)
        {
            operationRecorder.PushPause();
            {
                TearDownDocument();
                {
                    try
                    {
                        document.Load(fileName);
                    }
                    catch (LoadDocumentException)
                    {
                        operationRecorder.PopPause();
                        throw;
                    }
                }
                SetUpDocument();
            }
            operationRecorder.PopPause();

            operationRecorder.Clear();

            LoadDocumentHook();
        }

        protected object CopiedObject
        {
            get { return copiedObject; }
            set { copiedObject = value; }
        }
        
        protected Document Document { get { return document; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing) TearDownDocument();
            base.Dispose(disposing);
        }

        protected abstract void SaveDocumentHook();

        protected abstract void LoadDocumentHook();

        protected abstract void NewDocumentHook();

        protected abstract Document CreateDocumentHook();

        protected abstract DeleteResult CanDeleteItemHook(object item);

        protected abstract void DeleteItemHook(object item);

        protected abstract object PasteItemHook(object item);

        protected abstract object CloneItemHook(object item);

        protected abstract void SetUpComponentHook(ComponentToken component, ItemCreationStyle creationStyle);

        private void component_PropertyChanged(object sender, string propertyName, object oldValue)
        {
            if (propertyName == "ParentGuid") SetUpInstanceParent(OxHelper.Cast<ComponentToken>(sender));
        }

        private void document_StructureChanged(object sender)
        {
            if (StructureChanged != null) StructureChanged(this);
        }

        private void selection_SelectionChanged(Selection sender, IEnumerable oldSelection)
        {
            IEnumerable newSelection = Selection.ToArray();
            Action undo = delegate { SelectRange(oldSelection); };
            Action redo = delegate { SelectRange(newSelection); };
            OperationRecorder.Record("Change Selected", undo, redo);
        }

        private DeleteResult CanDelete()
        {
            foreach (DeleteResult deleteResult in Selection.Select(x => CanDeleteItem(x)))
                if (deleteResult != DeleteResult.Success)
                    return deleteResult;
            return DeleteResult.Success;
        }

        private void SelectRange(IEnumerable selection)
        {
            operationRecorder.PushPause();
            {
                Selection.SetRange(selection);
            }
            operationRecorder.PopPause();
        }

        private DeleteResult CanDeleteItem(object item)
        {
            ComponentToken component = item as ComponentToken;
            if (component != null) return Document.CanDeleteComponent(component);
            return CanDeleteItemHook(item);
        }

        private void DeleteItem(object item)
        {
            ComponentToken component = item as ComponentToken;
            if (component != null) DeleteComponent(component);
            else DeleteItemHook(item);
        }

        /// <summary>May return null.</summary>
        private object PasteItem(object item)
        {
            ComponentToken component = item as ComponentToken;
            if (component != null) return PasteComponent(component);
            return PasteItemHook(item);
        }

        /// <summary>May return null.</summary>
        private object CloneItem(object item)
        {
            ComponentToken component = item as ComponentToken;
            if (component != null) return CloneComponent(component);
            return CloneItemHook(item);
        }

        private void DeleteComponent(ComponentToken component)
        {
            bool isSelected = Selection.Contains(component);
            DoDeleteComponent(component);
            Action redo = delegate { DoDeleteComponent(component); };
            Action undo = delegate { UndeleteComponent(component, isSelected); };
            operationRecorder.Record("Delete Component", undo, redo);
        }

        private object PasteComponent(ComponentToken source)
        {
            ComponentToken result = DoPasteComponent(source);
            Action redo = delegate { UndeleteComponent(result, false); };
            Action undo = delegate { DoDeleteComponent(result); };
            operationRecorder.Record("Paste Component", undo, redo);
            return result;
        }

        private void ReplaceComponent(ComponentToken item, ComponentToken replacement)
        {
            operationRecorder.PushPause();
            {
                TearDownComponent(item);
                document.DeleteComponent(item);
                document.UndeleteComponent(replacement, true);
                SetUpComponent(replacement, ItemCreationStyle.Replacement);
            }
            operationRecorder.PopPause();
        }

        /// <summary>May return null.</summary>
        private object CloneComponent(ComponentToken source)
        {
            ComponentToken result = DoCloneComponent(source);
            if (result == null) return null;
            Action redo = delegate { UndeleteComponent(result, false); };
            Action undo = delegate { DoDeleteComponent(result); };
            operationRecorder.Record("Clone Component", undo, redo);
            return result;
        }

        private bool ChangeComponentType(ComponentToken component, string newType, out ComponentToken replacement)
        {
            bool success = false;

            OperationRecorder.PushPause();
            {
                replacement = document.CreateReplacementComponent(OxHelper.AffixToken(newType));
                SetUpComponent(replacement, ItemCreationStyle.Replacement);
                try
                {
                    replacement.Impersonate(component);
                    success = true;
                }
                catch (Exception e)
                {
                    TearDownComponent(replacement);
                    document.DeleteComponent(replacement);
                    MessageBox.Show(e.Message, "Type change error.");
                }

                if (success)
                {
                    TearDownComponent(component);
                    document.DeleteComponent(component);
                    if (component.ItemName != OxConfiguration.DefaultItemName) replacement.Name = component.Name;
                }
            }
            OperationRecorder.PopPause();

            return success;
        }

        private void UndeleteComponent(ComponentToken component, bool wasSelected)
        {
            operationRecorder.PushPause();
            {
                document.UndeleteComponent(component, wasSelected);
                SetUpComponent(component, ItemCreationStyle.Undelete);
            }
            operationRecorder.PopPause();
        }

        private ComponentToken DoCreateComponent(string componentType)
        {
            ComponentToken result;

            operationRecorder.PushPause();
            {
                result = document.CreateComponent(componentType);
                SetUpComponent(result, ItemCreationStyle.Normal);
            }
            operationRecorder.PopPause();

            return result;
        }

        private void DoDeleteComponent(ComponentToken component)
        {
            operationRecorder.PushPause();
            {
                TearDownComponent(component);
                document.DeleteComponent(component);
            }
            operationRecorder.PopPause();
        }

        private ComponentToken DoCloneComponent(ComponentToken source)
        {
            ComponentToken result;

            operationRecorder.PushPause();
            {
                result = document.CloneComponent(OxHelper.Cast<ComponentToken>(source));
                if (result != null) SetUpComponent(result, ItemCreationStyle.Clone);
            }
            operationRecorder.PopPause();

            return result;
        }

        private ComponentToken DoPasteComponent(ComponentToken source)
        {
            ComponentToken result;

            operationRecorder.PushPause();
            {
                result = document.PasteComponent(OxHelper.Cast<ComponentToken>(source));
                SetUpComponent(result, ItemCreationStyle.Paste);
            }
            operationRecorder.PopPause();

            return result;
        }

        private void SetUpDocument()
        {
            IEnumerable<ComponentToken> components = GetItems<ComponentToken>();
            components.ForEach(x => SetUpInstance(x));
            components.ForEach(x => SetUpInstanceParent(x));
            components.ForEach(x => SetUpComponentHook(x, ItemCreationStyle.Load));
        }

        private void TearDownDocument()
        {
            GetItems<ComponentToken>().ForEach( x => TearDownComponent(x));
        }

        private void SetUpComponent(ComponentToken component, ItemCreationStyle creationStyle)
        {
            SetUpInstance(component);
            SetUpInstanceParent(component);
            SetUpComponentHook(component, creationStyle);
        }

        private void TearDownComponent(ComponentToken component)
        {
            TearDownInstance(component);
            component.PropertyChanged -= component_PropertyChanged;
        }

        private void SetUpInstance(ComponentToken component)
        {
            component.PropertyChanged += component_PropertyChanged;
            component.CreateInstance(Engine, DomainName, true);
        }

        private void SetUpInstanceParent(ComponentToken component)
        {
            if (component.ParentGuid == null) component.Instance.Parent = Engine.Root;
            else
            {
                ComponentToken parent = document.Find<ComponentToken>(component.ParentGuid.Value);
                if (parent != null) component.Instance.Parent = parent.Instance;
                else component.Instance.Parent = Engine.Root;
            }
        }

        private static void TearDownInstance(ComponentToken component)
        {
            // the instance may be null if an unhandled exception is thrown during set up.
            if (component.Instance == null) return;
            component.Instance.Dispose();
            component.Instance = null;
        }

        private IEnumerable<T> GetDescendants<T>(Guid? guid) where T : ItemToken
        {
            var result = new List<T>();
            foreach (ItemToken sibling in GetItems<ItemToken>(guid))
            {
                T siblingAsT = sibling as T;
                if (siblingAsT != null) result.Add(siblingAsT);
                result.AddRange(GetDescendants<T>(sibling.Guid));
            }
            return result;
        }

        private readonly OperationRecorder operationRecorder = new OperationRecorder();
        private readonly Document document;
        /// <summary>May be null.</summary>
        private object copiedObject;
    }
}

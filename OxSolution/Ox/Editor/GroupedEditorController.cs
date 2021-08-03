using System;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;

namespace Ox.Editor
{
    public abstract class GroupedEditorController : EditorController
    {
        public GroupedEditorController(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            document = OxHelper.Cast<GroupedDocument>(base.Document);
        }

        public void CreateGroup()
        {
            GroupToken group;

            OperationRecorder.PushPause();
            {
                group = document.CreateGroup();
            }
            OperationRecorder.PopPause();

            Action redo = delegate { UndeleteGroup(group, false); };
            Action undo = delegate { DoDeleteGroup(group); };
            OperationRecorder.Record("Create Group", undo, redo);
        }

        protected new GroupedDocument Document { get { return document; } }

        protected override DeleteResult CanDeleteItemHook(object item)
        {
            GroupToken group = item as GroupToken;
            if (group != null) return Document.CanDeleteGroup(group);
            return DeleteResult.FailNotFound;
        }

        protected override void DeleteItemHook(object item)
        {
            GroupToken group = item as GroupToken;
            if (group != null) DeleteGroup(group);
        }

        protected override object PasteItemHook(object item)
        {
            GroupToken group = CopiedObject as GroupToken;
            if (group != null) PasteGroup(group);
            return group;
        }

        protected override object CloneItemHook(object item)
        {
            GroupToken group = item as GroupToken;
            if (group != null) CloneGroup(group);
            return group;
        }

        private void DeleteGroup(GroupToken group)
        {
            bool isSelected = Selection.Contains(group);

            OperationRecorder.PushPause();
            {
                document.DeleteGroup(group);
            }
            OperationRecorder.PopPause();

            Action redo = delegate { DoDeleteGroup(group); };
            Action undo = delegate { UndeleteGroup(group, isSelected); };
            OperationRecorder.Record("Delete Group", undo, redo);
        }

        private object PasteGroup(GroupToken source)
        {
            GroupToken result;

            OperationRecorder.PushPause();
            {
                result = document.PasteGroup(OxHelper.Cast<GroupToken>(source));
            }
            OperationRecorder.PopPause();

            Action redo = delegate { UndeleteGroup(result, false); };
            Action undo = delegate { DoDeleteGroup(result); };
            OperationRecorder.Record("Paste Group", undo, redo);
            return result;
        }

        private object CloneGroup(GroupToken source)
        {
            GroupToken result;

            OperationRecorder.PushPause();
            {
                result = document.CloneGroup(OxHelper.Cast<GroupToken>(source));
            }
            OperationRecorder.PopPause();

            Action redo = delegate { UndeleteGroup(result, false); };
            Action undo = delegate { DoDeleteGroup(result); };
            OperationRecorder.Record("Clone Group", undo, redo);
            return result;
        }

        private void UndeleteGroup(GroupToken group, bool wasSelected)
        {
            OperationRecorder.PushPause();
            {
                document.UndeleteGroup(group, wasSelected);
            }
            OperationRecorder.PopPause();
        }

        private void DoDeleteGroup(GroupToken group)
        {
            OperationRecorder.PushPause();
            {
                document.DeleteGroup(group);
            }
            OperationRecorder.PopPause();
        }

        private readonly GroupedDocument document;
    }
}

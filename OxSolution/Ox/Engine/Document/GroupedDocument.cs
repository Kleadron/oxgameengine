using System.Collections.Generic;
using Ox.Engine.Component;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// The base token to which a document will serialize.
    /// </summary>
    public class GroupedDocumentToken : DocumentToken
    {
        public GroupedDocumentToken() { }

        public GroupedDocumentToken(List<GroupToken> groups, List<ComponentToken> components)
            : base(components)
        {
            Groups = groups;
        }

        public List<GroupToken> Groups { get; set; }
    }

    /// <summary>
    /// A document that can recursively group its items.
    /// </summary>
    public abstract class GroupedDocument : Document
    {
        /// <summary>
        /// Create a GroupedDocument.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GroupedDocument(OxEngine engine) : base(engine) { }

        /// <summary>
        /// Create a new group in the document.
        /// </summary>
        public GroupToken CreateGroup()
        {
            GroupToken result = new GroupToken();
            InductGroup(result, ItemCreationStyle.Normal, false);
            return result;
        }

        /// <summary>
        /// Create a component by pasting a duplicate of the source.
        /// </summary>
        public GroupToken PasteGroup(GroupToken source)
        {
            GroupToken result = OxHelper.Cast<GroupToken>(source.Duplicate());
            InductGroup(result, ItemCreationStyle.Paste, false);
            return result;
        }

        /// <summary>
        /// Create a component by cloning the source.
        /// </summary>
        public GroupToken CloneGroup(GroupToken source)
        {
            GroupToken result = OxHelper.Cast<GroupToken>(source.Duplicate());
            InductGroup(result, ItemCreationStyle.Clone, false);
            return result;
        }

        /// <summary>
        /// Can the group be deleted from the document?
        /// </summary>
        public DeleteResult CanDeleteGroup(GroupToken group)
        {
            if (HasChildren(group.Guid)) return DeleteResult.FailHasChildren;
            if (!groups.Contains(group)) return DeleteResult.FailNotFound;
            return DeleteResult.Success;
        }

        /// <summary>
        /// Delete a group from the documents.
        /// </summary>
        public bool DeleteGroup(GroupToken group)
        {
            bool result = CanDeleteGroup(group) == DeleteResult.Success;
            if (result) ExpelGroup(group);
            return result;
        }

        /// <summary>
        /// Revive a group that has been deleted.
        /// </summary>
        /// <param name="group">The group to be revived.</param>
        /// <param name="wasSelected">Was the group selected at time of deletion?</param>
        public void UndeleteGroup(GroupToken group, bool wasSelected)
        {
            OxHelper.ArgumentNullCheck(group);
            InductGroup(group, ItemCreationStyle.Undelete, wasSelected);
        }

        /// <summary>
        /// The document groups.
        /// </summary>
        protected List<GroupToken> Groups { get { return groups; } }

        /// <inheritdoc />
        protected override bool VerifyComponentHook(ComponentToken component) { return true; }

        /// <inheritdoc />
        protected override bool AllowCloneHook(ComponentToken component) { return true; }

        /// <inheritdoc />
        protected override void PreCollectHook<V>(IList<V> result)
        {
            for (int i = 0; i < groups.Count; ++i)
            {
                V group = groups[i] as V;
                if (group != null) result.Add(group);
            }
        }

        /// <inheritdoc />
        protected override void PostCollectHook<V>(IList<V> result) { }

        /// <inheritdoc />
        protected override void LoadHook(DocumentToken documentToken)
        {
            GroupedDocumentToken groupedDocumentToken = OxHelper.Cast<GroupedDocumentToken>(documentToken);
            InductGroups(groupedDocumentToken.Groups, ItemCreationStyle.Load);
        }

        /// <inheritdoc />
        protected override void InductItemHook(
            ItemToken item, ItemToken firstSelected, ItemCreationStyle creationStyle, bool wasSelected)
        {
            if (creationStyle != ItemCreationStyle.Load &&
                creationStyle != ItemCreationStyle.External &&
                creationStyle != ItemCreationStyle.Undelete &&
                creationStyle != ItemCreationStyle.Replacement)
            {
                if (firstSelected != null)
                {
                    GroupToken group = FindGroupOf(firstSelected);
                    if (group != null) item.ParentGuid = group.Guid;
                }
            }
        }

        /// <inheritdoc />
        protected override void ClearHook()
        {
            ExpelGroups();
        }

        /// <summary>May return null.</summary>
        private GroupToken FindGroupOf(ItemToken item)
        {
            for ( ; ; )
            {
                GroupToken group = item as GroupToken;
                if (group != null) return group;
                if (item.ParentGuid == null) break;
                item = Find<ItemToken>(item.ParentGuid.Value);
            }

            return null;
        }

        private void InductGroups(IList<GroupToken> groups, ItemCreationStyle creationStyle)
        {
            for (int i = 0; i < groups.Count; ++i) InductGroup(groups[i], creationStyle, false);
        }

        private void InductGroup(GroupToken group, ItemCreationStyle creationStyle, bool wasSelected)
        {
            InductItem(group, creationStyle, wasSelected, false);
            groups.Add(group);
            RaiseStructureChanged();
        }

        private void ExpelGroups()
        {
            while (groups.Count != 0) ExpelGroup(groups[0]);
        }

        private void ExpelGroup(GroupToken group)
        {
            groups.Remove(group);
            ExpelItem(group);
            RaiseStructureChanged();
        }

        private readonly List<GroupToken> groups = new List<GroupToken>();
    }
}

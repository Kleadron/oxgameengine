using System;
using System.Collections.Generic;
using Ox.Engine.Component;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A document that has a single item as its root.
    /// </summary>
    public abstract class RootedDocument : Document
    {
        public RootedDocument(OxEngine engine) : base(engine) { }

        /// <summary>
        /// The root item.
        /// May be null.
        /// </summary>
        public ItemToken Root
        {
            get
            {
                ItemToken result = null;

                Collect((Guid?)null, cachedItems);
                {
                    System.Diagnostics.Trace.Assert(cachedItems.Count <= 1,
                        "There should only be one item without a parent in a rooted document.");
                    if (cachedItems.Count != 0) result = cachedItems[0];
                }
                cachedItems.Clear();

                return result;
            }
        }

        /// <inheritdoc />
        protected override bool AllowCloneHook(ComponentToken component) { return component != Root; }

        /// <inheritdoc />
        protected override void PreCollectHook<T>(IList<T> result) { }

        /// <inheritdoc />
        protected override void PostCollectHook<T>(IList<T> result) { }

        /// <inheritdoc />
        protected override void LoadHook(DocumentToken documentToken) { }

        /// <inheritdoc />
        protected override void ClearHook() { }

        /// <inheritdoc />
        protected override void InductItemHook(
            ItemToken item, ItemToken firstSelected, ItemCreationStyle creationStyle, bool wasSelected)
        {
            if (creationStyle == ItemCreationStyle.Clone)
            {
                if (firstSelected != null) item.ParentGuid = firstSelected.ParentGuid;
                else System.Diagnostics.Trace.Fail("A clone should never happen on a orphan in a rooted document.");
            }
            else if (
                creationStyle != ItemCreationStyle.Load &&
                creationStyle != ItemCreationStyle.External &&
                creationStyle != ItemCreationStyle.Undelete &&
                creationStyle != ItemCreationStyle.Replacement)
            {
                if (firstSelected != null) item.ParentGuid = firstSelected.Guid;
                else if (Root != null) item.ParentGuid = Root.Guid;
            }
        }

        private readonly IList<ItemToken> cachedItems = new List<ItemToken>();
    }
}

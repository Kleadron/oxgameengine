using System;
using System.Windows.Forms;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Editor
{
    public class GroupedEditorTreeViewWrapper : EditorTreeViewWrapper
    {
        public GroupedEditorTreeViewWrapper(OxEngine engine, EditorController controller, TreeView treeView)
            : base(engine, controller, treeView) { }

        protected override void DragDropHook(TreeNode targetNode, TreeNode draggedNode)
        {
            if (draggedNode.Equals(targetNode)) return;
            if (targetNode == null)
            {
                GroupToken draggedGroup = Controller.Find<GroupToken>(new Guid(draggedNode.Name));
                if (draggedGroup != null) draggedGroup.ParentGuid = null;
                else
                {
                    ComponentToken draggedComponent = Controller.Find<ComponentToken>(new Guid(draggedNode.Name));
                    if (draggedComponent != null) draggedComponent.ParentGuid = null;
                }
            }
            else if (!ContainsNode(draggedNode, targetNode))
            {
                GroupToken targetGroup = Controller.Find<GroupToken>(new Guid(targetNode.Name));
                if (targetGroup != null)
                {
                    ComponentToken draggedComponent = Controller.Find<ComponentToken>(new Guid(draggedNode.Name));
                    if (draggedComponent != null) draggedComponent.ParentGuid = targetGroup.Guid;
                    else
                    {
                        GroupToken draggedGroup = Controller.Find<GroupToken>(new Guid(draggedNode.Name));
                        if (draggedGroup != null) draggedGroup.ParentGuid = targetGroup.Guid;
                    }
                }
                else
                {
                    ComponentToken targetComponent = Controller.Find<ComponentToken>(new Guid(targetNode.Name));
                    if (targetComponent != null)
                    {
                        ComponentToken draggedComponent = Controller.Find<ComponentToken>(new Guid(draggedNode.Name));
                        if (draggedComponent != null) draggedComponent.ParentGuid = targetComponent.Guid;
                    }
                }
            }
        }
    }
}

using System;
using System.Windows.Forms;
using Ox.Engine;
using Ox.Engine.Component;

namespace Ox.Editor
{
    public class RootedEditorTreeViewWrapper : EditorTreeViewWrapper
    {
        public RootedEditorTreeViewWrapper(OxEngine engine, EditorController controller, TreeView treeView)
            : base(engine, controller, treeView) { }

        protected override void DragDropHook(TreeNode targetNode, TreeNode draggedNode)
        {
            if (draggedNode.Equals(targetNode)) return;
            if (targetNode != null && !ContainsNode(draggedNode, targetNode))
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;
using SysDrawing = System.Drawing;

namespace Ox.Editor
{
    public abstract class EditorTreeViewWrapper : Disposable
    {
        public EditorTreeViewWrapper(OxEngine engine, EditorController controller, TreeView treeView)
        {
            OxHelper.ArgumentNullCheck(engine, controller, treeView);

            this.controller = controller;
            this.treeView = treeView;

            treeView.NodeMouseClick += treeView_NodeMouseClick;
            treeView.MouseDown += treeView_MouseDown;
            treeView.AfterSelect += treeView_AfterSelect;
            treeView.ItemDrag += treeView_ItemDrag;
            treeView.DragEnter += treeView_DragEnter;
            treeView.DragDrop += treeView_DragDrop;
            treeView.AfterExpand += treeView_AfterExpand;
            treeView.AfterCollapse += treeView_AfterCollapse;
            controller.Selection.SelectionChanged += selection_SelectionChanged;
            controller.Selection.SelectionPropertyChanged += selection_SelectionPropertyChanged;
            controller.StructureChanged += controller_StructureChanged;
        }

        protected EditorController Controller { get { return controller; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                controller.StructureChanged -= controller_StructureChanged;
                controller.Selection.SelectionPropertyChanged -= selection_SelectionPropertyChanged;
                controller.Selection.SelectionChanged -= selection_SelectionChanged;
                treeView.AfterCollapse -= treeView_AfterCollapse;
                treeView.AfterExpand -= treeView_AfterExpand;
                treeView.DragDrop -= treeView_DragDrop;
                treeView.DragEnter -= treeView_DragEnter;
                treeView.ItemDrag -= treeView_ItemDrag;
                treeView.AfterSelect -= treeView_AfterSelect;
                treeView.MouseDown -= treeView_MouseDown;
                treeView.NodeMouseClick -= treeView_NodeMouseClick;
            }
            base.Dispose(disposing);
        }

        protected bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;
            return ContainsNode(node1, node2.Parent);
        }

        protected abstract void DragDropHook(TreeNode targetNode, TreeNode draggedNode);

        private Guid? SelectedGuid
        {
            get
            {
                TreeNode selectedNode = treeView.SelectedNode;
                return selectedNode != null ? (Guid?)new Guid(selectedNode.Name) : null;
            }
        }

        private ItemToken SelectedItem
        {
            get
            {
                Guid? selectedGuid = SelectedGuid;
                return selectedGuid != null ? controller.Find<ItemToken>(selectedGuid.Value) : null;
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView.SelectedNode = e.Node;
                controller.Selection.Set(SelectedItem);
            }
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                TreeNode targetNode = treeView.GetNodeAt(e.X, e.Y);
                treeView.SelectedNode = targetNode; // NOTE: this doesn't raise AfterSelect!
                ItemToken selectedItem = SelectedItem;
                if (selectedItem == null) controller.Selection.Clear();
                else controller.Selection.Set(selectedItem);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // this event will sometimes get called for unknown reasons (such as the tree view
            // control itself being focused). This triggers inappropriate behavior in this method.
            // Therefore, we filter out this case.
            if (e.Action != TreeViewAction.Unknown) controller.Selection.Set(SelectedItem);
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left) treeView.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            SysDrawing.Point scrollPosition = treeView.GetScrollPosition();
            {
                SysDrawing.Point targetPoint = treeView.PointToClient(new SysDrawing.Point(e.X, e.Y));
                TreeNode targetNode = treeView.GetNodeAt(targetPoint);
                TreeNode draggedNode = OxHelper.Cast<TreeNode>(e.Data.GetData(typeof(TreeNode)));
                DragDropHook(targetNode, draggedNode);
                RefreshTreeView();
            }
            treeView.SetScrollPosition(scrollPosition);
        }

        private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            ItemToken item = controller.Find<ItemToken>(new Guid(e.Node.Name));
            if (item != null) item.Expanded = true;
        }

        private void treeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            ItemToken item = controller.Find<ItemToken>(new Guid(e.Node.Name));
            if (item != null) item.Expanded = false;
        }

        private void selection_SelectionPropertyChanged(
            object sender, object target, string propertyName, object oldValue) { }

        private void selection_SelectionChanged(object sender, IEnumerable oldSelection)
        {
            if (controller.Selection.Count != 1) treeView.SelectedNode = null;
            else
            {
                ItemToken selectedItem = controller.Selection.FirstOrNull as ItemToken;
                treeView.SelectedNode = FindTreeNode(selectedItem);
            }
        }

        private void controller_StructureChanged(object sender)
        {
            RefreshTreeView();
            EnsureSelectedItemVisible();
        }

        private void EnsureSelectedItemVisible()
        {
            ItemToken selectedItem = controller.Selection.FirstOrNull as ItemToken;
            TreeNode node = FindTreeNode(selectedItem);
            if (node != null) node.EnsureVisible();
        }

        /// <summary>
        /// Find a tree node that matches an item.
        /// </summary>
        /// <param name="item">The item to match. May be null.</param>
        private TreeNode FindTreeNode(ItemToken item)
        {
            if (item == null) return null;
            TreeNode[] nodes = treeView.Nodes.Find(item.Guid.ToString(), true);
            if (nodes.GetLength(0) == 0) return null;
            return nodes[0];
        }

        private void RefreshTreeView()
        {
            controller.OperationRecorder.PushPause();
            {
                treeView.Nodes.Clear();
                BuildTreeNodes();
                ExpandTreeNodes();
            }
            controller.OperationRecorder.PopPause();
        }

        private void BuildTreeNodes()
        {
            controller
                .GetItems<ItemToken>()
                .Where(x => x.ParentGuid == null)
                .ForEach(x => BuildTreeNode(x, null));
        }

        private void ExpandTreeNodes()
        {
            controller
                .GetItems<ItemToken>()
                .Where(x => x.ParentGuid == null)
                .ForEach(x => ExpandTreeNode(x));
        }

        /// <summary>
        /// Build a tree node recursively.
        /// </summary>
        /// <param name="item">The current item.</param>
        /// <param name="parentNode">The current item's parent node. May be null.</param>
        private void BuildTreeNode(ItemToken item, TreeNode parentNode)
        {
            TreeNode node = new TreeNode(OxHelper.StripGuid(item.Name));
            node.Name = item.Guid.ToString();
            if (parentNode == null) treeView.Nodes.Add(node);
            else parentNode.Nodes.Add(node);

            controller
                .GetItems<ItemToken>()
                .Where(x => x.ParentGuid == item.Guid)
                .ForEach( x => BuildTreeNode(x, node));
        }

        /// <summary>
        /// Expand a tree node recursively.
        /// </summary>
        /// <param name="item">The current item.</param>
        private void ExpandTreeNode(ItemToken item)
        {
            TreeNode[] nodes = treeView.Nodes.Find(item.Guid.ToString(), true);
            if (nodes.GetLength(0) != 0)
            {
                TreeNode node = nodes[0];
                if (item.Expanded) node.Expand();
            }

            controller
                .GetItems<ItemToken>()
                .Where(x => x.ParentGuid == item.Guid)
                .ForEach( x => ExpandTreeNode(x));
        }

        private readonly EditorController controller;
        private readonly TreeView treeView;
    }
}

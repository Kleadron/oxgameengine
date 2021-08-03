using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui.Component;

namespace GuiEditorNamespace
{
    public class GuiComponentDragger : MouseDragger<GuiEditorController>
    {
        public GuiComponentDragger(OxEngine engine, GuiEditorController parent, Document document)
            : base(engine, parent, dragBeginDelay)
        {
            this.document = document;
        }

        public float Snap
        {
            get { return snap; }
            set
            {
                value = Math.Max(value, float.Epsilon); // VALIDATION
                snap = value;
            }
        }

        protected override void PrepareDragHook(Vector2 mousePosition)
        {
            selected = FirstSelectedComponent;
            if (!CanDrag) return;
            switch (Modifier)
            {
                case KeyboardModifier.ControlShift: BeginComponentScale(DragMode.VerticalScale, mousePosition); break;
                case KeyboardModifier.Control: BeginComponentScale(DragMode.FreeScale, mousePosition); break;
                case KeyboardModifier.Shift: BeginComponentScale(DragMode.HorizontalScale, mousePosition); break;
                case KeyboardModifier.None: BeginComponentTranslate(DragMode.Translate, mousePosition); break;
            }
        }

        protected override void BeginDragHook(Vector2 mousePosition)
        {
            if (!CanDrag) return;
            Component.OperationRecorder.PushGroup();
        }

        protected override void UpdateDragHook(Vector2 mousePosition)
        {
            if (!CanDrag) return;
            switch (dragMode)
            {
                case DragMode.VerticalScale: DragVerticalScale(mousePosition); break;
                case DragMode.FreeScale: DragFreeScale(mousePosition); break;
                case DragMode.HorizontalScale: DragHorizontalScale(mousePosition); break;
                case DragMode.Translate: DragTranslate(mousePosition); break;
            }
        }

        protected override void EndDragHook(Vector2 mousePosition)
        {
            if (!CanDrag) return;
            switch (dragMode)
            {
                case DragMode.VerticalScale: EndComponentScale(); break;
                case DragMode.FreeScale: EndComponentScale(); break;
                case DragMode.HorizontalScale: EndComponentScale(); break;
                case DragMode.Translate: EndComponentTranslate(); break;
            }
            Component.OperationRecorder.PopGroup();
        }

        /// <summary>May be null.</summary>
        private BaseGuiComponentToken FirstSelectedComponent
        {
            get
            {
                Func<object, bool> componentFilter = x => x is BaseGuiComponentToken;
                object component = Selection.FirstOrDefault(componentFilter);
                return OxHelper.Cast<BaseGuiComponentToken>(component);
            }
        }

        private Selection Selection { get { return document.Selection; } }

        private enum DragMode
        {
            Translate = 0,
            HorizontalScale,
            VerticalScale,
            FreeScale
        }

        private bool CanDrag { get { return selected != null; } }

        private void BeginComponentScale(DragMode dragMode, Vector2 mousePosition)
        {
            BaseGuiComponentToken component = selected;
            dragOffset = mousePosition - component.Scale;
            Vector2 oldScale = component.Scale;
            undo = delegate { component.Scale = oldScale; };
            this.dragMode = dragMode;
        }

        private void BeginComponentTranslate(DragMode dragMode, Vector2 mousePosition)
        {
            BaseGuiComponentToken component = selected;
            dragOffset = mousePosition - component.Position;
            Vector2 oldPosition = component.Position;
            Guid? oldParentGuid = component.ParentGuid;
            undo = delegate { component.Position = oldPosition; component.ParentGuid = oldParentGuid; };
            this.dragMode = dragMode;
        }

        private void DragTranslate(Vector2 mousePosition)
        {
            selected.Position = (mousePosition - dragOffset).GetSnap(snap);
            if (selected.ParentGuid == null) return;
            BaseGuiComponentToken currentParent = document.Find<BaseGuiComponentToken>(selected.ParentGuid.Value);
            BaseGuiComponentToken newParent = Component.FindComponent(mousePosition, selected, true);

            if (currentParent == null ||
                newParent == null ||
                currentParent == newParent ||
                document.IsDescending(selected, newParent))
                return;

            dragOffset -= currentParent.Instance.PositionWorld;
            dragOffset += newParent.Instance.PositionWorld;
            selected.Position = (mousePosition - dragOffset).GetSnap(snap);
            selected.ParentGuid = newParent.Guid;
        }

        private void DragHorizontalScale(Vector2 mousePosition)
        {
            selected.Scale = new Vector2(
                (mousePosition.X - dragOffset.X).GetSnap(snap), selected.Scale.Y);
        }

        private void DragVerticalScale(Vector2 mousePosition)
        {
            selected.Scale = new Vector2(
                selected.Scale.X, (mousePosition.Y - dragOffset.Y).GetSnap(snap));
        }

        private void DragFreeScale(Vector2 mousePosition)
        {
            selected.Scale = (mousePosition - dragOffset).GetSnap(snap);
        }

        private void EndComponentScale()
        {
            BaseGuiComponentToken component = selected;
            Vector2 newScale = component.Scale;
            Action redo = delegate { component.Scale = newScale; };
            Component.OperationRecorder.Record("Change Scale", undo, redo);
            undo = null;
        }

        private void EndComponentTranslate()
        {
            BaseGuiComponentToken component = selected;
            Vector2 newPosition = component.Position;
            Guid? newParentGuid = component.ParentGuid;
            Action redo = delegate { component.Position = newPosition; component.ParentGuid = newParentGuid; };
            Component.OperationRecorder.Record("Change Position", undo, redo);
            undo = null;
        }

        private const double dragBeginDelay = 0.15;

        private readonly Document document;
        /// <summary>May be null.</summary>
        private BaseGuiComponentToken selected;
        private DragMode dragMode;
        private Vector2 dragOffset;
        /// <summary>May be null.</summary>
        private Action undo;
        private float snap = 10;
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;
using SysDrawing = System.Drawing;

namespace SceneEditorNamespace
{
    public partial class SceneEditorForm : Form
    {
        public SceneEditorForm(OxEngine engine, SceneEditorController controller)
        {
            OxHelper.ArgumentNullCheck(engine, controller);
            InitializeComponent();
            this.engine = engine;
            this.controller = controller;
            buttonLimitFPS.Checked = engine.Game.IsFixedTimeStep;
            wrapper = new SceneEditorFormWrapper(engine, controller, this, propertyGrid,
                buttonUndo, buttonRedo, comboBoxComponentType, openFileDialog, saveFileDialog,
                richTextBoxConsole, treeView, textBoxPositionSnap, textBoxScaleSnap,
                textBoxOrientationSnap, textBoxCreationDepth, canvas);
        }
        
        public PropertyGrid PropertyGrid { get { return propertyGrid; } }

        public SysDrawing.Rectangle PanelTransform
        {
            get { return new SysDrawing.Rectangle(canvas.PointToScreen(new SysDrawing.Point()), canvas.Size); }
        }
        public IntPtr PanelHandle
        {
            get { return canvas.IsHandleCreated ? canvas.Handle : IntPtr.Zero; }
        }
        
        private void menuItemExit_Click(object sender, EventArgs e) { wrapper.ActionExit(); }
        private void menuItemNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void menuItemSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void menuItemSaveAs_Click(object sender, EventArgs e) { wrapper.ActionSaveAs(); }
        private void menuItemOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void menuItemUndo_Click(object sender, EventArgs e) { wrapper.ActionUndo(); }
        private void menuItemRedo_Click(object sender, EventArgs e) { wrapper.ActionRedo(); }
        private void menuItemClone_Click(object sender, EventArgs e) { wrapper.ActionClone(); }
        private void menuItemDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void menuItemCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void menuItemPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void menuItemSelectAll_Click(object sender, EventArgs e) { wrapper.ActionSelectAll(); }
        private void menuItemSelectFamily_Click(object sender, EventArgs e) { wrapper.ActionSelectFamily(); }
        private void menuItemSelectFamilyAcross_Click(object sender, EventArgs e) { wrapper.ActionSelectFamilyAcross(); }
        private void menuItemSelectSiblings_Click(object sender, EventArgs e) { wrapper.ActionSelectSiblings(); }
        private void menuItemSelectSameType_Click(object sender, EventArgs e) { wrapper.ActionSelectSameType(); }
        private void menuItemSelectSameScript_Click(object sender, EventArgs e) { wrapper.ActionSelectSameScript(); }
        private void menuItemCreateComponent_Click(object sender, EventArgs e) { wrapper.ActionCreateComponent(); }
        private void menuItemCreateGroup_Click(object sender, EventArgs e) { wrapper.ActionCreateGroup(); }
        private void menuItemToggleFreezing_Click(object sender, EventArgs e) { wrapper.ActionToggleFreezing(); }
        private void menuItemFreeze_Click(object sender, EventArgs e) { wrapper.ActionFreeze(); }
        private void menuItemChangeType_Click(object sender, EventArgs e) { wrapper.ActionChangeType(); }
        private void menuItemAutoBox_Click(object sender, EventArgs e) { wrapper.ActionAutoBox(); }
        private void menuItemRename_Click(object sender, EventArgs e) { wrapper.ActionRename(); }
        private void menuItemCreateComponentInCanvas_Click(object sender, EventArgs e) { wrapper.ActionCreateComponentInCanvas(); }
        private void menuItemPasteInCanvas_Click(object sender, EventArgs e) { wrapper.ActionPasteInCanvas(); }
        private void buttonNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void buttonOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void buttonSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void buttonClone_Click(object sender, EventArgs e) { wrapper.ActionClone(); }
        private void buttonDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void buttonCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void buttonPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void buttonAutoBox_Click(object sender, EventArgs e) { wrapper.ActionAutoBox(); }
        private void buttonCreateComponent_Click(object sender, EventArgs e) { wrapper.ActionCreateComponent(); }
        private void buttonCreateGroup_Click(object sender, EventArgs e) { wrapper.ActionCreateGroup(); }
        private void buttonUndo_ButtonClick(object sender, EventArgs e) { wrapper.ActionUndo(); }
        private void buttonRedo_ButtonClick(object sender, EventArgs e) { wrapper.ActionRedo(); }
        private void buttonUndo_DropDownOpening(object sender, EventArgs e) { wrapper.ActionPopulateUndoDropDown(); }
        private void buttonRedo_DropDownOpening(object sender, EventArgs e) { wrapper.ActionPopulateRedoDropDown(); }
        private void buttonUndo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { wrapper.ActionUndo(new Guid(e.ClickedItem.Name)); }
        private void buttonRedo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { wrapper.ActionRedo(new Guid(e.ClickedItem.Name)); }
        private void buttonLimitFPS_Click(object sender, EventArgs e) { ActionLimitFPS(); }
        private void buttonF_Click(object sender, EventArgs e) { ActionLook(Direction.Forward); }
        private void buttonB_Click(object sender, EventArgs e) { ActionLook(Direction.Backward); }
        private void buttonU_Click(object sender, EventArgs e) { ActionLook(Direction.Up); }
        private void buttonD_Click(object sender, EventArgs e) { ActionLook(Direction.Down); }
        private void buttonL_Click(object sender, EventArgs e) { ActionLook(Direction.Left); }
        private void buttonR_Click(object sender, EventArgs e) { ActionLook(Direction.Right); }
        private void buttonSetCameraPosition_Click(object sender, EventArgs e) { ActionSetCameraPosition(); }
        private void buttonX_Click(object sender, EventArgs e) { ActionX(sender); }
        private void buttonY_Click(object sender, EventArgs e) { ActionY(sender); }
        private void buttonZ_Click(object sender, EventArgs e) { ActionZ(sender); }
        private void buttonV_Click(object sender, EventArgs e) { ActionV(sender); }
        private void buttonXY_Click(object sender, EventArgs e) { ActionXY(sender); }
        private void buttonYZ_Click(object sender, EventArgs e) { ActionYZ(sender); }
        private void buttonZX_Click(object sender, EventArgs e) { ActionZX(sender); }

        private void SceneEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !wrapper.ActionPromptSave();
            if (!e.Cancel) wrapper.Dispose();
        }

        private void cavas_Enter(object sender, EventArgs e)
        {
            controller.Focused = true;
        }

        private void canvas_Leave(object sender, EventArgs e)
        {
            controller.Focused = false;
        }

        private void textBoxPosition_TextChanged(object sender, EventArgs e)
        {
            float snap;
            if (float.TryParse(textBoxPositionSnap.Text, out snap))
                controller.PositionSnap = snap;
        }

        private void textBoxScale_TextChanged(object sender, EventArgs e)
        {
            float snap;
            if (float.TryParse(textBoxScaleSnap.Text, out snap))
                controller.ScaleSnap = snap;
        }

        private void textBoxOrientation_TextChanged(object sender, EventArgs e)
        {
            float snap;
            if (float.TryParse(textBoxOrientationSnap.Text, out snap))
                controller.OrientationSnap = snap;
        }

        private void textBoxCreationDepth_TextChanged(object sender, EventArgs e)
        {
            float creationDepth;
            if (float.TryParse(textBoxCreationDepth.Text, out creationDepth))
                controller.CreationDepth = creationDepth;
        }

        private void ActionLook(Direction direction)
        {
            controller.Look(direction);
        }

        private void ActionX(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.Axis = Axis.X;
        }

        private void ActionY(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.Axis = Axis.Y;
        }

        private void ActionZ(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.Axis = Axis.Z;
        }

        private void ActionV(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.Axis = Axis.V;
        }

        private void ActionXY(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.Axis = Axis.XY;
        }

        private void ActionYZ(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.Axis = Axis.YZ;
        }

        private void ActionZX(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.Axis = Axis.ZX;
        }

        public void ActionLimitFPS()
        {
            engine.Game.IsFixedTimeStep = buttonLimitFPS.Checked;
        }

        private void ActionSetCameraPosition()
        {
            try
            {
                Vector3Converter converter = new Vector3Converter();
                string cameraPositionString = textBoxCameraPosition.Text;
                Vector3 cameraPosition = (Vector3)converter.ConvertFrom(cameraPositionString);
                textBoxCameraPosition.Text = (string)converter.ConvertTo(cameraPosition, typeof(string));
                controller.CameraPosition = cameraPosition;
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, Text);
            }
        }

        private void UpdateToolStripButtonAxes(object sender)
        {
            buttonX.Checked = sender == buttonX;
            buttonY.Checked = sender == buttonY;
            buttonZ.Checked = sender == buttonZ;
            buttonV.Checked = sender == buttonV;
            buttonXY.Checked = sender == buttonXY;
            buttonYZ.Checked = sender == buttonYZ;
            buttonZX.Checked = sender == buttonZX;
        }

        private readonly SceneEditorFormWrapper wrapper;
        private readonly SceneEditorController controller;
        private readonly OxEngine engine;
    }
}

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ox.Editor;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Gui;
using Ox.Gui.Component;
using Ox.Gui.Event;
using Ox.Gui.QuickSpriteNamespace;
using Ox.Engine.CameraNamespace;

namespace GuiEditorNamespace
{
    public class GuiEditorController : RootedEditorController
    {
        public GuiEditorController(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            documentPanel = new Panel(engine, domainName, false);
            documentPanel.Position = OxConfiguration.VirtualScreen.Position;
            documentPanel.Scale = OxConfiguration.VirtualScreen.Scale;
            documentPanel.Color = new Color();
            AddGarbage(documentPanel);
            eventPanel = new Panel(engine, domainName, false);
            eventPanel.Position = OxConfiguration.VirtualScreen.Position;
            eventPanel.Scale = OxConfiguration.VirtualScreen.Scale;
            eventPanel.Color = new Color();
            eventPanel.MouseButtonAction += eventPanel_MouseButtonAction;
            AddGarbage(eventPanel);
            selectionVisualizer = new GuiSelectionVisualizer(engine, domainName, false, Document);
            AddGarbage(selectionVisualizer);
            componentDragger = new GuiComponentDragger(engine, this, Document);
            ActivateEventPanel();
        }

        public float Snap
        {
            get { return componentDragger.Snap; }
            set { componentDragger.Snap = value; }
        }

        public bool Focused
        {
            get { return _focused; }
            set
            {
                _focused = value;
                GuiSystem guiSystem = Engine.GetService<GuiSystem>();
                guiSystem.EventsEnabled = value;
            }
        }

        public bool Picked
        {
            get { return picked; }
            set { picked = value; }
        }

        public void ActivateDocumentPanel()
        {
            documentPanel.ClearChildren();
            BaseGuiComponentToken root = Root as BaseGuiComponentToken;
            if (root != null) documentPanel.AddChild(root.Instance);
            Engine.GetService<GuiSystem>().Screen = documentPanel;
        }

        public void ActivateEventPanel()
        {
            Engine.GetService<GuiSystem>().Screen = eventPanel;
        }

        /// <summary>
        /// Find a component at the mouse's position.
        /// May return null.
        /// </summary>
        /// <param name="mousePosition">The mouse position at which to search.</param>
        public BaseGuiComponentToken FindComponent(Vector2 mousePosition)
        {
            return FindComponent(mousePosition, null, false);
        }

        /// <summary>
        /// Find a component at the mouse's position.
        /// May return null.
        /// </summary>
        /// <param name="mousePosition">The mouse position at which to search.</param>
        /// <param name="excluded">The component that is to be overlooked. May be null.</param>
        /// <param name="excludeFrozen">Include finding frozen components.</param>
        /// <returns>The found component.</returns>
        public BaseGuiComponentToken FindComponent(Vector2 mousePosition, BaseGuiComponentToken excluded, bool includeFrozen)
        {
            BaseGuiComponentToken result = null;
            IList<BaseGuiComponentToken> components = new List<BaseGuiComponentToken>();
            Document.Collect(components);
            for (int i = components.Count - 1; i > -1; --i)
            {
                BaseGuiComponentToken component = components[i];
                Rect boundsWorld = component.Instance.BoundsWorld;
                if (component != excluded &&
                    (includeFrozen || !component.Frozen) &&
                    boundsWorld.Contains(mousePosition) &&
                    (result == null || component.Instance.ZWorld > result.Instance.ZWorld))
                    result = component;
            }

            return result;
        }

        protected new GuiDocument Document
        {
            get { return OxHelper.Cast<GuiDocument>(base.Document); }
        }

        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            BaseGuiComponentToken selected = Selection.FirstOrNull as BaseGuiComponentToken;
            if (selected != null && Focused) UpdateTransformByKeyboard(selected);
            documentPanel.Update(gameTime);
        }

        protected override void SaveDocumentHook() { }

        protected override void LoadDocumentHook() { }

        protected override void NewDocumentHook() { }

        protected override Document CreateDocumentHook()
        {
            return new GuiDocument(Engine);
        }

        protected override void SetUpComponentHook(ComponentToken component, ItemCreationStyle creationStyle)
        {
            if (creationStyle != ItemCreationStyle.Load &&
                creationStyle != ItemCreationStyle.External &&
                creationStyle != ItemCreationStyle.Clone &&
                creationStyle != ItemCreationStyle.Undelete &&
                creationStyle != ItemCreationStyle.Replacement)
            {
                BaseGuiComponentToken componentAsGui = component as BaseGuiComponentToken;
                if (componentAsGui != null) componentAsGui.Position = componentSpawnPosition;
            }
        }

        private void eventPanel_MouseButtonAction(
            BaseGuiComponent sender, InputType inputType, MouseButton button, Vector2 mousePosition)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (button == MouseButton.Left) HandleLeftMouseButton(inputType, mousePosition);
            else if (button == MouseButton.Right) HandleRightMouseButton(inputType, mousePosition);
        }

        private void HandleLeftMouseButton(InputType inputType, Vector2 mousePosition)
        {
            HandleLeftSelection(inputType, mousePosition);
            HandleComponentManipulation(inputType, mousePosition);
        }

        private void HandleRightMouseButton(InputType inputType, Vector2 mousePosition)
        {
            HandleRightSelection(inputType, mousePosition);
        }

        private void HandleLeftSelection(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickDown && picked)
            {
                LeftSetSelection(mousePosition);
            }
        }

        private void HandleRightSelection(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickDown && picked && Selection.Count <= 1)
            {
                RightSetSelection(mousePosition);
            }
        }

        private void HandleComponentManipulation(InputType inputType, Vector2 mousePosition)
        {
            if (inputType == InputType.ClickUp)
            {
                componentDragger.HandleButton(inputType, mousePosition);
            }
            else if (picked)
            {
                componentDragger.HandleButton(inputType, mousePosition);
            }
        }

        private void LeftSetSelection(Vector2 mousePosition)
        {
            BaseGuiComponentToken pickedComponent = FindComponent(mousePosition);
            if (pickedComponent == null) Selection.Clear();
            else
            {
                bool componentSelected = Selection.Contains(pickedComponent);
                if (!componentSelected) Selection.Set(pickedComponent);
            }
        }

        private void RightSetSelection(Vector2 mousePosition)
        {
            BaseGuiComponentToken pickedComponent = FindComponent(mousePosition);
            if (pickedComponent != null) Selection.Set(pickedComponent);
        }

        private void UpdateTransformByKeyboard(BaseGuiComponentToken selected)
        {
            KeyboardState keyboard = Engine.KeyboardState;
            if (keyboard.GetControlState()) UpdateScaleByKeyboard(selected);
            else UpdateTranslateByKeyboard(selected);
        }

        private void UpdateTranslateByKeyboard(BaseGuiComponentToken selected)
        {
            KeyboardState keyboard = Engine.KeyboardState;
            if (keyboard.IsKeyDown(Keys.Up)) selected.Position += new Vector2(0, -componentDragger.Snap);
            if (keyboard.IsKeyDown(Keys.Down)) selected.Position += new Vector2(0, componentDragger.Snap);
            if (keyboard.IsKeyDown(Keys.Left)) selected.Position += new Vector2(-componentDragger.Snap, 0);
            if (keyboard.IsKeyDown(Keys.Right)) selected.Position += new Vector2(componentDragger.Snap, 0);
        }

        private void UpdateScaleByKeyboard(BaseGuiComponentToken selected)
        {
            KeyboardState keyboard = Engine.KeyboardState;
            if (keyboard.IsKeyDown(Keys.Up)) selected.Scale += new Vector2(0, -componentDragger.Snap);
            if (keyboard.IsKeyDown(Keys.Down)) selected.Scale += new Vector2(0, componentDragger.Snap);
            if (keyboard.IsKeyDown(Keys.Left)) selected.Scale += new Vector2(-componentDragger.Snap, 0);
            if (keyboard.IsKeyDown(Keys.Right)) selected.Scale += new Vector2(componentDragger.Snap, 0);
        }

        private static readonly Vector2 componentSpawnPosition = new Vector2(10, 10);

        private readonly IList<BaseGuiComponent> cachedInstances = new List<BaseGuiComponent>();
        private readonly GuiSelectionVisualizer selectionVisualizer;
        private readonly GuiComponentDragger componentDragger;
        private Panel eventPanel;
        private Panel documentPanel;
        private bool picked;
        private bool _focused;
    }
}

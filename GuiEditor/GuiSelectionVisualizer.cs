using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;
using Ox.Gui;
using Ox.Gui.Component;
using Ox.Gui.QuickSpriteNamespace;
using Ox.Gui.SkinGroup;
using Ox.Gui.ViewElement;

namespace GuiEditorNamespace
{
    /// <summary>
    /// Draw the bounds of all the selected gui components in a document.
    /// </summary>
    public class GuiSelectionVisualizer : InterleavedComponent
    {
        /// <summary>
        /// Create a BoundingRectVisualizer.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GuiSelectionVisualizer(OxEngine engine, string domainName, bool ownedByDomain, GuiDocument document)
            : base(engine, domainName, ownedByDomain)
        {
            OxHelper.ArgumentNullCheck(document);
            this.document = document;
            borders.Add(CreateBorder());
        }

        /// <summary>
        /// The color by which to visualize the selection.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateColor();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GuiSkinGroups skinGroups = Engine.GetService<GuiSkinGroups>();
                BorderSkinGroup skinGroup = skinGroups.NormalBorder;
                borders.ForEach(x => skinGroup.RemoveBorder(x));
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override IList<QuickSprite> CollectQuickSpritesHook(IList<QuickSprite> result)
        {
            base.CollectQuickSpritesHook(result);

            CollectComponents(cachedComponents);
            {
                EnsureBorderCapacity(cachedComponents);
                ConfigureBorders(cachedComponents);
                CollectBorderSprites(cachedComponents, result);
            }
            cachedComponents.Clear();

            return result;
        }

        private Selection Selection { get { return document.Selection; } }

        private Border CreateBorder()
        {
            Border result = new Border();
            Engine.GetService<GuiSkinGroups>().NormalBorder.AddBorder(result);
            result.EffectColor = Color;
            result.Z = 1.0f;
            return result;
        }

        private void CollectComponents(IList<BaseGuiComponent> result)
        {
            foreach (BaseGuiComponentToken c in Selection.OfType<BaseGuiComponentToken>()) result.Add(c.Instance);
        }

        private void CollectBorderSprites(IList<BaseGuiComponent> components, IList<QuickSprite> result)
        {
            CollectQuickSprites(components, result);
        }

        private void ConfigureBorders(IList<BaseGuiComponent> components)
        {
            for (int i = 0; i < components.Count; ++i)
            {
                Border border = borders[i];
                BaseGuiComponent component = components[i];
                border.Position = component.PositionWorld;
                border.Scale = component.Scale;
            }
        }

        private void CollectQuickSprites(IList<BaseGuiComponent> components, IList<QuickSprite> result)
        {
            for (int i = 0; i < components.Count; ++i) borders[i].CollectQuickSprites(result);
        }

        private void UpdateColor()
        {
            for (int i = 0; i < borders.Count; ++i) borders[i].EffectColor = Color;
        }

        private void EnsureBorderCapacity(IList<BaseGuiComponent> components)
        {
            int requiredCapacity = components.Count;
            if (requiredCapacity > borders.Count) IncreaseBorderCapacity(requiredCapacity);
        }

        private void IncreaseBorderCapacity(int requiredCapacity)
        {
            while (borders.Count < requiredCapacity * 2) // MAGICVALUE
                borders.Add(CreateBorder());
        }

        private readonly IList<BaseGuiComponent> cachedComponents = new List<BaseGuiComponent>();
        private readonly IList<Border> borders = new List<Border>();
        private readonly GuiDocument document;
        private Color _color = Color.Red;
    }
}

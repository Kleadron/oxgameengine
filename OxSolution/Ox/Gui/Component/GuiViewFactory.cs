using System;
using Ox.Engine;

namespace Ox.Gui.Component
{
    /// <summary>
    /// Creates views for gui components.
    /// </summary>
    public class GuiViewFactory
    {
        /// <summary>
        /// Create a GuiViewFactory.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GuiViewFactory(OxEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Create a view of type T.
        /// </summary>
        public GuiView Create(BaseGuiComponent parent, Type type)
        {
            OxHelper.ArgumentNullCheck(parent);
            return CreateHook(type, parent);
        }

        /// <summary>
        /// Handle creating a view of the specified type.
        /// </summary>
        protected virtual GuiView CreateHook(Type type, BaseGuiComponent parent)
        {
            if (type == typeof(ButtonView)) return new ButtonView(engine, parent);
            if (type == typeof(CheckBoxView)) return new CheckBoxView(engine, parent);
            if (type == typeof(DialogView)) return new DialogView(engine, parent);
            if (type == typeof(FillBarView)) return new FillBarView(engine, parent);
            if (type == typeof(LabelView)) return new LabelView(engine, parent);
            if (type == typeof(PanelView)) return new PanelView(engine, parent);
            if (type == typeof(RadioButtonView)) return new RadioButtonView(engine, parent);
            if (type == typeof(TextBoxView)) return new TextBoxView(engine, parent);
            if (type == typeof(GuiView)) return new GuiView(engine, parent);
            return null;
        }

        private readonly OxEngine engine;
    }
}

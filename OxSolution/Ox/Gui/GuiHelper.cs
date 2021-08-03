using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Gui.Component;

namespace Ox.Gui
{
    /// <summary>
    /// Helper methods for various gui tasks.
    /// </summary>
    public static class GuiHelper
    {
        /// <summary>
        /// Calculate the operating system's mouse position.
        /// </summary>
        public static Vector2 OSMouseToAppMouse(Point mousePosition, Point screenResolution)
        {
            return new Vector2(
                OxConfiguration.VirtualScreen.Width / screenResolution.X * mousePosition.X,
                OxConfiguration.VirtualScreen.Height / screenResolution.Y * mousePosition.Y);
        }

        /// <summary>
        /// Calculate the mouse's position relative to the game window.
        /// </summary>
        public static Point AppMouseToOSMouse(Vector2 mousePosition, Point screenResolution)
        {
            // the actual OS's mouse position is integer-based, so we chop off the decimal
            return new Point(
                (int)((float)screenResolution.X / OxConfiguration.VirtualScreen.Width * mousePosition.X),
                (int)((float)screenResolution.Y / OxConfiguration.VirtualScreen.Height * mousePosition.Y));
        }

        /// <summary>
        /// Align gui components.
        /// </summary>
        /// <typeparam name="T">The type of gui components to align.</typeparam>
        /// <param name="components">The gui components to align.</param>
        /// <param name="position">The position of the first gui component.</param>
        /// <param name="scale">The scale of the area within which to place the gui components.</param>
        /// <param name="columns">The number of columns to use.</param>
        /// <param name="rows">The number of rows to use.</param>
        /// <param name="fillHorizontally">
        /// True if we fill from left to right. False if we fill from top to bottom.</param>
        public static void Align<T>(
            this IList<T> components, Vector2 position, Vector2 scale, int columns, int rows, bool fillHorizontally)
            where T : BaseGuiComponent
        {
            OxHelper.ArgumentNullCheck(components);
            int column = 0;
            int row = 0;
            for (int i = 0; i < components.Count; i++)
            {
                BaseGuiComponent component = components[i];
                Vector2 gridScale = new Vector2(scale.X / columns, scale.Y / rows);
                Vector2 positionOffset = gridScale * new Vector2(column, row);
                if (fillHorizontally)
                {
                    ++row;
                    if (row >= rows)
                    {
                        ++column;
                        row = 0;
                    }
                }
                else
                {
                    ++column;
                    if (column >= columns)
                    {
                        ++row;
                        column = 0;
                    }
                }
                component.Scale = gridScale;
                component.Position += position + positionOffset;
            }
        }
    }
}

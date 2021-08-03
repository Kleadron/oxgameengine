using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.Font;
using Ox.Gui.SkinGroup;

namespace Ox.Gui.Component
{
    /// <summary>
    /// All the skin groups needed to skin the gui system.
    /// </summary>
    public class GuiSkinGroups : Disposable
    {
        /// <summary>
        /// Create a GuiSkinGroups object.
        /// </summary>
        public GuiSkinGroups()
        {
            textBorder = new BorderSkinGroup();
            raisedBorder = new BorderSkinGroup();
            raisedFocusedBorder = new BorderSkinGroup();
            sunkenBorder = new BorderSkinGroup();
            sunkenFocusedBorder = new BorderSkinGroup();
            normalBorder = new BorderSkinGroup();
            normalFocusedBorder = new BorderSkinGroup();
            litSurface = new ImageSkinGroup();
            raisedSurface = new ImageSkinGroup();
            sunkenSurface = new ImageSkinGroup();
            normalSurface = new ImageSkinGroup();
            checkOff = new ImageSkinGroup();
            checkOffFocused = new ImageSkinGroup();
            checkOn = new ImageSkinGroup();
            checkOnFocused = new ImageSkinGroup();
            radioOff = new ImageSkinGroup();
            radioOffFocused = new ImageSkinGroup();
            radioOn = new ImageSkinGroup();
            radioOnFocused = new ImageSkinGroup();
            filling = new ImageSkinGroup();
            selectionCursor = new ImageSkinGroup();

            IFont normalFont = new MonospaceFont(
                "Courier New Bold",
                new Point(256, 256),
                "Ox/Gui/Fonts/courier_new_bold-0",
                new Vector2(13, 24));

            float inset = 1.0f / 128.0f;
            normalText = new TextSkinGroup(normalFont);
            normalText.FontScale = new Vector2(16, 32);
            normalText.Inset = inset;

            bar = new ImageSkinGroup();
            buttonUp = new ImageSkinGroup();
            buttonDown = new ImageSkinGroup();
            buttonLeft = new ImageSkinGroup();
            buttonRight = new ImageSkinGroup();
            raisedX = new ImageSkinGroup();
            sunkenX = new ImageSkinGroup();
            normalTextCursor = new ImageSkinGroup();

            normalMouse = new ImageSkinGroup();
            waitMouse = new ImageSkinGroup();

            float step = 1.0f / 8.0f;
            Vector2 tileSize = new Vector2(step);
            float borderStep = step * 9.0f / 32.0f;
            Vector2 borderTileSize = new Vector2(borderStep);

            raisedBorder.Decorate(new Vector2(0, 0), borderTileSize, "Ox/Gui/Textures/textures");
            raisedFocusedBorder.Decorate(new Vector2(step, 0), borderTileSize, "Ox/Gui/Textures/textures");
            sunkenBorder.Decorate(new Vector2(step * 2, 0), borderTileSize, "Ox/Gui/Textures/textures");
            sunkenFocusedBorder.Decorate(new Vector2(step * 3, 0), borderTileSize, "Ox/Gui/Textures/textures");
            normalBorder.Decorate(new Vector2(step * 4, 0), borderTileSize, "Ox/Gui/Textures/textures");
            normalFocusedBorder.Decorate(new Vector2(step * 5, 0), borderTileSize, "Ox/Gui/Textures/textures");
            textBorder.Decorate(new Vector2(step * 6, 0), borderTileSize, "Ox/Gui/Textures/textures");

            litSurface.TextureFileName = "Ox/Gui/Textures/textures";
            litSurface.SourcePosition = new Vector2(0, step);
            litSurface.SourceSize = tileSize;
            litSurface.Inset = inset;
            raisedSurface.TextureFileName = "Ox/Gui/Textures/textures";
            raisedSurface.SourcePosition = new Vector2(step, step);
            raisedSurface.SourceSize = tileSize;
            raisedSurface.Inset = inset;
            sunkenSurface.TextureFileName = "Ox/Gui/Textures/textures";
            sunkenSurface.SourcePosition = new Vector2(step * 2, step);
            sunkenSurface.SourceSize = tileSize;
            sunkenSurface.Inset = inset;
            normalSurface.TextureFileName = "Ox/Gui/Textures/textures";
            normalSurface.SourcePosition = new Vector2(step * 3, step);
            normalSurface.SourceSize = tileSize;
            normalSurface.Inset = inset;

            raisedX.TextureFileName = "Ox/Gui/Textures/textures";
            raisedX.SourcePosition = new Vector2(0, step * 2);
            raisedX.SourceSize = tileSize;
            raisedX.Inset = inset;
            sunkenX.TextureFileName = "Ox/Gui/Textures/textures";
            sunkenX.SourcePosition = new Vector2(step, step * 2);
            sunkenX.SourceSize = tileSize;
            sunkenX.Inset = inset;

            radioOff.TextureFileName = "Ox/Gui/Textures/textures";
            radioOff.SourcePosition = new Vector2(0, step * 3);
            radioOff.SourceSize = tileSize;
            radioOff.Inset = inset;
            radioOffFocused.TextureFileName = "Ox/Gui/Textures/textures";
            radioOffFocused.SourcePosition = new Vector2(step, step * 3);
            radioOffFocused.SourceSize = tileSize;
            radioOffFocused.Inset = inset;
            radioOn.TextureFileName = "Ox/Gui/Textures/textures";
            radioOn.SourcePosition = new Vector2(step * 2, step * 3);
            radioOn.SourceSize = tileSize;
            radioOn.Inset = inset;
            radioOnFocused.TextureFileName = "Ox/Gui/Textures/textures";
            radioOnFocused.SourcePosition = new Vector2(step * 3, step * 3);
            radioOnFocused.SourceSize = tileSize;
            radioOnFocused.Inset = inset;

            checkOff.TextureFileName = "Ox/Gui/Textures/textures";
            checkOff.SourcePosition = new Vector2(0, step * 4);
            checkOff.SourceSize = tileSize;
            checkOff.Inset = inset;
            checkOffFocused.TextureFileName = "Ox/Gui/Textures/textures";
            checkOffFocused.SourcePosition = new Vector2(step, step * 4);
            checkOffFocused.SourceSize = tileSize;
            checkOffFocused.Inset = inset;
            checkOn.TextureFileName = "Ox/Gui/Textures/textures";
            checkOn.SourcePosition = new Vector2(step * 2, step * 4);
            checkOn.SourceSize = tileSize;
            checkOn.Inset = inset;
            checkOnFocused.TextureFileName = "Ox/Gui/Textures/textures";
            checkOnFocused.SourcePosition = new Vector2(step * 3, step * 4);
            checkOnFocused.SourceSize = tileSize;
            checkOnFocused.Inset = inset;

            buttonUp.TextureFileName = "Ox/Gui/Textures/textures";
            buttonUp.SourcePosition = new Vector2(0, step * 5);
            buttonUp.SourceSize = tileSize;
            buttonUp.Inset = inset;
            buttonDown.TextureFileName = "Ox/Gui/Textures/textures";
            buttonDown.SourcePosition = new Vector2(step, step * 5);
            buttonDown.SourceSize = tileSize;
            buttonDown.Inset = inset;
            buttonLeft.TextureFileName = "Ox/Gui/Textures/textures";
            buttonLeft.SourcePosition = new Vector2(step * 2, step * 5);
            buttonLeft.SourceSize = tileSize;
            buttonLeft.Inset = inset;
            buttonRight.TextureFileName = "Ox/Gui/Textures/textures";
            buttonRight.SourcePosition = new Vector2(step * 3, step * 5);
            buttonRight.SourceSize = tileSize;
            buttonRight.Inset = inset;

            bar.TextureFileName = "Ox/Gui/Textures/textures";
            bar.SourcePosition = new Vector2(0, step * 6);
            bar.SourceSize = tileSize;
            bar.Inset = inset;
            filling.TextureFileName = "Ox/Gui/Textures/textures";
            filling.SourcePosition = new Vector2(step, step * 6);
            filling.SourceSize = tileSize;
            filling.Inset = inset;
            selectionCursor.TextureFileName = "Ox/Gui/Textures/textures";
            selectionCursor.SourcePosition = new Vector2(step * 2, step * 6);
            selectionCursor.SourceSize = tileSize;
            selectionCursor.Inset = inset;

            normalTextCursor.TextureFileName = "Ox/Gui/TextCursors/normalTextCursor";

            normalMouse.TextureFileName = "Ox/Gui/Mice/normalMouse";
            waitMouse.TextureFileName = "Ox/Gui/Mice/waitMouse";
        }

        public BorderSkinGroup TextBorder { get { return textBorder; } }
        public BorderSkinGroup RaisedBorder { get { return raisedBorder; } }
        public BorderSkinGroup RaisedFocusedBorder { get { return raisedFocusedBorder; } }
        public BorderSkinGroup SunkenBorder { get { return sunkenBorder; } }
        public BorderSkinGroup SunkenFocusedBorder { get { return sunkenFocusedBorder; } }
        public BorderSkinGroup NormalBorder { get { return normalBorder; } }
        public BorderSkinGroup NormalFocusedBorder { get { return normalFocusedBorder; } }
        public ImageSkinGroup LitSurface { get { return litSurface; } }
        public ImageSkinGroup RaisedSurface { get { return raisedSurface; } }
        public ImageSkinGroup SunkenSurface { get { return sunkenSurface; } }
        public ImageSkinGroup NormalSurface { get { return normalSurface; } }
        public ImageSkinGroup Unchecked { get { return checkOff; } }
        public ImageSkinGroup UncheckedFocused { get { return checkOffFocused; } }
        public ImageSkinGroup Checked { get { return checkOn; } }
        public ImageSkinGroup CheckedFocused { get { return checkOnFocused; } }
        public ImageSkinGroup RadioOff { get { return radioOff; } }
        public ImageSkinGroup RadioOffFocused { get { return radioOffFocused; } }
        public ImageSkinGroup RadioOn { get { return radioOn; } }
        public ImageSkinGroup RadioOnFocused { get { return radioOnFocused; } }
        public ImageSkinGroup Filling { get { return filling; } }
        public ImageSkinGroup SelectionCursor { get { return selectionCursor; } }
        public TextSkinGroup NormalText { get { return normalText; } }
        public ImageSkinGroup NormalTextCursor { get { return normalTextCursor; } }
        public ImageSkinGroup Bar { get { return bar; } }
        public ImageSkinGroup ButtonUp { get { return buttonUp; } }
        public ImageSkinGroup ButtonDown { get { return buttonDown; } }
        public ImageSkinGroup ButtonLeft { get { return buttonLeft; } }
        public ImageSkinGroup ButtonRight { get { return buttonRight; } }
        public ImageSkinGroup NormalMouse { get { return normalMouse; } }
        public ImageSkinGroup WaitMouse { get { return waitMouse; } }
        public ImageSkinGroup RaisedX { get { return raisedX; } }
        public ImageSkinGroup SunkenX { get { return sunkenX; } }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) normalText.Font.Dispose();
            base.Dispose(disposing);
        }

        private BorderSkinGroup textBorder;
        private BorderSkinGroup raisedBorder;
        private BorderSkinGroup raisedFocusedBorder;
        private BorderSkinGroup sunkenBorder;
        private BorderSkinGroup sunkenFocusedBorder;
        private BorderSkinGroup normalBorder;
        private BorderSkinGroup normalFocusedBorder;
        private ImageSkinGroup litSurface;
        private ImageSkinGroup raisedSurface;
        private ImageSkinGroup sunkenSurface;
        private ImageSkinGroup normalSurface;
        private ImageSkinGroup checkOff;
        private ImageSkinGroup checkOffFocused;
        private ImageSkinGroup checkOn;
        private ImageSkinGroup checkOnFocused;
        private ImageSkinGroup radioOff;
        private ImageSkinGroup radioOffFocused;
        private ImageSkinGroup radioOn;
        private ImageSkinGroup radioOnFocused;
        private ImageSkinGroup filling;
        private TextSkinGroup normalText;
        private ImageSkinGroup normalTextCursor;
        private ImageSkinGroup selectionCursor;
        private ImageSkinGroup bar;
        private ImageSkinGroup buttonUp;
        private ImageSkinGroup buttonDown;
        private ImageSkinGroup buttonLeft;
        private ImageSkinGroup buttonRight;
        private ImageSkinGroup normalMouse;
        private ImageSkinGroup waitMouse;
        private ImageSkinGroup raisedX;
        private ImageSkinGroup sunkenX;
    }
}

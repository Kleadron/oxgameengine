using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical keyboard for a user interface.
    /// </summary>
    public class VirtualKeyboard : Dialog
    {
        /// <summary>
        /// Create a VirtualKeyboard.
        /// </summary>
        public VirtualKeyboard(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            CreateKeys(domainName);
            AlignKeys();
            DrawNumbers();
            KeysToUpper();
            DrawExtras();
            Modal = true;
            Scale = scale;
            Text = "Virtual Keyboard 2.0";
            CloseClicked += this_CloseClicked;
            GuiAction += this_GuiAction;
        }

        /// <summary>
        /// Is the keyboard taking input?
        /// </summary>
        public bool Executing { get { return textBox != null; } }

        /// <summary>
        /// Execute the keyboard to start taking user input.
        /// </summary>
        public void Execute(TextBox textBox)
        {
            OxHelper.ArgumentNullCheck(textBox);
            if (Executing) return;
            this.textBox = textBox;
            grid1Buttons[0].FocusByOtherInput();
            textBox.CursorVisible = true;
        }

        /// <summary>
        /// Terminate the keyboard to stop taking user input.
        /// </summary>
        public void Terminate()
        {
            if (!Executing) return;
            textBox.CursorVisible = false;
            textBox.FocusByOtherInput();
            textBox = null;
        }

        private void this_CloseClicked(Dialog sender)
        {
            OxHelper.ArgumentNullCheck(sender);
            GuiSystem guiSystem = Engine.GetService<GuiSystem>();
            guiSystem.HideVirtualKeyboard();
        }

        private void this_GuiAction(BaseGuiComponent sender, GuiEventType type)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (type == GuiEventType.ColorChanged) CascadeColor();
        }

        private void keys_Clicked(BaseGuiComponent sender)
        {
            OxHelper.ArgumentNullCheck(sender);
            Button buttonSender = sender as Button;
            if (textBox == null || buttonSender == null) return;
            if (grid1Buttons.Contains(buttonSender)) textBox.TextEntry(buttonSender.Text[0]);
            else if (grid2Buttons.Contains(buttonSender))
            {
                switch (buttonSender.Text)
                {
                    case abcLower: KeysToLower(); break;
                    case abcUpper: KeysToUpper(); break;
                    case symbols: KeysToSymbols(); break;
                    case backspace: textBox.Backspace(); break;
                    case delete: textBox.Delete(); break;
                    case space: textBox.TextEntry(' '); break;
                    case home: textBox.Home(); break;
                    case end: textBox.End(); break;
                    case left: --textBox.CurrentCharacter; break;
                    case right: ++textBox.CurrentCharacter; break;
                }
            }
        }

        private void CascadeColor()
        {
            IList<BaseGuiComponent> children = CollectChildren(CollectionAlgorithm.Descending, new List<BaseGuiComponent>()); // MEMORYCHURN
            for (int i = 0; i < children.Count; ++i) children[i].Color = Color;
        }

        private void CreateKeys(string domainName)
        {
            CreateKeyGrid(domainName, grid1Buttons, grid1Dims);
            CreateKeyGrid(domainName, grid2Buttons, grid2Dims);
        }

        private Point CreateKeyGrid(string domainName, IList<Button> gridButtons, Point gridDims)
        {
            for (int i = 0; i < gridDims.X; ++i)
            {
                for (int j = 0; j < gridDims.Y; ++j)
                {
                    Button button = new Button(Engine, domainName, false);
                    button.Clicked += keys_Clicked;
                    gridButtons.Add(button);
                    AddGarbage(button);
                    AddChild(button);
                }
            }
            return gridDims;
        }

        private void AlignKeys()
        {
            grid1Buttons.Align(grid1Position, grid1Scale, grid1Dims.X, grid1Dims.Y, false);
            grid2Buttons.Align(grid2Position, grid2Scale, grid2Dims.X, grid2Dims.Y, false);
        }

        private void DrawNumbers()
        {
            int c = 48;
            for (int i = 0; i < 10; ++i) grid1Buttons[i].Text = ((char)c++).ToString();
        }

        private void DrawExtras()
        {
            grid2Buttons[0].Text = abcLower;
            grid2Buttons[1].Text = abcUpper;
            grid2Buttons[2].Text = symbols;
            grid2Buttons[3].Text = backspace;
            grid2Buttons[4].Text = delete;
            grid2Buttons[5].Text = space;
            grid2Buttons[6].Text = home;
            grid2Buttons[7].Text = end;
            grid2Buttons[9].Text = left;
            grid2Buttons[10].Text = right;
        }

        private void KeysToUpper()
        {
            int c = 65;
            for (int i = 10; i < 36; ++i) grid1Buttons[i].Text = ((char)c++).ToString();
            LastFourKeysToFrequentlyUsedSymbols();
        }

        private void KeysToLower()
        {
            int c = 97;
            for (int i = 10; i < 36; ++i) grid1Buttons[i].Text = ((char)c++).ToString();
            LastFourKeysToFrequentlyUsedSymbols();
        }

        private void KeysToSymbols()
        {
            int c = 33;
            for (int i = 10; i < 40; ++i)
            {
                if (c == 48) c = 58;
                if (c == 63) c = 91;
                if (c == 97) c = 123;
                grid1Buttons[i].Text = ((char)c).ToString();
                ++c;
            }
        }

        private void LastFourKeysToFrequentlyUsedSymbols()
        {
            grid1Buttons[36].Text = period;
            grid1Buttons[37].Text = questionMark;
            grid1Buttons[38].Text = underscore;
            grid1Buttons[39].Text = asperand;
        }

        private const string abcLower = "abc";
        private const string abcUpper = "ABC";
        private const string symbols = "!#$";
        private const string backspace = "Bk Sp";
        private const string delete = "Del";
        private const string space = "Space";
        private const string home = "Home";
        private const string end = "End";
        private const string left = "<";
        private const string right = ">";
        private const string period = ".";
        private const string questionMark = "?";
        private const string underscore = "_";
        private const string asperand = "@";

        private readonly IList<Button> grid1Buttons = new List<Button>();
        private readonly IList<Button> grid2Buttons = new List<Button>();
        private readonly Vector2 grid1Position = new Vector2(16, 56);
        private readonly Vector2 grid2Position = new Vector2(628, 56);
        private readonly Vector2 grid1Scale = new Vector2(600, 192);
        private readonly Vector2 grid2Scale = new Vector2(360, 192);
        private readonly Vector2 scale = new Vector2(1004, 264);
        private readonly Point grid1Dims = new Point(10, 4);
        private readonly Point grid2Dims = new Point(3, 4);
        /// <summary>May be null.</summary>
        private TextBox textBox;
    }
}

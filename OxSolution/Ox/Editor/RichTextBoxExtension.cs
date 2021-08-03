using System.Windows.Forms;

namespace Ox.Editor
{
    public static class RichTextBoxExtension
    {
        public static void WriteLine(this RichTextBox richTextBox, string line)
        {
            string[] lines = new string[richTextBox.Lines.Length + 1];
            richTextBox.Lines.CopyTo(lines, 0);
            lines[lines.Length - 1] = line;
            richTextBox.Lines = lines;
            richTextBox.ScrollToEnd();
        }

        public static void ScrollToEnd(this RichTextBox richTextBox)
        {
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.ScrollToCaret();
        }
    }
}

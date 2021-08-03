using System;
using System.Windows.Forms;

namespace GuiEditorNamespace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the game.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (GuiEditor game = new GuiEditor()) game.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

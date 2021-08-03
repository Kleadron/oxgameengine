using System;
using System.Windows.Forms;

namespace GeneralEditorNamespace
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
                using (GeneralEditor game = new GeneralEditor()) game.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

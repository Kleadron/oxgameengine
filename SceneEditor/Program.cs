using System;
using System.Windows.Forms;

namespace SceneEditorNamespace
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
                using (SceneEditor game = new SceneEditor()) game.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

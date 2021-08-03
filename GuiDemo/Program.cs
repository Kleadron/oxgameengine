using System;

namespace GuiDemoNamespace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the game.
        /// </summary>
        static void Main(string[] args)
        {
            using (GuiDemo game = new GuiDemo()) game.Run();
        }
    }
}

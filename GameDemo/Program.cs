using System;

namespace GameDemoNamespace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the game.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameDemo game = new GameDemo()) game.Run();
        }
    }
}

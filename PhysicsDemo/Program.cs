using System;

namespace PhysicsDemoNamespace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the game.
        /// </summary>
        static void Main(string[] args)
        {
            using (PhysicsDemo game = new PhysicsDemo()) game.Run();
        }
    }
}

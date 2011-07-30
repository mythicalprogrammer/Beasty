using System;

namespace Beasty
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Core game = new Core())
            {
                game.Run();
            }
        }
    }
}


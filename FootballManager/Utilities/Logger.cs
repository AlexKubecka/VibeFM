using System;

namespace FootballManager.Utilities
{
    public static class Logger
    {
        public static bool DebugMode { get; set; } = false; // Default: Debug mode disabled

        public static void Log(string message)
        {
            if (DebugMode)
            {
                Console.WriteLine(message);
            }
        }
    }
}
using System;
using System.IO;

namespace Unity.VisualScripting
{
    public static class DebugUtility
    {
        public static string logPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Ludiq.log");

        public static void LogToFile(string message)
        {
            File.AppendAllText(logPath, message + Environment.NewLine);
        }
    }
}

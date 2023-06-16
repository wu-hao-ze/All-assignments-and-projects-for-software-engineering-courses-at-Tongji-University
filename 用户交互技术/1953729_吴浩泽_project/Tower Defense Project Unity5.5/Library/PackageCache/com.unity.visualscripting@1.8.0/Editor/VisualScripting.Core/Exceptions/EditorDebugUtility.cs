using System.IO;
using UnityEditor;

namespace Unity.VisualScripting
{
    public static class EditorDebugUtility
    {
        internal static void DeleteDebugLogFile()
        {
            if (File.Exists(DebugUtility.logPath))
            {
                File.Delete(DebugUtility.logPath);
            }
        }
    }
}

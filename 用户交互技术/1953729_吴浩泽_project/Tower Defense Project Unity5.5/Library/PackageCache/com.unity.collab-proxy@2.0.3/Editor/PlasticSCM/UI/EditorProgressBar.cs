using System.Linq;
using System.Reflection;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class EditorProgressBar
    {
        static EditorProgressBar()
        {
            var type = typeof(UnityEditor.Editor).Assembly.GetTypes().Where(
                t => t.Name == "AsyncProgressBar").FirstOrDefault();

            if (type == null)
                return;

            mDisplayMethod = type.GetMethod("Display");
            mClearMethod = type.GetMethod("Clear");
        }

        internal static void ShowProgressBar(string text, float progress)
        {
            if (mDisplayMethod == null)
                return;

            mDisplayMethod.Invoke(null, new object[] { text, progress });
        }

        internal static void ClearProgressBar()
        {
            if (mClearMethod == null)
                return;

            mClearMethod.Invoke(null, null);
        }

        static MethodInfo mDisplayMethod = null;
        static MethodInfo mClearMethod = null;
    }
}
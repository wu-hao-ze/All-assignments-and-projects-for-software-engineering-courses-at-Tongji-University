using System;

using UnityEditor;
using UnityEditorInternal;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class EditorWindowFocus
    {
        internal static event Action OnApplicationActivated;
        internal static event Action OnApplicationDeactivated;

        static EditorWindowFocus()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            bool isApplicationActive = InternalEditorUtility.isApplicationActive;

            if (!mLastIsApplicationFocused && isApplicationActive)
            {
                mLastIsApplicationFocused = isApplicationActive;

                if (OnApplicationActivated != null)
                    OnApplicationActivated();

                return;
            }

            if (mLastIsApplicationFocused && !isApplicationActive)
            {
                mLastIsApplicationFocused = isApplicationActive;

                if (OnApplicationDeactivated != null)
                    OnApplicationDeactivated();

                return;
            }
        }

        static bool mLastIsApplicationFocused;
    }
}

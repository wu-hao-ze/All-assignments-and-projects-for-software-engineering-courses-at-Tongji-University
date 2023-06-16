using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.PlasticSCM.Editor.UI.UIElements
{
    internal class LoadingSpinner : VisualElement
    {
        internal LoadingSpinner()
        {
            mStarted = false;

            // add child elements to set up centered spinner rotation
            mSpinner = new VisualElement();
            Add(mSpinner);

            mSpinner.style.backgroundImage = Images.GetImage(Images.Name.Loading);
            mSpinner.style.position = Position.Absolute;
            mSpinner.style.width = 16;
            mSpinner.style.height = 16;
            mSpinner.style.left = -8;
            mSpinner.style.top = -8;

            style.position = Position.Relative;
            style.width = 16;
            style.height = 16;
            style.left = 8;
            style.top = 8;
        }

        internal void Dispose()
        {
            if (mStarted)
                EditorApplication.update -= UpdateProgress;
        }

        internal void Start()
        {
            if (mStarted)
                return;

            mRotation = 0;
            mLastRotationTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += UpdateProgress;

            mStarted = true;
        }

        internal void Stop()
        {
            if (!mStarted)
                return;

            EditorApplication.update -= UpdateProgress;

            mStarted = false;
        }

        void UpdateProgress()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            double deltaTime = currentTime - mLastRotationTime;

#if UNITY_2021_2_OR_NEWER
            mSpinner.transform.rotation = Quaternion.Euler(0, 0, mRotation);
#else
            transform.rotation = Quaternion.Euler(0, 0, mRotation);
#endif

            mRotation += (int)(ROTATION_SPEED * deltaTime);
            mRotation = mRotation % 360;
            if (mRotation < 0) mRotation += 360;

            mLastRotationTime = currentTime;
        }

        int mRotation;
        double mLastRotationTime;
        bool mStarted;
        VisualElement mSpinner;

        const int ROTATION_SPEED = 360; // Euler degrees per second
    }
}

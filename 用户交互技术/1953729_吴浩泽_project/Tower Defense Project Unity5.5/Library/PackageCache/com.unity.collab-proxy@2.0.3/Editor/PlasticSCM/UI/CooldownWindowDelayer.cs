using System;

using UnityEditor;

namespace Unity.PlasticSCM.Editor.UI
{
    public class CooldownWindowDelayer
    {
        internal static bool IsUnitTesting { get; set; }

        public CooldownWindowDelayer(Action action, double cooldownSeconds)
        {
            mAction = action;
            mCooldownSeconds = cooldownSeconds;
        }

        public void Ping()
        {
            if (IsUnitTesting)
            {
                mAction();
                return;
            }
            
            if (mIsOnCooldown)
            {
                RefreshCooldown();
                return;
            }

            StartCooldown();
        }

        void RefreshCooldown()
        {
            mIsOnCooldown = true;

            mSecondsOnCooldown = mCooldownSeconds;
        }

        void StartCooldown()
        {
            mLastUpdateTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += OnUpdate;

            RefreshCooldown();
        }

        void EndCooldown()
        {
            EditorApplication.update -= OnUpdate;

            mIsOnCooldown = false;

            mAction();
        }

        void OnUpdate()
        {
            double updateTime = EditorApplication.timeSinceStartup;
            double deltaSeconds = updateTime - mLastUpdateTime;

            mSecondsOnCooldown -= deltaSeconds;

            if (mSecondsOnCooldown < 0)
                EndCooldown();

            mLastUpdateTime = updateTime;
        }

        readonly Action mAction;
        readonly double mCooldownSeconds;

        double mLastUpdateTime;
        bool mIsOnCooldown;
        double mSecondsOnCooldown;
    }
}

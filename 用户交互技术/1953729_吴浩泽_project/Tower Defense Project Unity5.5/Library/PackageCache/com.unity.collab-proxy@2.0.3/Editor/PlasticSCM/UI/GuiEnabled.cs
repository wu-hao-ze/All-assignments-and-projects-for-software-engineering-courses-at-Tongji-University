using System;

using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class GuiEnabled : IDisposable
    {
        internal GuiEnabled(bool enabled)
        {
            mEnabled = GUI.enabled;
            GUI.enabled = enabled && mEnabled;
        }

        void IDisposable.Dispose()
        {
            GUI.enabled = mEnabled;
        }

        bool mEnabled;
    }
}

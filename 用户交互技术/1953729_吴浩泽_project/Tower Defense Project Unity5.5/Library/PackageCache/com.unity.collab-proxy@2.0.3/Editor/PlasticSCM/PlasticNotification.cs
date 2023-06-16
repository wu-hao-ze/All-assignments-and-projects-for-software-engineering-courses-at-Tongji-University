using UnityEngine;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal static class PlasticNotification
    {
        internal enum Status
        {
            None,
            IncomingChanges,
            Conflicts
        }

        internal static Texture GetIcon(Status status)
        {
            if (status == Status.IncomingChanges)
                return Images.GePlasticNotifyIncomingIcon();

            if (status == Status.Conflicts)
                return Images.GetPlasticNotifyConflictIcon();

            return Images.GetPlasticViewIcon();
        }
    }
}

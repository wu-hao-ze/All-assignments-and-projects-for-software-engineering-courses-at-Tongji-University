using System;

namespace Unity.PlasticSCM.Editor.AssetsOverlays
{
    [Flags]
    internal enum AssetStatus
    {
        None            = 0,
        Private         = 1 << 0,
        Ignored         = 1 << 2,
        Added           = 1 << 3,
        Checkout        = 1 << 4,
        Controlled      = 1 << 5,
        UpToDate        = 1 << 6,
        OutOfDate       = 1 << 7,
        Conflicted      = 1 << 8,
        DeletedOnServer = 1 << 9,
        Locked          = 1 << 10,
        LockedRemote    = 1 << 11,
        HiddenChanged   = 1 << 12,
    }

    internal class LockStatusData
    {
        internal readonly AssetStatus Status;
        internal readonly string LockedBy;
        internal readonly string WorkspaceName;

        internal LockStatusData(
            AssetStatus status,
            string lockedBy,
            string workspaceName)
        {
            Status = status;
            LockedBy = lockedBy;
            WorkspaceName = workspaceName;
        }
    }

    internal class ClassifyAssetStatus
    {
        internal static bool IsPrivate(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.Private);
        }

        internal static bool IsIgnored(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.Ignored);
        }

        internal static bool IsControlled(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.Controlled);
        }

        internal static bool IsLocked(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.Locked);
        }

        internal static bool IsLockedRemote(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.LockedRemote);
        }

        internal static bool IsOutOfDate(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.OutOfDate);
        }

        internal static bool IsDeletedOnServer(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.DeletedOnServer);
        }

        internal static bool IsConflicted(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.Conflicted);
        }

        internal static bool IsAdded(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.Added);
        }

        internal static bool IsCheckedOut(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.Checkout);
        }

        internal static bool IsHiddenChanged(AssetStatus status)
        {
            return ContainsAny(status, AssetStatus.HiddenChanged);
        }

        static bool ContainsAny(AssetStatus status, AssetStatus matchTo)
        {
            return (status & matchTo) != AssetStatus.None;
        }
    }
}

using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticGui;

namespace Unity.PlasticSCM.Editor
{
    static class SetupCloudProjectId
    {
        internal static bool IsUnitTesting { get; set; }

        internal static bool HasCloudProjectId()
        {
            if (IsUnitTesting)
                return false;
            
            return !string.IsNullOrEmpty(GetCloudProjectId());
        }

        internal static string GetCloudProjectId()
        {
            //disable Warning CS0618  'PlayerSettings.cloudProjectId' is obsolete: 'cloudProjectId is deprecated
#pragma warning disable 0618
            return PlayerSettings.cloudProjectId;
        }

        internal static void ForWorkspace(
            WorkspaceInfo wkInfo,
            IPlasticAPI plasticApi)
        {
            RepositoryInfo repInfo = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    RepositorySpec repSpec = plasticApi.GetRepositorySpec(wkInfo);

                    repInfo = plasticApi.GetRepositoryInfo(repSpec);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.LogException(
                            "SetupCloudProjectId",
                            waiter.Exception);
                        return;
                    }

                    SetupCloudProjectId.ForRepository(repInfo);
                });
        }

        internal static void ForRepository(RepositoryInfo repInfo)
        {
            string projectId = repInfo.GUID.ToString();

            // Invokes PlayerSettings.SetCloudProjectId(projectId)
            SetCloudProjectId(projectId);

            AssetDatabase.SaveAssets();
        }

        internal static void SetCloudProjectId(string projectId)
        {
            MethodInfo InternalSetCloudProjectId = PlayerSettingsType.GetMethod(
                "SetCloudProjectId",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (InternalSetCloudProjectId == null)
            {
                Debug.LogWarning(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CannotWriteCloudProjectId));
                return;
            }

            InternalSetCloudProjectId.Invoke(
                null, new object[] { projectId });
        }

        static readonly Type PlayerSettingsType =
            typeof(UnityEditor.PlayerSettings);

        static readonly ILog mLog = LogManager.GetLogger("SetupCloudProjectId");
    }
}
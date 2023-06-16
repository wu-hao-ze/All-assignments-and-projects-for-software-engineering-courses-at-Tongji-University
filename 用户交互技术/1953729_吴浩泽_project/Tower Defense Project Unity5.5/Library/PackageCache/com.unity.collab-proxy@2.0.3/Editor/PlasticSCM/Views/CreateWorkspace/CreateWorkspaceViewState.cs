using System.IO;

using UnityEngine;

using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace
{
    internal class CreateWorkspaceViewState
    {
        internal enum WorkspaceModes
        {
            Developer,
            Gluon
        }

        internal string RepositoryName { get; set; }
        internal string WorkspaceName { get; set; }
        internal WorkspaceModes WorkspaceMode { get; set; }
        internal ProgressControlsForViews.Data ProgressData { get; set; }

        internal static CreateWorkspaceViewState BuildForProjectDefaults()
        {
            string projectName = Application.productName;

            return new CreateWorkspaceViewState()
            {
                RepositoryName = projectName,
                WorkspaceName = projectName,
                WorkspaceMode = WorkspaceModes.Developer,
                ProgressData = new ProgressControlsForViews.Data()
            };
        }
    }
}

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI.UIElements;
using Unity.PlasticSCM.Editor.Views.Welcome;
using Unity.PlasticSCM.Editor.Tool;

namespace Unity.PlasticSCM.Editor.Views
{
    internal class DownloadPlasticExeWindow :
        EditorWindow,
        DownloadAndInstallOperation.INotify
    {
        internal bool IsPlasticInstalling { get { return mIsPlasticInstalling; } }

        internal static void ShowWindow(
            RepositorySpec repSpec,
            bool isGluonMode,
            string installCloudFrom,
            string installEnterpriseFrom,
            string cancelInstallFrom)
        {
            DownloadPlasticExeWindow window = GetWindow<DownloadPlasticExeWindow>();
            window.mRepSpec = repSpec;
            window.mIsGluonMode = isGluonMode;
            window.mInstallCloudFrom = installCloudFrom;
            window.mInstallEnterpriseFrom = installEnterpriseFrom;
            window.mCancelInstallFrom = cancelInstallFrom;

            window.titleContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.UnityVersionControl));

            if (EditionToken.IsCloudEdition())
                window.minSize = window.maxSize = new Vector2(700, 160);
            else
                window.minSize = window.maxSize = new Vector2(700, 230);

            window.Show();
        }
        void DownloadAndInstallOperation.INotify.InstallationStarted()
        {
            mIsPlasticInstalling = true;
        }

        void DownloadAndInstallOperation.INotify.InstallationFinished()
        {
            mIsPlasticInstalling = false;
        }

        void OnEnable()
        {
            BuildComponents();
            mInstallerFile = GetInstallerTmpFileName.ForPlatform();
        }

        void OnDestroy()
        {
            Dispose();
        }

        void Dispose()
        {
            mDownloadCloudEditionButton.clicked -= DownloadCloudEditionButton_Clicked;
            if (!EditionToken.IsCloudEdition())
                mDownloadEnterpriseButton.clicked -= DownloadEnterpriseEditionButton_Clicked;
            mCancelButton.clicked -= CancelButton_Clicked;
            EditorApplication.update -= CheckForPlasticExe;
        }

        void DownloadCloudEditionButton_Clicked()
        {
            TrackFeatureUseEvent.For(mRepSpec, mInstallCloudFrom);

            DownloadAndInstallOperation.Run(
                Edition.Cloud,
                mInstallerFile,
                mProgressControls,
                this);

            EditorApplication.update += CheckForPlasticExe;
        }

        void DownloadEnterpriseEditionButton_Clicked()
        {
            TrackFeatureUseEvent.For(mRepSpec, mInstallEnterpriseFrom);

            DownloadAndInstallOperation.Run(
                Edition.Enterprise,
                mInstallerFile,
                mProgressControls,
                this);
        }

        void CancelButton_Clicked()
        {
            if (!IsExeAvailable.ForMode(mIsGluonMode))
                TrackFeatureUseEvent.For(mRepSpec, mCancelInstallFrom);

            Close();
        }

        void CheckForPlasticExe()
        {
            // executable becomes available halfway through the install
            // we do not want to say install is done too early
            // when progress control finishes, cancel button will be enabled
            // then we can check for exe existing
            if (mCancelButton.enabledSelf && IsExeAvailable.ForMode(mIsGluonMode))
            {
                mMessageLabel.text = "Unity Version Control installed. You can now use the feature.";
                mCancelButton.text =
                    PlasticLocalization.GetString(PlasticLocalization.Name.CloseButton);
                mRequireMessageLabel.Collapse();
                mDownloadCloudEditionButton.Collapse();
                mDownloadEnterpriseButton.Collapse();
            }
        }

        void BuildComponents()
        {
            VisualElement root = rootVisualElement;
            root.Clear();
            InitializeLayoutAndStyles();

            mRequireMessageLabel = root.Q<Label>("requireMessage");
            mMessageLabel = root.Q<Label>("message");
            mDownloadCloudEditionButton = root.Q<Button>("downloadCloudEdition");
            mDownloadEnterpriseButton = root.Q<Button>("downloadEnterpriseEdition");
            mCancelButton = root.Q<Button>("cancel");
            mProgressControlsContainer = root.Q<VisualElement>("progressControlsContainer");

            root.Q<Label>("title").text =
                PlasticLocalization.GetString(PlasticLocalization.Name.InstallUnityVersionControl);

            mDownloadCloudEditionButton.text =
                PlasticLocalization.GetString(PlasticLocalization.Name.DownloadCloudEdition);
            mDownloadCloudEditionButton.clicked += DownloadCloudEditionButton_Clicked;

            if (EditionToken.IsCloudEdition())
            {
                mDownloadEnterpriseButton.Collapse();
                DownloadPlasticExeWindow window = GetWindow<DownloadPlasticExeWindow>();
            }
            else
            {
                mMessageLabel.text =
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.WhichVersionInstall);
                mDownloadEnterpriseButton.text =
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.DownloadEnterpriseEdition);
                mDownloadEnterpriseButton.clicked += DownloadEnterpriseEditionButton_Clicked;
            }

            mCancelButton.text =
                PlasticLocalization.GetString(PlasticLocalization.Name.CancelButton);
            mCancelButton.clicked += CancelButton_Clicked;

            mProgressControls = new ProgressControlsForDialogs(
                new VisualElement[] {
                    mDownloadCloudEditionButton,
                    mDownloadEnterpriseButton,
                    mCancelButton
                });

            mProgressControlsContainer.Add(mProgressControls);
        }

        void InitializeLayoutAndStyles()
        {
            rootVisualElement.LoadLayout(typeof(DownloadPlasticExeWindow).Name);
            rootVisualElement.LoadStyle(typeof(DownloadPlasticExeWindow).Name);
        }

        bool mIsGluonMode;
        string mInstallCloudFrom;
        string mInstallEnterpriseFrom;
        string mCancelInstallFrom;
        RepositorySpec mRepSpec;

        string mInstallerFile;
        
        bool mIsPlasticInstalling = false;

        Label mRequireMessageLabel;
        Label mMessageLabel;
        Button mDownloadCloudEditionButton;
        Button mDownloadEnterpriseButton;
        Button mCancelButton;
        VisualElement mProgressControlsContainer;
        ProgressControlsForDialogs mProgressControls;
    }
}
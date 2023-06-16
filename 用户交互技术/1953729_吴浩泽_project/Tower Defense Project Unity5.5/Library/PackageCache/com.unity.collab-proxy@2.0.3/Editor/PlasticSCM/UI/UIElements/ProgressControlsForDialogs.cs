using UnityEditor;
using UnityEngine.UIElements;
using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI.UIElements
{
    class ProgressControlsForDialogs :
        VisualElement,
        IProgressControls
    {
        internal class Data
        {
            internal bool IsWaitingAsyncResult;
            internal float ProgressPercent;
            internal string ProgressMessage;

            internal MessageType StatusType;
            internal string StatusMessage;

            internal void CopyInto(Data other)
            {
                other.IsWaitingAsyncResult = IsWaitingAsyncResult;
                other.ProgressPercent = ProgressPercent;
                other.ProgressMessage = ProgressMessage;
                other.StatusType = StatusType;
                other.StatusMessage = StatusMessage;
            }
        }

        internal Data ProgressData { get { return mData; } }

        internal void ForcedUpdateProgress()
        {
            if (mData.IsWaitingAsyncResult)
            {
                mUndefinedProgress.Show();
                mPercentageLabel.Show();
                mLoadingSpinner.Start();
                EditorApplication.update += UpdatePercent;
            }
            else
            {
                mUndefinedProgress.Collapse();
                mPercentageLabel.Collapse();
                mLoadingSpinner.Stop();
                EditorApplication.update -= UpdatePercent;
            }

            mStatusLabel.text = mData.StatusMessage;
            mProgressLabel.text = mData.ProgressMessage;
        }

        internal void UpdatePercent()
        {
            if (mData.ProgressPercent >= 0)
                mPercentageLabel.text = string.Format("({0}%)", (int)(mData.ProgressPercent * 100));
            else
                mPercentageLabel.text = "";
        }

        internal ProgressControlsForDialogs(
            VisualElement[] actionControls)
        {
            mActionControls = actionControls;

            InitializeLayoutAndStyles();

            BuildComponents();
        }

        internal void EnableActionControls(bool enable)
        {
            if (mActionControls != null)
                foreach (var control in mActionControls)
                    if (control != null)
                        control.SetEnabled(enable);
        }

        void IProgressControls.HideProgress()
        {
            EnableActionControls(true);

            mData.IsWaitingAsyncResult = false;
            mData.ProgressMessage = string.Empty;
            ForcedUpdateProgress();
        }

        void IProgressControls.ShowProgress(string message)
        {
            EnableActionControls(false);

            CleanStatusMessage(mData);

            mData.IsWaitingAsyncResult = true;
            mData.ProgressPercent = -1f;
            mData.ProgressMessage = message;
            ForcedUpdateProgress();
        }

        void IProgressControls.ShowError(string message)
        {
            mData.StatusMessage = message;
            mData.StatusType = MessageType.Error;
            ForcedUpdateProgress();
        }

        void IProgressControls.ShowNotification(string message)
        {
            mData.StatusMessage = message;
            mData.StatusType = MessageType.Info;
            ForcedUpdateProgress();
        }

        void IProgressControls.ShowSuccess(string message)
        {
            mData.StatusMessage = message;
            mData.StatusType = MessageType.Info;
            ForcedUpdateProgress();
        }

        void IProgressControls.ShowWarning(string message)
        {
            mData.StatusMessage = message;
            mData.StatusType = MessageType.Warning;
            ForcedUpdateProgress();
        }

        void BuildComponents()
        {
            mUndefinedProgress = this.Q<VisualElement>("UndefinedProgress");
            mProgressLabel = this.Q<Label>("Progress");
            mStatusLabel = this.Q<Label>("Status");
            mPercentageLabel = this.Q<Label>("Percentage");

            mLoadingSpinner = new LoadingSpinner();
            mUndefinedProgress.Add(mLoadingSpinner);
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(ProgressControlsForDialogs).Name);

            this.LoadStyle(typeof(ProgressControlsForDialogs).Name);
        }

        static void CleanStatusMessage(Data data)
        {
            data.StatusMessage = string.Empty;
            data.StatusType = MessageType.None;
        }

        Data mData = new Data();
        VisualElement mUndefinedProgress;
        Label mProgressLabel;
        Label mStatusLabel;
        Label mPercentageLabel;
        VisualElement[] mActionControls;

        LoadingSpinner mLoadingSpinner;
    }
}
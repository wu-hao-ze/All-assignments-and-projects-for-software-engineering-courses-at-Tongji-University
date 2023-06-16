using Codice.Client.Common;
using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class EditorProgressControls : IProgressControls
    {
        internal EditorProgressControls(GuiMessage.IGuiMessage guiMessage)
        {
            mGuiMessage = guiMessage;
        }

        void IProgressControls.HideProgress()
        {
            EditorProgressBar.ClearProgressBar();
        }

        void IProgressControls.ShowError(string message)
        {
            mGuiMessage.ShowError(message);
        }

        void IProgressControls.ShowNotification(string message)
        {
            mGuiMessage.ShowMessage(
                UnityConstants.PLASTIC_WINDOW_TITLE,
                message,
                GuiMessage.GuiMessageType.Informational);
        }

        void IProgressControls.ShowProgress(string message)
        {
            EditorProgressBar.ShowProgressBar(message, 1f);
        }

        void IProgressControls.ShowSuccess(string message)
        {
            mGuiMessage.ShowMessage(
                UnityConstants.PLASTIC_WINDOW_TITLE,
                message,
                GuiMessage.GuiMessageType.Informational);
        }

        void IProgressControls.ShowWarning(string message)
        {
            mGuiMessage.ShowMessage(
                UnityConstants.PLASTIC_WINDOW_TITLE,
                message,
                GuiMessage.GuiMessageType.Warning);
        }

        GuiMessage.IGuiMessage mGuiMessage;
    }
}

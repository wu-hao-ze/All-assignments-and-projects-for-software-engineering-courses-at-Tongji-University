using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using PlasticPipe.Certificates;
using Unity.PlasticSCM.Editor.UI;
using UnityEditor;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class ChannelCertificateUiImpl : IChannelCertificateUI
    {
        internal ChannelCertificateUiImpl()
        {
        }

        CertOperationResult IChannelCertificateUI.AcceptNewServerCertificate(PlasticCertInfo serverCertificate)
        {
            return GetUserResponse(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.NewCertificateTitle),
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.NewCertificateMessageUnityVCS),
                serverCertificate);
        }

        CertOperationResult IChannelCertificateUI.AcceptChangedServerCertificate(PlasticCertInfo serverCertificate)
        {
            return GetUserResponse(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.ExistingCertificateChangedTitle),
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.ExistingCertificateChangedMessageUnityVCS),
                serverCertificate);
        }

        bool IChannelCertificateUI.AcceptInvalidHostname(string certHostname, string serverHostname)
        {
            bool result = false;

            GUIActionRunner.RunGUIAction(delegate {
                result = EditorUtility.DisplayDialog(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.InvalidCertificateHostnameTitle),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.InvalidCertificateHostnameMessage,
                        certHostname, serverHostname),
                    PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoButton));
            });

            return result;
        }

        CertOperationResult GetUserResponse(
            string title, string message, PlasticCertInfo serverCertificate)
        {
            GuiMessage.GuiMessageResponseButton result =
                GuiMessage.GuiMessageResponseButton.Neutral;

            GUIActionRunner.RunGUIAction(delegate {
                result = GuiMessage.ShowQuestion(
                    title,
                    GetCertificateMessageString(message, serverCertificate),
                    PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                    PlasticLocalization.GetString(PlasticLocalization.Name.CancelButton),
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoButton));
            });

            switch (result)
            {
                case GuiMessage.GuiMessageResponseButton.Positive:
                    return CertOperationResult.AddToStore;
                case GuiMessage.GuiMessageResponseButton.Negative:
                    return CertOperationResult.DoNotAddToStore;
                case GuiMessage.GuiMessageResponseButton.Neutral:
                    return CertOperationResult.Cancel;
                default:
                    return CertOperationResult.Cancel;
            }
        }

        string GetCertificateMessageString(string message, PlasticCertInfo serverCertificate)
        {
            return string.Format(message,
                CertificateUi.GetCnField(serverCertificate.Subject),
                CertificateUi.GetCnField(serverCertificate.Issuer),
                serverCertificate.Format,
                serverCertificate.ExpirationDateString,
                serverCertificate.KeyAlgorithm,
                serverCertificate.CertHashString);
        }
    }
}

using System;
using UnityEditor;

using Codice.CM.Common;
using System.Threading.Tasks;
using Codice.Client.Common;
using Codice.Client.Common.Connection;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Codice.Client.Common.Threading;
using Unity.PlasticSCM.Editor.WebApi;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class CredentialsUiImpl : AskCredentialsToUser.IGui
    {
        AskCredentialsToUser.DialogData AskCredentialsToUser.IGui.AskUserForCredentials(string servername, SEIDWorkingMode seidWorkingMode)
        {
            AskCredentialsToUser.DialogData result = null;

            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
                return result;

            GUIActionRunner.RunGUIAction(delegate
            {
                result = CredentialsDialog.RequestCredentials(
                        servername, seidWorkingMode, ParentWindow.Get());
            });

            return result;
        }

        void AskCredentialsToUser.IGui.ShowSaveProfileErrorMessage(string message)
        {
            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
                return;

            GUIActionRunner.RunGUIAction(delegate
            {
                GuiMessage.ShowError(string.Format(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.CredentialsErrorSavingProfile),
                    message));
            });
        }

        AskCredentialsToUser.DialogData AskCredentialsToUser.IGui.AskUserForOidcCredentials(
            string server)
        {
            throw new NotImplementedException("OIDC authentication not supported yet.");
        }

        AskCredentialsToUser.DialogData AskCredentialsToUser.IGui.AskUserForSsoCredentials(
            string cloudServer)
        {
            AskCredentialsToUser.DialogData result = null;

            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
                return result;

            GUIActionRunner.RunGUIAction(delegate
            {
                result = RunSSOCredentialsRequest(
                    cloudServer, CloudProjectSettings.accessToken);
            });

            return result;
        }

        AskCredentialsToUser.DialogData RunSSOCredentialsRequest(
            string cloudServer,
            string unityAccessToken)
        {
            if (string.IsNullOrEmpty(unityAccessToken))
            {
                return SSOCredentialsDialog.RequestCredentials(
                    cloudServer, ParentWindow.Get());
            }

            TokenExchangeResponse tokenExchangeResponse =
                WaitUntilTokenExchange(unityAccessToken);

            // There is no internet connection, so no way to get credentials
            if (tokenExchangeResponse == null)
            {
                return new AskCredentialsToUser.DialogData(
                    false, null, null, false,
                    SEIDWorkingMode.SSOWorkingMode);
            }

            if (tokenExchangeResponse.Error == null)
            {
                return new AskCredentialsToUser.DialogData(
                    true, 
                    tokenExchangeResponse.User,
                    tokenExchangeResponse.AccessToken, 
                    false,
                    SEIDWorkingMode.SSOWorkingMode);
            }

            return SSOCredentialsDialog.RequestCredentials(
                cloudServer, ParentWindow.Get());
        }

        static TokenExchangeResponse WaitUntilTokenExchange(
            string unityAccessToken)
        {
            TokenExchangeResponse result = null;

            Task.Run(() =>
            {
                try
                {
                    result = WebRestApiClient.PlasticScm.
                        TokenExchange(unityAccessToken);
                }
                catch (Exception ex)
                {
                    ExceptionsHandler.LogException(
                        "CredentialsUiImpl", ex);
                }
            }).Wait();

            return result;
        }
    }
}

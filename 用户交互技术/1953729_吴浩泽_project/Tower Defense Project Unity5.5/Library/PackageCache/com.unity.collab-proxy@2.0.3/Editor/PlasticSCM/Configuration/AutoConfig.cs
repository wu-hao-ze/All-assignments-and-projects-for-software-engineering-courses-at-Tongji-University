using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome;
using Unity.PlasticSCM.Editor.WebApi;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal static class AutoConfig
    {
        internal static TokenExchangeResponse PlasticCredentials(
            string unityAccessToken,
            string serverName,
            string projectPath)
        {
            SetupUnityEditionToken.CreateCloudEditionTokenIfNeeded();

            bool isClientConfigConfigured = ClientConfig.IsConfigured();
            if (!isClientConfigConfigured)
            {
                ConfigureClientConf.FromUnityAccessToken(
                    unityAccessToken, serverName, projectPath);
            }

            TokenExchangeResponse tokenExchangeResponse = WebRestApiClient.
                PlasticScm.TokenExchange(unityAccessToken);

            if (tokenExchangeResponse.Error != null)
                return tokenExchangeResponse;

            CloudEditionWelcomeWindow.JoinCloudServer(
                serverName,
                tokenExchangeResponse.User,
                tokenExchangeResponse.AccessToken);

            if (!isClientConfigConfigured)
                return tokenExchangeResponse;
          
            ConfigureProfile.ForServerIfNeeded(
                serverName,
                tokenExchangeResponse.User);

            return tokenExchangeResponse;
        }

        static class ConfigureClientConf
        {
            internal static void FromUnityAccessToken(
                string unityAccessToken,
                string serverName,
                string projectPath)
            {
                CredentialsResponse response = WebRestApiClient.
                    PlasticScm.GetCredentials(unityAccessToken);

                if (response.Error != null)
                {
                    UnityEngine.Debug.LogErrorFormat(
                        PlasticLocalization.GetString(
                            PlasticLocalization.Name.ErrorGettingCredentialsCloudProject),
                        response.Error.Message,
                        response.Error.ErrorCode);

                    return;
                }

                ClientConfigData configData = BuildClientConfigData(
                    serverName, projectPath, response);

                ClientConfig.Get().Save(configData);
            }

            static ClientConfigData BuildClientConfigData(
                string serverName,
                string projectPath,
                CredentialsResponse response)
            {
                SEIDWorkingMode workingMode = GetWorkingMode(response.Type);

                ClientConfigData configData = new ClientConfigData();

                configData.WorkspaceServer = serverName;
                configData.CurrentWorkspace = projectPath;
                configData.WorkingMode = workingMode.ToString();
                configData.SecurityConfig = UserInfo.GetSecurityConfigStr(
                    workingMode,
                    response.Email,
                    GetPassword(response.Token, response.Type));
                configData.LastRunningEdition = InstalledEdition.Get();
                return configData;
            }

            static string GetPassword(
                string token,
                CredentialsResponse.TokenType tokenType)
            {
                if (tokenType == CredentialsResponse.TokenType.Bearer)
                    return BEARER_PREFIX + token;

                return token;
            }

            static SEIDWorkingMode GetWorkingMode(CredentialsResponse.TokenType tokenType)
            {
                if (tokenType == CredentialsResponse.TokenType.Bearer)
                    return SEIDWorkingMode.SSOWorkingMode;

                return SEIDWorkingMode.LDAPWorkingMode;
            }

            const string BEARER_PREFIX = "Bearer ";
        }

        static class ConfigureProfile
        {
            internal static void ForServerIfNeeded(string serverName, string user)
            {
                ProfileManager profileManager = CmConnection.Get().GetProfileManager();

                ServerProfile serverProfile = profileManager.GetProfileForServer(serverName);

                if (serverProfile != null)
                    return;

                serverProfile = ProfileManager.CreateProfile(
                    serverName,
                    SEIDWorkingMode.SSOWorkingMode,
                    user);

                profileManager.SaveProfile(serverProfile);
            }
        }

    }

}


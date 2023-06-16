using System;
using System.IO;
using System.Net;

using Unity.Plastic.Newtonsoft.Json;

using Codice.Client.Common.WebApi;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticGui.WebApi.Responses;

namespace Unity.PlasticSCM.Editor.WebApi
{
    internal static class WebRestApiClient
    {
        internal static class PlasticScm
        {
            internal static TokenExchangeResponse TokenExchange(string unityAccessToken)
            {
                Uri endpoint = mWebApiUris.GetFullUri(
                    string.Format(TokenExchangeEndpoint, unityAccessToken));

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    return GetResponse<TokenExchangeResponse>(request);
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat(
                        "Unable to exchange tokens '{0}': {1}",
                        endpoint.ToString(), ex.Message);

                    mLog.DebugFormat(
                        "StackTrace:{0}{1}",
                        Environment.NewLine, ex.StackTrace);

                    return null;
                }
            }

            internal static IsCollabProjectMigratedResponse IsCollabProjectMigrated(string bearerToken, string projectId)
            {
                Uri endpoint = mWebApiUris.GetFullUri(string.Format(
                    IsCollabProjectMigratedEndpoint, projectId));

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Headers.Add(
                        HttpRequestHeader.Authorization,
                        string.Format("Bearer {0}", bearerToken));

                    return GetResponse<IsCollabProjectMigratedResponse>(request);
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat(
                        "Unable to retrieve is collab migrated '{0}': {1}",
                        endpoint.ToString(), ex.Message);

                    mLog.DebugFormat(
                        "StackTrace:{0}{1}",
                        Environment.NewLine, ex.StackTrace);

                    return null;
                }
            }

            internal static NewVersionResponse GetLastVersion(Edition plasticEdition)
            {
                Uri endpoint = mWebApiUris.GetFullUri(
                        WebApiEndpoints.LastVersion.NewVersion,
                        "9.0.0.0",
                        WebApiEndpoints.LastVersion.GetEditionString(plasticEdition),
                        WebApiEndpoints.LastVersion.GetPlatformString());

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                    request.Method = "GET";
                    request.ContentType = "application/json";

                    return GetResponse<NewVersionResponse>(request);
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat(
                        "Unable to retrieve new versions from '{0}': {1}",
                        endpoint.ToString(), ex.Message);

                    mLog.DebugFormat(
                        "StackTrace:{0}{1}",
                        Environment.NewLine, ex.StackTrace);

                    return null;
                }
            }

            internal static CredentialsResponse GetCredentials(string unityToken)
            {
                Uri endpoint = mWebApiUris.GetFullUri(
                    WebApiEndpoints.Authentication.Credentials,
                    unityToken);

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                    request.Method = "GET";
                    request.ContentType = "application/json";

                    return GetResponse<CredentialsResponse>(request);
                }
                catch (Exception ex)
                {
                    return new CredentialsResponse
                    {
                        Error = BuildLoggedErrorFields(ex, endpoint)
                    };
                }
            }

            internal static CurrentUserAdminCheckResponse IsUserAdmin(
                string organizationName,
                string authToken)
            {
                Uri endpoint = mWebApiUris.GetFullUri(
                    IsUserAdminEnpoint,
                    organizationName);

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                    request.Method = "GET";
                    request.ContentType = "application/json";

                    string authenticationToken = "Basic " + authToken;

                    request.Headers.Add(
                       HttpRequestHeader.Authorization, authenticationToken);

                    return GetResponse<CurrentUserAdminCheckResponse>(request);
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat(
                       "Unable to retrieve is user admin '{0}': {1}",
                       endpoint.ToString(), ex.Message);

                    mLog.DebugFormat(
                        "StackTrace:{0}{1}",
                        Environment.NewLine, ex.StackTrace);

                    return new CurrentUserAdminCheckResponse
                    {
                        Error = BuildLoggedErrorFields(ex, endpoint)
                    };
                }
            }

            const string IsBetaEnabledEndpoint = "api/unity-package/beta/is-enabled";
            const string TokenExchangeEndpoint = "api/oauth/unityid/exchange/{0}";
            const string IsCollabProjectMigratedEndpoint = "api/cloud/unity/projects/{0}/is-migrated";
            const string IsUserAdminEnpoint  = "api/cloud/organizations/{0}/is-user-admin";
            static readonly PlasticWebApiUris mWebApiUris = PlasticWebApiUris.BuildDefault();
        }

        internal static class CloudServer
        {
            internal static string WebLogin(
                string webServerUri,
                string organizationName,
                OrganizationCredentials credentials)
            {
                Uri endpoint = new Uri(
                    new Uri(webServerUri),
                    string.Format(WebLoginEndPoint, organizationName));

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Timeout = 5000;

                    WriteBody(request, credentials);

                    return GetResponse<string>(request);
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat(
                        "Unable to retrieve the organization login '{0}': {1}",
                        endpoint.ToString(), ex.Message);

                    mLog.DebugFormat(
                        "StackTrace:{0}{1}",
                        Environment.NewLine, ex.StackTrace);

                    return null;
                }
            }

            internal static ChangesetFromCollabCommitResponse GetChangesetFromCollabCommit(
                string webServerUri,
                string organizationName,
                string webLoginAccessToken,
                string projectId,
                string commitSha)
            {
                Uri endpoint = new Uri(
                    new Uri(webServerUri),
                    string.Format(GetChangesetFromCollabCommitEndpoint,
                        organizationName, projectId, commitSha));

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Headers.Add(
                       HttpRequestHeader.Authorization,
                       string.Format("Bearer {0}", webLoginAccessToken));

                    return GetResponse<ChangesetFromCollabCommitResponse>(request);
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat(
                        "Unable to retrieve the changeset from collab commit '{0}': {1}",
                        endpoint.ToString(), ex.Message);

                    mLog.DebugFormat(
                        "StackTrace:{0}{1}",
                        Environment.NewLine, ex.StackTrace);

                    return null;
                }
            }

            const string WebLoginEndPoint = "api/v1/organizations/{0}/login/accesstoken";
            const string GetChangesetFromCollabCommitEndpoint = "cloudapi/v1/organizations/{0}/repos/{1}/collabcommit/{2}/changeset";
        }

        static void WriteBody(WebRequest request, object body)
        {
            using (Stream st = request.GetRequestStream())
            using (StreamWriter writer = new StreamWriter(st))
            {
                writer.Write(JsonConvert.SerializeObject(body));
            }
        }

        static TRes GetResponse<TRes>(WebRequest request)
        {
            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();

                if (string.IsNullOrEmpty(json))
                    return default(TRes);

                return JsonConvert.DeserializeObject<TRes>(json);
            }
        }

        static ErrorResponse.ErrorFields BuildLoggedErrorFields(
            Exception ex, Uri endpoint)
        {
            LogException(ex, endpoint);

            return new ErrorResponse.ErrorFields
            {
                ErrorCode = ErrorCodes.ClientError,
                Message = ex.Message
            };
        }

        static void LogException(Exception ex, Uri endpoint)
        {
            mLog.ErrorFormat(
                "There was an error while calling '{0}': {1}",
                endpoint.ToString(), ex.Message);

            mLog.DebugFormat(
                "StackTrace:{0}{1}",
                Environment.NewLine, ex.StackTrace);
        }

        static readonly ILog mLog = LogManager.GetLogger("WebRestApiClient");
    }
}

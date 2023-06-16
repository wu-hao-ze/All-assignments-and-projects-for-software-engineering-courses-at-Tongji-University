using Unity.Plastic.Newtonsoft.Json;
using PlasticGui.WebApi.Responses;

namespace Unity.PlasticSCM.Editor.WebApi
{
    public class IsCollabProjectMigratedResponse
    {
        [JsonProperty("error")]
        public ErrorResponse.ErrorFields Error { get; set; }

        [JsonProperty("IsMigrated")]
        public bool IsMigrated { get; set; }

        [JsonProperty("WebServerUri")]
        public string WebServerUri { get; set; }

        [JsonProperty("PlasticCloudOrganizationName")]
        public string PlasticCloudOrganizationName { get; set; }

        [JsonProperty("Credentials")]
        public CredentialsResponse Credentials { get; set; }
    }
}

using System.Reflection;

using Unity.Plastic.Newtonsoft.Json;

using PlasticGui.WebApi.Responses;

namespace Unity.PlasticSCM.Editor.WebApi
{
    /// <summary>
    /// Response to current user admin check request.
    /// </summary>
    public class CurrentUserAdminCheckResponse
    {
        /// <summary>
        /// Error caused by the request.
        /// </summary>
        [JsonProperty("error")]
        public ErrorResponse.ErrorFields Error { get; set; }

        [JsonProperty("isCurrentUserAdmin")]
        public bool IsCurrentUserAdmin { get; set; }

        [JsonProperty("organizationName")]
        public string OrganizationName { get; set; }
    }
}

using Unity.Plastic.Newtonsoft.Json;
using PlasticGui.WebApi.Responses;

namespace Unity.PlasticSCM.Editor.WebApi
{
    /// <summary>
    /// Response to token exchange request.
    /// </summary>
    public class TokenExchangeResponse
    {
        /// <summary>
        /// Error caused by the request.
        /// </summary>
        [JsonProperty("error")]
        public ErrorResponse.ErrorFields Error { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        [JsonProperty("user")]
        public string User { get; set; }

        /// <summary>
        /// The access token.
        /// </summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token.
        /// </summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }
}

using Unity.Plastic.Newtonsoft.Json;

using PlasticGui.WebApi.Responses;

namespace Unity.PlasticSCM.Editor.WebApi
{
    /// <summary>
    /// Response to credentials request.
    /// </summary>
    public class CredentialsResponse
    {
        /// <summary>
        /// Error caused by the request.
        /// </summary>
        [JsonProperty("error")]
        public ErrorResponse.ErrorFields Error { get; set; }

        /// <summary>
        /// Type of the token.
        /// </summary>
        public enum TokenType : int
        {
            /// <summary>
            /// Password token.
            /// </summary>
            Password = 0,

            /// <summary>
            /// Bearer token.
            /// </summary>
            Bearer = 1,
        }

        /// <summary>
        /// Get the type of the token.
        /// </summary>
        [JsonIgnore]
        public TokenType Type
        {
            get { return (TokenType)TokenTypeValue; }
        }

        /// <summary>
        /// The user's email.
        /// </summary>
        [JsonProperty("email")]
        public string Email;

        /// <summary>
        /// The credential's token.
        /// </summary>
        [JsonProperty("token")]
        public string Token;

        /// <summary>
        /// The token type represented as an integer.
        /// </summary>
        [JsonProperty("tokenTypeValue")]
        public int TokenTypeValue;
    }
}

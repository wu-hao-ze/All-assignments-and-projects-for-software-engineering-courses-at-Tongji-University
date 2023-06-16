using Unity.Plastic.Newtonsoft.Json;
using PlasticGui.WebApi.Responses;

namespace Unity.PlasticSCM.Editor.WebApi
{
    public class ChangesetFromCollabCommitResponse
    {
        /// <summary>
        /// Error caused by the request.
        /// </summary>
        [JsonProperty("error")]
        public ErrorResponse.ErrorFields Error { get; set; }

        /// <summary>
        /// The repository ID
        /// </summary>
        [JsonProperty("repId")]
        public uint RepId { get; set; }

        /// <summary>
        /// The repository module ID
        /// </summary>
        [JsonProperty("repModuleId")]
        public uint RepModuleId { get; set; }

        /// <summary>
        /// The changeset ID
        /// </summary>
        [JsonProperty("changesetId")]
        public long ChangesetId { get; set; }

        /// <summary>
        /// The branch ID
        /// </summary>
        [JsonProperty("branchId")]
        public long BranchId { get; set; }
    }
}

using Unity.Plastic.Newtonsoft.Json;

public class OrganizationCredentials
{
    [JsonProperty("user")]
    public string User { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }
}


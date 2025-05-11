using Newtonsoft.Json;

namespace rca.data.remote.dto;

public class AuthDto
{
    [JsonProperty("access_token")] public required string Token { get; init; }
}
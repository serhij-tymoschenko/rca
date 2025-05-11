using Newtonsoft.Json;

namespace rca.data.remote.dto;

public class PolyscanDto
{
    [JsonProperty("result")] public required List<TransactionResult> Result { get; set; }

    public class TransactionResult
    {
        [JsonProperty("methodId")] public required string MethodId { get; set; }

        [JsonProperty("contractAddress")] public required string ContractAddress { get; set; }
    }
}
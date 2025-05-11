using System.Net;
using Newtonsoft.Json;
using rca.bot;
using rca.config;
using rca.data.remote.dto;
using rca.utils.constants;

namespace rca.data.remote.api;

public class PolyscanApi(HttpClient client)
{
    private readonly string _polyscanQuery =
        $"api?" +
        $"module=account" +
        $"&action=txlist" +
        $"&address={ApiConstants.RedditDeployerAddress}" +
        $"&page=1" +
        $"&offset=40" +
        $"&sort=desc" +
        $"&apikey={Config.POLYSCAN_API_KEY}";

    public async Task<List<string>?> GetEntityIdsAsync()
    {
        try
        {
            var reqMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(ApiConstants.PolyscanApiEndpoint + _polyscanQuery),
                Headers =
                {
                    { "User-Agent", ApiConstants.UserAgent },
                    { "Accept", "application/json" }
                }
            };

            var resMessage = await client.SendAsync(reqMessage);
            resMessage.EnsureSuccessStatusCode();

            var resContent = await resMessage.Content.ReadAsStringAsync();
            var polyscan = JsonConvert.DeserializeObject<PolyscanDto>(resContent);

            if (polyscan != null)
            {
                var entityIds = polyscan.Result
                    .Where(transaction => transaction.MethodId == ApiConstants.CreateMethodId)
                    .ToList()
                    .ConvertAll(transaction =>
                    {
                        var entityId = $"nft_eip155:137_{transaction.ContractAddress.Substring(2)}_0";
                        return entityId;
                    });
                return entityIds;
            }
        }
        catch (Exception e)
        {
            Bot.Log($"Error getting entity ids: {e.Message}");
        }

        return null;
    }
}
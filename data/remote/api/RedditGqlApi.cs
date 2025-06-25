using System.Collections;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using rca.bot;
using rca.data.entities;
using rca.data.entities.mappers;
using rca.data.remote.dto;
using rca.utils;
using rca.utils.constants;

namespace rca.data.remote.api;

public class RedditGqlApi(HttpClient client)
{
    public async Task<string?> GetTokenAsync()
    {
        var clientAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApiConstants.ClientId}:"));
        var authHeader = new AuthenticationHeaderValue("Basic", clientAuth);

        var reqBody = new { scopes = new[] { "*", "email", "pii" } };
        var reqJson = JsonConvert.SerializeObject(reqBody);
        var reqContent = new StringContent(reqJson, Encoding.UTF8, "application/json");
        var reqMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiConstants.AuthRedditEndpoint + "/api/access_token"),
            Headers =
            {
                { "Accept", "text/vnd.reddit.partial+html, text/html;q=0.9" },
                { "Authorization", authHeader.ToString() },
                { "User-Agent", ApiConstants.UserAgent }
            },
            Content = reqContent
        };

        try
        {
            var resMessage = await client.SendAsync(reqMessage);
            resMessage.EnsureSuccessStatusCode();

            var resContent = await resMessage.Content.ReadAsStringAsync();
            var auth = JsonConvert.DeserializeObject<AuthDto>(resContent);
            return auth.Token;
        }
        catch (Exception e)
        {
            Bot.Log($"Error getting token: {e.Message}");
        }

        return null;
    }

    public async Task<List<string>?> GetStorefrontIdsAsync(string token, List<string> entityIds)
    {
        var authHeader = new AuthenticationHeaderValue("Bearer", token);

        var reqBody = new
        {
            id = "e9865cc4d93d",
            variables = new
            {
                // ids = entityIds.ToArray()
                ids = new[] {"nft_eip155:137_724b4d5b52dac4f19ca29d5c8109dc9069c2b8e0_0"}
            }
        };
        var reqJson = JsonConvert.SerializeObject(reqBody);
        var reqContent = new StringContent(reqJson, Encoding.UTF8, "application/json");
        var reqMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiConstants.GqlRedditEndpoint),
            Headers =
            {
                { "Accept", "text/vnd.reddit.partial+html, text/html;q=0.9" },
                { "Authorization", authHeader.ToString() },
                { "User-Agent", ApiConstants.UserAgent }
            },
            Content = reqContent
        };

        try
        {
            var resMessage = await client.SendAsync(reqMessage);
            resMessage.EnsureSuccessStatusCode();

            var resContent = await resMessage.Content.ReadAsStringAsync();
            var nftsDetails = JsonConvert.DeserializeObject<InventoryItemsDto>(resContent);
            if (nftsDetails != null)
            {
                var storefrontIds = nftsDetails.ResData.InventoryItems.Edges.ConvertAll(edge =>
                {
                    var id = edge.Node.Benefits.AvatarOutfit.Id;
                    return $"storefront_nft_{id}";
                });
                return storefrontIds;
            }
        }
        catch (Exception e)
        {
            Bot.Log($"Error getting storefront ids: {e.Message}");
        }

        return null;
    }

    public async Task<Rca?> GetRcaAsync(string token,
        string storefrontId)
    {
        var authHeader = new AuthenticationHeaderValue("Bearer", token);

        var reqBody = new
        {
            id = "fcd362ce7dbf",
            variables = new
            {
                id = storefrontId
            }
        };
        var reqJson = JsonConvert.SerializeObject(reqBody);
        var reqContent = new StringContent(reqJson, Encoding.UTF8, "application/json");
        var reqMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiConstants.GqlRedditEndpoint),
            Headers =
            {
                { "Accept", "text/vnd.reddit.partial+html, text/html;q=0.9" },
                { "Authorization", authHeader.ToString() },
                { "User-Agent", ApiConstants.UserAgent }
            },
            Content = reqContent
        };

        try
        {
            var resMessage = await client.SendAsync(reqMessage);
            resMessage.EnsureSuccessStatusCode();
            var resContent = await resMessage.Content.ReadAsStringAsync();
            var avatarStorefront = JsonConvert.DeserializeObject<AvatarStorefrontDto>(resContent);
            if (avatarStorefront != null)
            {
                var rca = avatarStorefront.ConvertToRca();
                return rca;
            }
        }
        catch (Exception e)
        {
            Bot.Log($"Error getting rcas: {e.Message}");
            if (e is HttpRequestException)
                throw new AuthException();
        }

        return null;
    }
}
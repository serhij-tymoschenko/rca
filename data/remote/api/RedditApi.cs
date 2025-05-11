using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using rca.bot;
using rca.config;
using rca.data.entities;
using rca.utils;
using rca.utils.constants;

namespace rca.data.remote.api;

public class RedditApi(HttpClient client, CookieContainer cookies)
{
    public async Task<Session?> GetSession()
    {
        var query = "/avatar/shop";
        var reqMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiConstants.RedditEndpoint + query),
            Headers =
            {
                { "Accept", "text/vnd.reddit.partial+html, text/html;q=0.9" },
                { "User-Agent", "RCA" }
            }
        };

        try
        {
            var resMessage = await client.SendAsync(reqMessage);
            resMessage.EnsureSuccessStatusCode();
            var content = await resMessage.Content.ReadAsStringAsync();

            var cookieCollection = cookies.GetCookies(new Uri(ApiConstants.RedditEndpoint + query));
            if (cookieCollection.Count == 0) throw new Exception("No cookies found");

            var sessionTracker = "";
            var csrfToken = "";
            foreach (Cookie cookie in cookieCollection)
            {
                if (cookie.Name == "session_tracker") sessionTracker = cookie.Value;

                if (cookie.Name == "csrf_token") csrfToken = cookie.Value;
            }

            var postData = Regex
                .Match(content, DataRegex.PostDataRegex)
                .Groups["json"]
                .Value;

            return new Session
            {
                CsrfToken = csrfToken,
                SessionTracker = sessionTracker,
                PostData = postData.Replace("&quot;", "\"")
            };
        }
        catch (Exception e)
        {
            Bot.Log($"Error creating store session: {e.Message}");
        }

        return null;
    }

    public async Task<List<string>?> GetMainPageStorefrontIds(Session session)
    {
        var query = "/svc/shreddit/" +
                    "shop-home-data-fetcher";

        var values = new Dictionary<string, string>
        {
            { "postData", session.PostData },
            { "csrf_token", session.CsrfToken }
        };

        var content = new FormUrlEncodedContent(values);
        var reqMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiConstants.RedditEndpoint + query),
            Headers =
            {
                { "accept", "text/vnd.reddit.partial+html, text/html;q=0.9" },
                { "User-agent", ApiConstants.UserAgent },
                {
                    "Authorization",
                    $"bearer {Config.REDDIT_API_KEY}"
                },
                { "Cookie", $"session_tracker={session.SessionTracker}; csrf_token={session.CsrfToken};" }
            },
            Content = content
        };

        try
        {
            var resMessage = await client.SendAsync(reqMessage);
            resMessage.EnsureSuccessStatusCode();

            var htmlDoc = new HtmlDocument();
            var stream = await resMessage.Content.ReadAsStreamAsync();
            htmlDoc.Load(stream);

            var recentlyReleasedHeader = htmlDoc
                .DocumentNode
                .Descendants("marketplace-section-header")
                .FirstOrDefault(header =>
                    header.Attributes["title-label"].Value.Equals("Recently released"));

            if (recentlyReleasedHeader == null) throw new Exception("No recently released header found");

            var divInnerHtml = recentlyReleasedHeader.ParentNode.InnerHtml;
            var storefrontIds = Regex
                .Matches(divInnerHtml, DataRegex.StorefrontRegex)
                .ToList()
                .ConvertAll(match => match.Value)
                .ToHashSet()
                .ToList();

            return storefrontIds;
        }
        catch (Exception e)
        {
            Bot.Log($"Error getting main page storefront ids: {e.Message}");
            if (e is HttpRequestException)
                throw new AuthException();
        }

        return null;
    }

    public async Task<List<string>?> GetStorefrontIdsAsync()
    {
        var query = "/svc/shreddit/" +
                    "shop-gallery-data-fetcher?" +
                    "sort=RELEASE_TIME_REVERSE" +
                    "&releasedWithinDays=30" +
                    "&status=AVAILABLE" +
                    "&lastEnd=cmVsZWFzZWRfYXQ9fGxpc3RpbmdfaWQ9";
        var reqMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiConstants.RedditEndpoint + query),
            Headers =
            {
                { "Accept", "text/vnd.reddit.partial+html, text/html;q=0.9" },
                {
                    "Authorization",
                    $"bearer {Config.REDDIT_API_KEY}"
                },
                { "User-Agent", ApiConstants.UserAgent }
            }
        };

        try
        {
            var resMessage = await client.SendAsync(reqMessage);
            resMessage.EnsureSuccessStatusCode();
            var resContent = await resMessage.Content.ReadAsStringAsync();

            var storefrontIds = Regex
                .Matches(resContent, DataRegex.StorefrontRegex)
                .ToList()
                .ConvertAll(match => match.Value)
                .ToHashSet()
                .ToList();

            if (storefrontIds.Count > 0) return storefrontIds;
        }
        catch (Exception e)
        {
            Bot.Log($"Error getting storefront ids: {e.Message}");
        }

        return null;
    }
}
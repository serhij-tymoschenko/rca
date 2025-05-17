using System.Net;
using rca.bot;
using rca.data.db;
using rca.data.entities;
using rca.data.remote.api;
using rca.utils;

namespace rca.services;

public class RcaService(RedditApi api, RedditGqlApi gqlApi, CookieContainer cookies)
{
    private readonly object _lastIdLock = new();
    private readonly object _storefrontIdsLock = new();
    private string _lastId = "";
    private List<string> _storefrontIds = new();

    private void SetLastId(string id)
    {
        lock (_lastIdLock)
        {
            _lastId = id;
        }
    }

    private string GetLastId()
    {
        lock (_lastIdLock)
        {
            return _lastId;
        }
    }

    private void SetStorefrontIds(List<string> newStorefrontIds)
    {
        lock (_storefrontIdsLock)
        {
            if (newStorefrontIds.Count > 0 && !_storefrontIds.SequenceEqual(newStorefrontIds))
                _storefrontIds = newStorefrontIds;
        }
    }

    private List<string> GetStorefrontIds()
    {
        lock (_storefrontIdsLock)
        {
            return new List<string>(_storefrontIds);
        }
    }

    private async Task StartMainPageFetching()
    {
        Session? session = null;

        while (true)
        {
            try
            {
                if (session == null) throw new Exception("Session is null");

                var storefrontIds = await api.GetMainPageStorefrontIds(session);
                if (storefrontIds == null) throw new Exception("No storefrontIds fetched");

                var lastIdIndex = storefrontIds.IndexOf(GetLastId());
                if (lastIdIndex != -1)
                {
                    var localStorefrontIds = storefrontIds.Take(lastIdIndex).ToList();
                    SetStorefrontIds(localStorefrontIds);
                }
            }
            catch (Exception e)
            {
                var sessionTracker = "";
                var csrfToken = "";
                var cookieCollection =
                    cookies.GetCookies(new Uri("https://www.reddit.com/svc/shreddit/shop-gallery-data-fetcher"));

                foreach (Cookie cookie in cookieCollection)
                {
                    if (cookie.Name == "session_tracker") sessionTracker = cookie.Value;
                    if (cookie.Name == "csrf_token") csrfToken = cookie.Value;
                }

                session = new Session
                {
                    CsrfToken = csrfToken,
                    SessionTracker = sessionTracker,
                    PostData =
                        """{"metadata":{"id":"d4ee172c-bffd-45ee-91a5-c073ef66618f","telemetry":{"description":"Blossom 2025","header":"production_1"}},"sections":[{"body":{"content":[{"data":{"filters":{"tags":["mates-2025"]},"first":6,"query":"batchListings"},"description":"","id":"mates_2025_category_1","image":"https://i.redd.it/snoovatar/snoo_assets/marketing/h0RgXQoPyj4_mates_2025_column_1.png","subtitle":"","title":""},{"data":{"filters":{"tags":["mates-2025"]},"first":6,"query":"batchListings"},"description":"","id":"mates_2025_category_2","image":"https://i.redd.it/snoovatar/snoo_assets/marketing/UM_S6U7O-3w_mates_2025_column_2.png","subtitle":"","title":""},{"data":{"filters":{"tags":["mates-2025"]},"first":6,"query":"batchListings"},"description":"","id":"mates_2025_category_3","image":"https://i.redd.it/snoovatar/snoo_assets/marketing/EH0U2Ofh-5k_mates_2025_column_3.png","subtitle":"","title":""}],"size":"large","title":"Mates' Day"},"id":"mates_2025_lasso","type":"categoriesRow"},{"body":{"cta":"See all","data":{"filters":{"tags":["mates-2025"]},"query":"batchListings"},"title":"Mates' Day"},"id":"mates_2025_outfits","type":"outfitsRow"},{"body":{"content":[{"data":{"filters":{"tags":["blossom-2025"]},"first":6,"query":"batchListings"},"description":"","id":"blossoming_category_1","image":"https://i.redd.it/snoovatar/snoo_assets/marketing/OkcCRSqJnMI_blossoming_jas_column_1.png","subtitle":"","title":""},{"data":{"filters":{"tags":["blossom-2025"]},"first":6,"query":"batchListings"},"description":"","id":"blossoming_category_2","image":"https://i.redd.it/snoovatar/snoo_assets/marketing/B4zOd2_v3yc_blossoming_jas_column_2.png","subtitle":"","title":""},{"data":{"filters":{"tags":["blossom-2025"]},"first":6,"query":"batchListings"},"description":"","id":"blossoming_category_3","image":"https://i.redd.it/snoovatar/snoo_assets/marketing/98DxMmkhLAw_blossoming_jas_column_3.png","subtitle":"","title":""}],"size":"large","title":"Blossoming: foliage, flora, and reemerging fauna"},"id":"blossoming_lasso","type":"categoriesRow"},{"body":{"cta":"See all","data":{"filters":{"tags":["blossom-2025"]},"query":"batchListings"},"title":"Blossoming: foliage, flora, and reemerging fauna"},"id":"blossoming_outfits","type":"outfitsRow"},{"body":{"cta":"See all","data":{"filters":{"releasedWithinDays":30,"status":"AVAILABLE"},"query":"batchListings","sort":"RELEASE_TIME_REVERSE"},"title":"Recently released"},"id":"recently_released","type":"outfitsRow"},{"body":{"cta":"See all","data":{"filters":{"theme":"ALMOST_GONE"},"query":"batchListings"},"title":"Almost Gone"},"id":"almost_gone","type":"outfitsRow"},{"body":{"image":"https://www.redditstatic.com/marketplace-assets/v1/mobile/browse_all_banner.png","title":"Browse All Collectibles"},"id":"browse_all","type":"browseAll"},{"body":{"cta":"See all","data":{"filters":{"theme":"POPULAR"},"query":"batchListings"},"title":"Popular"},"id":"popular","type":"outfitsRow"},{"body":{"cta":"See all","data":{"filters":{"theme":"FEATURED"},"query":"batchListings"},"title":"Explore More"},"id":"explore","type":"outfitsRow"},{"body":{"cta":"All Creators","data":{"filters":{"ids":["t2_114e25","t2_131npa","t2_138uu5q","t2_14gjzd","t2_1nfapja","t2_1s91aw2","t2_1u83iemi","t2_20jwj3ud","t2_255lg","t2_2f2stuee","t2_2g1lvsmt","t2_2hsa9xmf","t2_2nr0qevj","t2_3389d","t2_35ndznbx","t2_35t6ikyy","t2_3riu9","t2_3wj7q8wy","t2_3xzp2","t2_42ek22xl","t2_438n059h","t2_4773e","t2_4cw9v","t2_4cz90476","t2_4fc8mpjg","t2_4fe4e7sd","t2_4je30vck","t2_4kplck72","t2_4omt4m3f","t2_4wk3iwuw","t2_4y3wyq4d","t2_51g2oegy","t2_56spdd7","t2_5lgf2","t2_5o0m2mwx","t2_5zdzecta","t2_64pyk0zy","t2_65o9v7yt","t2_6en3h2ca","t2_6o6m0q7m","t2_6s1c1p5","t2_7g8fygz72","t2_89ucorewb","t2_8afmycoc","t2_8fsvp5kp","t2_8icpwu1pt","t2_8xa4ej5j","t2_8xcum","t2_9fhivlc","t2_9lps39e3","t2_9ncg6a6t","t2_9ux0m0fm","t2_9xo8wcdl","t2_aacv1dvt","t2_ab310krl","t2_ab8aryad","t2_ao3qa7kg","t2_b0uyqjtxr","t2_b34t5ct","t2_b7wzzr2ga","t2_b89qbt49","t2_b94812hxc","t2_bgl5y","t2_bic06jvr","t2_bijnkfgb0","t2_bru6e852","t2_bu4jbomn","t2_buqn1","t2_c25hqb6","t2_caeax9zq","t2_d3f6c0na","t2_d7hib584","t2_d7lr6","t2_d98qk5tg0","t2_desn1a0wx","t2_dpgm5cnk","t2_e182xtvm","t2_en0jl","t2_f3lnj","t2_fie9np5j","t2_gcjgi","t2_gfbpoafl","t2_goowg5s","t2_hbvpzxim","t2_hbvygc3l","t2_ir00p","t2_jqkbt6pu","t2_jx7p4wj7","t2_k8ezrq7t","t2_kqxmzj1i","t2_ku7y9","t2_kxv6j62e","t2_m7xfy8j1","t2_mdvd36cu","t2_medsjjhl","t2_mfpsx1v2","t2_mttuxyhy","t2_mxf1h8cd","t2_n06mnnw3","t2_n3cyvgym","t2_ndd63h87","t2_ouryl","t2_p1sq63dg","t2_p490s","t2_pzg8f7p6","t2_qb858cak","t2_rkgvk","t2_rtuqcuyu","t2_sfs6iw50","t2_siao2yxk","t2_srqdrrh3","t2_tcbt5k34","t2_tj9e1bec","t2_tkmt7smp","t2_tkppx2yf","t2_to0yr82c","t2_ttgozto0","t2_u5vmrp76","t2_ulrnporj","t2_us8suia0","t2_uyz6029q","t2_uz5cs6oa","t2_uzdyfxcm","t2_v3rmryry","t2_va6lxems","t2_va82s0g9","t2_vfysvwof","t2_vhfm6gaf","t2_vi4i34ih","t2_vi98cobn","t2_vig9po1f","t2_viht2","t2_vsafx5z","t2_wngpd","t2_xvc02","t2_yfwt7","t2_yi9g30h"]},"query":"batchArtists"},"title":"Creators"},"id":"43bfaad6-a355-4ba1-958b-4088c9042ae9","type":"artistsCarousel"},{"body":{"content":[{"data":{"filters":{"status":"AVAILABLE","tags":["artsy"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Collectibles for every aesthetic that take creativity to a new level.","id":"genre_category_artsy","subtitle":"A nod to the fine arts","title":"Artsy"},{"data":{"filters":{"status":"AVAILABLE","tags":["animals"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Aww-worthy collectibles inspired by furry friends.","id":"genre_category_animals","subtitle":"Made you aww","title":"Animals"},{"data":{"filters":{"status":"AVAILABLE","tags":["scifi"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Collectibles so unreal, it's like they're from an alternate universe.","id":"genre_category_scifi","subtitle":"Out of this world","title":"Sci-Fi"},{"data":{"filters":{"status":"AVAILABLE","tags":["robots"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"High-tech collectibles that are ready to take over the world.","id":"genre_category_robots","subtitle":"Beep boop","title":"Robots"},{"data":{"filters":{"status":"AVAILABLE","tags":["pets"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Collectibles that come complete with loyal companions.","id":"genre_category_pets","subtitle":"Redditor's best friend","title":"Pets"},{"data":{"filters":{"status":"AVAILABLE","tags":["snoo"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Catch a glimpse of Snoo in these uncanny collectibles.","id":"genre_category_snoo","subtitle":"Looks familiar","title":"True to Snoo"},{"data":{"filters":{"status":"AVAILABLE","tags":["edgy"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Super sick collectibles with punk and anime themes.","id":"genre_category_edgy","subtitle":"For the cool kids","title":"Edgy"},{"data":{"filters":{"status":"AVAILABLE","tags":["spooky"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Spooky collectibles with creatures that lurk in the dark.","id":"genre_category_spooky","subtitle":"Nightmare fuel","title":"Monsters"},{"data":{"filters":{"status":"AVAILABLE","tags":["nature"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Marvel at the beauty of nature in collectible-form.","id":"genre_category_nature","subtitle":"The great outdoors","title":"Nature"},{"data":{"filters":{"status":"AVAILABLE","tags":["food"]},"first":6,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"description":"Collectibles that look so good, they're practically edible.","id":"genre_category_food","subtitle":"Nom nom nom","title":"Food"}],"cta":"See all","title":"Explore"},"id":"genre","type":"categoriesRow"},{"body":{"cta":"All Items","data":{"filters":{"status":"AVAILABLE"},"first":9,"query":"batchListings","sort":"CREATION_TIME_REVERSE"},"title":"Browse the Gallery"},"id":"mini_gallery","type":"outfitsGallery"},{"body":{"content":[{"data":{"filters":{"priceUpperBound":999},"first":6,"query":"batchListings","sort":"PRICE"},"description":"","id":"price_10","subtitle":"Great value","title":"Low"},{"data":{"filters":{"priceLowerBound":999,"priceUpperBound":4999},"first":6,"query":"batchListings","sort":"PRICE"},"description":"","id":"price_50","subtitle":"Just right","title":"Mid"},{"data":{"filters":{"priceLowerBound":4999},"first":6,"query":"batchListings","sort":"PRICE"},"description":"","id":"price_100","subtitle":"Worth it","title":"High"}],"cta":"See all","title":"By Price"},"id":"price","type":"categoriesRow"},{"body":{"content":[{"data":{"filters":{"tags":["spooky2023"]},"first":6,"query":"batchListings","sort":"PRICE"},"description":"Trick or treat?","id":"creator_drops_spooky_sequel","subtitle":"So nice we did it twice","title":"Spooky Sequel (Oct 2023)"},{"data":{"filters":{"priceLowerBound":299,"tags":["cp4"]},"first":6,"query":"batchListings","sort":"PRICE"},"description":"Collectibles that bring on warm, fuzzy feelings of nostalgia.","id":"creator_drops_category_cp4","subtitle":"Retro Reimagined","title":"Gen 4 (Aug 2023)"},{"data":{"filters":{"tags":["cp3"]},"first":6,"query":"batchListings","sort":"PRICE"},"description":"Third-generation collectibles with more variety than ever.","id":"creator_drops_category_cp3","subtitle":"Lucky number three","title":"Gen 3 (April 2023)"},{"data":{"filters":{"tags":["cp2"]},"first":6,"query":"batchListings","sort":"PRICE"},"description":"Original collectibles from the second generation of creators.","id":"creator_drops_category_cp2","subtitle":"The second generation","title":"Gen 2 (Oct 2022)"},{"data":{"filters":{"tags":["cp1"]},"first":6,"query":"batchListings","sort":"PRICE"},"description":"Trailblazing collectibles from the first-ever creator drop.","id":"creator_drops_category_cp1","subtitle":"First drop ever!","title":"Gen 1 (July 2022)"}],"cta":"See all","title":"Past Drops"},"id":"past_drops","type":"categoriesRow"}]}"""
                };

                Bot.Log($"Error fetching main page: {e.Message}");
            }

            await Task.Delay(1200);
        }
    }

    private async Task StartCategoryFetching()
    {
        while (true)
        {
            try
            {
                var storefrontIds = await api.GetStorefrontIdsAsync();
                if (storefrontIds == null) throw new Exception("No storefrontIds fetched");

                var lastIdIndex = storefrontIds.IndexOf(GetLastId());
                if (lastIdIndex != -1)
                {
                    var localStorefrontIds = storefrontIds.Take(lastIdIndex).ToList();
                    SetStorefrontIds(localStorefrontIds);
                }
            }
            catch (Exception e)
            {
                Bot.Log($"Error fetching category: {e.Message}");
            }

            await Task.Delay(2400);
        }
    }

    public async Task StartAsync()
    {
        var initialLastId = await Database.GetLastStorefrontIdAsync();
        SetLastId(initialLastId);

        StartMainPageFetching();
        StartCategoryFetching();

        var token = await gqlApi.GetTokenAsync();

        while (true)
        {
            try
            {
                var storefrontIds = GetStorefrontIds();
                if (token == null) throw new Exception("Token not generated");

                if (storefrontIds.Count > 0 && storefrontIds[0] != GetLastId())
                {
                    var rcas = new List<Rca>();

                    foreach (var id in storefrontIds)
                    {
                        var rca = await gqlApi.GetRcaAsync(token, id);
                        if (rca != null)
                            rcas.Add((Rca)rca);
                    }

                    rcas.Reverse();
                    foreach (var rca in rcas) Bot.PostRcaAsync(rca, MessageType.Rca);

                    await Database.SetLastStorefrontIdAsync(storefrontIds[0]);
                    SetLastId(storefrontIds[0]);
                }
            }
            catch (Exception e)
            {
                if (e is AuthException) token = await gqlApi.GetTokenAsync();
                Bot.Log($"Error getting RCAs: {e.Message}");
            }

            await Task.Delay(2400);
        }
    }
}
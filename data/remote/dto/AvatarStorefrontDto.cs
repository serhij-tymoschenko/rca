using Newtonsoft.Json;

namespace rca.data.remote.dto;

public class AvatarStorefrontDto
{
    [JsonProperty("data")] public required Data ResData { get; set; }

    public class Data
    {
        [JsonProperty("avatarStorefront")] public required AvatarStorefront AvatarStorefront { get; set; }
    }

    public class AvatarStorefront
    {
        [JsonProperty("listings")] public required Listings Listings { get; set; }
    }

    public class Listings
    {
        [JsonProperty("edges")] public required List<Edge> Edges { get; set; }
    }

    public class Edge
    {
        [JsonProperty("node")] public required Node Node { get; set; }
    }

    public class Node
    {
        [JsonProperty("id")] public required string Id { get; set; }

        [JsonProperty("totalQuantity")] public int TotalQuantity { get; set; }

        [JsonProperty("productOffer")] public required ProductOffer ProductOffer { get; set; }

        [JsonProperty("artist")] public required StorefrontArtist Artist { get; set; }

        [JsonProperty("item")] public required StorefrontItem Item { get; set; }
    }

    public class ProductOffer
    {
        [JsonProperty("pricePackages")] public required List<PricePackage> PricePackages { get; set; }
    }

    public class PricePackage
    {
        [JsonProperty("price")] public required string Price { get; set; }
    }

    public class StorefrontArtist
    {
        [JsonProperty("redditorInfo")] public required RedditorInfo RedditorInfo { get; set; }
    }

    public class RedditorInfo
    {
        [JsonProperty("displayName")] public required string DisplayName { get; set; }
        [JsonProperty("prefixedName")] public required string PrefixedName { get; set; }
    }

    public class StorefrontItem
    {
        [JsonProperty("images")] public required List<Image> Images { get; set; }

        [JsonProperty("drop")] public required Drop Drop { get; set; }

        [JsonProperty("benefits")] public required Benefits Benefits { get; set; }

        [JsonProperty("nft")] public required Nft Nft { get; set; }
    }

    public class Image
    {
        [JsonProperty("url")] public required string Url { get; set; }
    }

    public class Drop
    {
        [JsonProperty("size")] public int Size { get; set; }
    }

    public class Benefits
    {
        [JsonProperty("avatarOutfit")] public required AvatarOutfit AvatarOutfit { get; set; }
    }

    public class AvatarOutfit
    {
        [JsonProperty("accessoryIds")] public required List<string> AccessoryIds { get; set; }

        [JsonProperty("backgroundImage")] public required BackgroundImage BackgroundImage { get; set; }

        [JsonProperty("preRenderImage")] public required PreRenderImage PreRenderImage { get; set; }
    }

    public class BackgroundImage
    {
        [JsonProperty("url")] public required string Url { get; set; }
    }

    public class PreRenderImage
    {
        [JsonProperty("url")] public required string Url { get; set; }
    }

    public class Nft
    {
        [JsonProperty("title")] public required string Title { get; set; }

        [JsonProperty("description")] public required string Description { get; set; }
    }
}
namespace rca.utils.constants;

public static class DataRegex
{
    public const string StorefrontRegex = "storefront_nft_.{26}";
    public const string PostDataRegex = @"name=""postData""\s+value=""(?<json>(?:[^""\\]|\\.)*)""";   
}
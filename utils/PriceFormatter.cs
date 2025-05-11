namespace rca.utils;

public static class PriceFormatter
{
    public static string ValueToPrice(this string value)
    {
        var dollars = decimal.Parse(value) / 100;
        return $"${dollars:0.00}";
    }
}
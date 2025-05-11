namespace rca.utils;

public static class StringFormatter
{
    public static string ValueToPrice(this string value)
    {
        var dollars = decimal.Parse(value) / 100;
        return $"${dollars:0.00}";
    }
}
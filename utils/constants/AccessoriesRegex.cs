namespace rca.utils.constants;

public static class AccessoriesRegex
{
    public const string Right = "accessory_(.{11})$";
    public const string Left = "accessory_back_(.{11})$";
    public const string Bottoms = "body_bottom_(.{11})$";
    public const string Tops = "body_(.{11})$";
    public const string Face = "face_lower_(.{11})$";
    public const string Eyes = "face_upper_(.{11})$";
    public const string Hats = "head_accessory_(.{11})$";
    public const string Hair = "hair_(.{11})(?:_(.{11}))?";
}
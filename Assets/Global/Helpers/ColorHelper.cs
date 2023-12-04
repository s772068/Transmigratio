using UnityEngine;

public static class ColorHelper {
    public static Color Parse(this Color owner, Vector4 val) => owner.Parse(val.x, val.y, val.z, val.w);
    public static Color Parse(this Color owner, Vector3 val) => owner.Parse(val.x, val.y, val.z);
    public static Color Parse(this Color owner, Vector2 val) => owner.Parse(val.x, val.y);
    public static Color Parse(this Color owner, float val) => owner.Parse(val);
    public static Color Parse(this Color owner, Vector3 type, float val) => owner.Parse(type * val);
    public static Color Parse(this Color owner, float red = 0, float green = 0, float blue = 0, float alpha = 255) {
        owner.r = Mathf.Abs(red) % 256 / 255;
        owner.g = Mathf.Abs(green) % 256 / 255;
        owner.b = Mathf.Abs(blue) % 256 / 255;
        owner.a = Mathf.Abs(alpha) % 256 / 255;
        return owner;
    }
}

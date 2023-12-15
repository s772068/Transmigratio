using UnityEngine;

public static class NumHelper {
    public static int Proportion(this int owner, params int[] vals) {
        int all = owner;
        for (int i = 0; i < vals.Length; ++i) {
            all += vals[i];
        }
        return owner / all;
    }
    public static float Proportion(this float owner, params float[] vals) {
        float all = owner;
        for (int i = 0; i < vals.Length; ++i) {
            all += vals[i];
        }
        return owner / all;
    }
    public static int Percent(this int owner, int percent) => (int)(owner * percent / 100);
    public static float Percent(this int owner, float percent) => owner * percent / 100f;
    public static float Percent(this float owner, float percent) => owner * percent / 100f;
}

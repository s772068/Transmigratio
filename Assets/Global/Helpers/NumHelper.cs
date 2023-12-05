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
}

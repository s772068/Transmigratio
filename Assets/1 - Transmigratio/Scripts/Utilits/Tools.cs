using System.Collections.Generic;
using System.Linq;
using System;

public static class Tools {
    //ToPercent
    public static List<float> ToPercents(this List<float> list) {
        List<float> res = new();
        float sum = list.Sum();
        for (int i = 0; i < list.Count; ++i) {
            res.Add(list[i] / sum * 100);
        }
        return res;
    }

    public static List<int> ToPercents(this List<int> list) {
        List<int> res = new();
        int sum = list.Sum();
        for (int i = 0; i < list.Count; ++i) {
            res.Add(list[i] / sum * 100);
        }
        return res;
    }

    public static List<T> GetData<T, V>(this List<V> list, Func<V, T> func) {
        List<T> res = new();
        for(int i = 0; i < list.Count; ++i) {
            res.Add(func(list[i]));
        }
        return res;
    }
}

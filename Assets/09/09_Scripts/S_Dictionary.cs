using System.Collections.Generic;
using System;

[Serializable]
public class S_Dictionary<K, V> {
    public List<Source> sources = new List<Source>();

    public List<K> Keys {
        get {
            List<K> res = new();
            for(int i = 0; i < sources.Count; ++i) {
                res.Add(sources[i].Key);
            }
            return res;
        }
    }
    public List<V> Values {
        get {
            List<V> res = new();
            for (int i = 0; i < sources.Count; ++i) {
                res.Add(sources[i].Value);
            }
            return res;
        }
    }

    public V this[K key] {
        get {
            for(int i = 0; i < sources.Count; ++i) {
                if (sources[i].Key.Equals(key)) {
                    return sources[i].Value;
                }
            }
            return default;
        }
        set {
            for (int i = 0; i < sources.Count; ++i) {
                if (sources[i].Key.Equals(key)) {
                    sources[i].Value = value;
                    return;
                }
            }
            sources.Add(new() { Key = key, Value = value });
        }
    }

    public Source FirstOrDefault(Predicate<Source> predicate) {
        for(int i = 0; i < sources.Count; ++i) {
            if (predicate(sources[i])) {
                return sources[i];
            }
        }
        return default;
    }

    [Serializable]
    public class Source {
        public K Key;
        public V Value;
    }
}

using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Paramiter {
    // Set через paramiter[key] = value
    [SerializeField] private SerializedDictionary<string, ParamiterValue> quantities = new();

    private bool _isPercent;

    // For JSON
    public SerializedDictionary<string, float> Quantities {
        set {
            foreach(var pair in value) {
                quantities.Add(pair.Key, new(pair.Value));
            }
        }
    }

    public Paramiter(bool isPercent) {
        _isPercent = isPercent;
    }

    public float this[string key] {
        get => quantities[key].Value;
        set {
            if (!quantities.ContainsKey(key)) {
                quantities.Add(key, new(value));
            }
            quantities[key].Value = value;
        }
    }

    public ParamiterValue GetValue(string key) => quantities[key];
    public float GetStartValue(string key) => quantities[key].StartValue;

    public Dictionary<string, float> GetValues() {
        Dictionary<string, float> res = new();
        float full = quantities.Values.Sum(v => v.Value);
        foreach (var pair in quantities) {
            res[pair.Key] = _isPercent ? pair.Value.Value / full * 100 : pair.Value.Value;
        }
        return res;
    }

    public void Init(params (string, float)[] quantity) {
        for (int i = 0; i < quantity.Length; ++i) {
            ParamiterValue val = new(quantity[i].Item2);
            quantities[quantity[i].Item1] = val;
        }
    }

    public void Init(params string[] quantity) {
        for (int i = 0; i < quantity.Length; ++i) {
            ParamiterValue val = new(0);
            quantities[quantity[i]] = val;
        }
    }

    public (string key, float value) GetMax() {
        (string key, float value) res = default;
        foreach (var pair in quantities) {
            if(pair.Value.Value > res.value) {
                res.key = pair.Key;
                res.value = pair.Value.Value;
            }
        }
        return res;
    }

    public static Paramiter operator +(Paramiter p1, Paramiter p2) {
        Paramiter paramiter = new(p1._isPercent || p1._isPercent);
        foreach(var quantity in p1.quantities) {
            paramiter[quantity.Key] = quantity.Value.Value;
        }
        foreach(var quantity in p2.quantities) {
            paramiter[quantity.Key] = quantity.Value.Value;
        }
        return paramiter;
    }
}

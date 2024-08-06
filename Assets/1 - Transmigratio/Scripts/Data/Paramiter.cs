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
            foreach (var pair in quantities) {
                quantities[pair.Key].Percent = UpdateQuantityProcent(pair.Key);
            }
        }
    }

    public Dictionary<string, float> GetQuantities() {
        Dictionary<string, float> res = new();
        foreach (var pair in quantities) {
            res[pair.Key] = _isPercent ? pair.Value.Percent : pair.Value.Value;
        }
        return res;
    }

    public Paramiter(bool isPercent) {
        this._isPercent = isPercent;
    }

    public ParamiterValue this[string key] {
        get => quantities[key];
        set {
            if (!quantities.ContainsKey(key)) {
                quantities.Add(key, new(value.Value));
            }
            quantities[key].Value = value.Value;
            foreach (var pair in quantities) {
                quantities[pair.Key].Percent = UpdateQuantityProcent(pair.Key);
            }
        }
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

    public (string key, float value) GetMaxQuantity() {
        (string key, float value) res = default;
        foreach (var pair in quantities) {
            if(pair.Value.Value > res.value) {
                res.key = pair.Key;
                res.value = pair.Value.Value;
            }
        }
        return res;
    }

    private int UpdateQuantityProcent(string name) {
        float sum = quantities.Sum(x => x.Value.Value);
        return (int) (quantities[name].Value / sum * 100);
    }
}

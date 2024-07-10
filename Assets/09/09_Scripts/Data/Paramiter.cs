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
    public SerializedDictionary<string, int> Quantities {
        set {
            foreach(var pair in value) {
                quantities.Add(pair.Key, new());
                quantities[pair.Key].Max = pair.Value;
                quantities[pair.Key].Value = pair.Value;
            }
            foreach (var pair in quantities) {
                quantities[pair.Key].Percent = UpdateQuantityProcent(pair.Key);
            }
        }
    }

    public Dictionary<string, int> GetQuantities() {
        Dictionary<string, int> res = new();
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
                quantities.Add(key, new() { Max = value.Value });
            }
            quantities[key].Value = value.Value;
            foreach (var pair in quantities) {
                quantities[pair.Key].Percent = UpdateQuantityProcent(pair.Key);
            }
        }
    }

    public void Init(params string[] quantityNames) {
        for(int i = 0; i < quantityNames.Length; ++i) {
            quantities[quantityNames[i]] = new();
        }
    }

    public (string key, int value) GetMaxQuantity() {
        (string key, int value) res = default;
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

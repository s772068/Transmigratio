using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Paramiter {
    // Set через paramiter[key] = value
    [SerializeField] private SerializedDictionary<string, ParamiterValue> quantities = new();

    private bool isPercent;

    // For JSON
    public SerializedDictionary<string, int> Quantities {
        set {
            foreach(var pair in value) {
                quantities.Add(pair.Key, new());
                quantities[pair.Key].max = pair.Value;
                quantities[pair.Key].value = pair.Value;
            }
            foreach (var pair in quantities) {
                quantities[pair.Key].percent = UpdateQuantityProcent(pair.Key);
            }
        }
    }

    public Dictionary<string, int> GetQuantities() {
        Dictionary<string, int> res = new();
        foreach (var pair in quantities) {
            res[pair.Key] = isPercent ? pair.Value.percent : pair.Value.value;
        }
        return res;
    }

    public Paramiter(bool isPercent) {
        this.isPercent = isPercent;
    }

    public ParamiterValue this[string key] {
        get => quantities[key];
        set {
            if (!quantities.ContainsKey(key)) {
                quantities.Add(key, new() { max = value.value });
            }
            quantities[key].value = value.value;
            foreach (var pair in quantities) {
                quantities[pair.Key].percent = UpdateQuantityProcent(pair.Key);
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
            if(pair.Value.value > res.value) {
                res.key = pair.Key;
                res.value = pair.Value.value;
            }
        }
        return res;
    }

    private int UpdateQuantityProcent(string name) {
        float sum = quantities.Sum(x => x.Value.value);
        return (int) (quantities[name].value / sum * 100);
    }
}

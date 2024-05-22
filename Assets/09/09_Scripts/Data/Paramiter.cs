using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Paramiter {
    // Set через paramiter[key] = value
    public SerializedDictionary<string, int> quantities = new();

    private Dictionary<string, int> percentQuantities = new();

    private bool isPercent;

    public Dictionary<string, int> Quantities => isPercent ? percentQuantities : quantities;

    public Paramiter(bool isPercent) {
        this.isPercent = isPercent;
    }

    public int this[string key] {
        get => isPercent ? percentQuantities[key] : quantities[key];
        set { quantities[key] = value; percentQuantities[key] = UpdateQuantityProcent(key); }
    }

    public void Init(params string[] quantityNames) {
        for(int i = 0; i < quantityNames.Length; ++i) {
            percentQuantities[quantityNames[i]] = 0;
            quantities[quantityNames[i]] = 0;
        }
    }

    public KeyValuePair<string, int> GetMax() {
        if (quantities.Count < 0) return default;
        else return quantities.FirstOrDefault(x => x.Value == quantities.Values.Max());
    }

    private int UpdateQuantityProcent(string name) {
        float sum = quantities.Sum(x => x.Value);
        return (int) (quantities[name] / sum * 100);
    }
}

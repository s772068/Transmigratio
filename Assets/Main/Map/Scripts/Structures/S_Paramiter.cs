using AYellowpaper.SerializedCollections;
using System.Linq;

[System.Serializable]
public class S_Paramiter {
    [UnityEngine.SerializeField] private SerializedDictionary<string, float> details = new();

    public string[] Keys => details.Keys.ToArray();
    public float[] Values => details.Values.ToArray();

    public float AllValues {
        get {
            float all = 0;
            foreach (var detail in details) {
                all += detail.Value;
            }
            return all;
        }
    }
    public string MaxKey {
        get {
            string key = "";
            float max = -1;
            foreach (var detail in details) {
                if (detail.Value > max) {
                    key = detail.Key;
                    max = detail.Value;
                }
            }
            return key;
        }
    }
    public float MaxValue {
        get {
            float max = -1;
            foreach (var detail in details) {
                if (detail.Value > max) {
                    max = detail.Value;
                }
            }
            return max;
        }
    }

    public float Get(string detail) {
        if (details == null) return -1f;
        if (!details.ContainsKey(detail)) return -1f;
        return details[detail];
    }
    // True
    public void Set(S_Paramiter value) {
        Set(value.details);
    }

    // True
    public void Set(SerializedDictionary<string, float> values) {
        foreach (var value in values) {
            Set(value.Key, value.Value);
        }
    }

    // True
    public void Set(string detail, float value) {
        if (details == null) details = new();
        details[detail] = value;
    }

    public bool Remove(string name) {
        return details.Remove(name);
    }

    public void Clear() {
        details.Clear();
    }
}

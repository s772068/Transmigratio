// Paramiters
// 0: ProdMode
// 1: Economics
// 2: Governments

using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class S_Civilization {
    [SerializeField] private int population;
    [SerializeField] private float takenFood;
    [SerializeField] private SerializedDictionary<string, SerializedDictionary<string, float>> paramiters = new();

    public SerializedDictionary<string, float> this[string paramiter] {
        get => paramiters[paramiter];
        set { Set(paramiter, value); }
    }
    public float this[string paramiter, string detail] {
        get => paramiters[paramiter][detail];
        set { Set(paramiter, detail, value); }
    }

    public int Population {
        get => population;
        set => population = value;
    }

    public float TakenFood {
        get => takenFood;
        set => takenFood = value;
    }

    public string[] ParamitersKeys => paramiters.Keys.ToArray();
    public string[] DetailsKeys(string paramiter) => paramiters[paramiter].Keys.ToArray();
    public float[] DetailsValues(string paramiter) => paramiters[paramiter].Values.ToArray();

    public SerializedDictionary<string, float> Get(string paramiter) {
        if (paramiters == null) return null;
        if (!paramiters.ContainsKey(paramiter)) return null;
        return paramiters[paramiter];
    }
    public float Get(string paramiter, string detail) {
        if (paramiters == null) return -1f;
        if (!paramiters.ContainsKey(paramiter)) return -1f;
        return paramiters[paramiter][detail];
    }

    #region Set
    public void Set(S_Civilization civilization) {
        population = civilization.population;
        takenFood = civilization.takenFood;

        Set(civilization.paramiters);
    }

    public void Set(SerializedDictionary<string, SerializedDictionary<string, float>> values) {
        foreach(var value in values) {
            Set(value.Key, value.Value);
        }
    }
    
    public void Set(string paramiter, SerializedDictionary<string, float> values) {
        foreach(var value in values) {
            Set(paramiter, value.Key, value.Value);
        }
    }
    
    public void Set(string paramiter, string detail, float value) {
        if (paramiters == null) paramiters = new();

        if (!paramiters.ContainsKey(paramiter))
            paramiters.Add(paramiter, new());

        if (!paramiters[paramiter].ContainsKey(detail))
            paramiters[paramiter].Add(detail, new());

        paramiters[paramiter][detail] = value;
    }
    #endregion

    #region AllValues
    public float AllValues {
        get {
            float all = 0;
            foreach (var paramiter in paramiters) {
                foreach(var detail in paramiter.Value) {
                    all += detail.Value;
                }
            }
            return all;
        }
    }

    public float AllValuesByDetail(string detail) {
        float all = 0;
        foreach (var paramiter in paramiters) {
            all += paramiter.Value[detail];
        }
        return all;
    }
    #endregion

    #region MaxKey
    public string MaxKey {
        get {
            string key = "";
            float max = -1;
            foreach (var paramiter in paramiters) {
                foreach (var detail in paramiter.Value) {
                    if (detail.Value > max) {
                        key = detail.Key;
                        max = detail.Value;
                    }
                }
            }
            return key;
        }
    }

    public string MaxKeyByDetail(string detail) {
        string key = "";
        float max = -1;
        foreach (var paramiter in paramiters) {
            if (paramiter.Value[detail] > max) {
                key = paramiter.Key;
                max = paramiter.Value[detail];
            }
        }
        return key;
    }
    #endregion

    #region MaxValue
    public float MaxValue {
        get {
            float max = -1;
            foreach (var paramiter in paramiters) {
                foreach (var detail in paramiter.Value) {
                    if (detail.Value > max) {
                        max = detail.Value;
                    }
                }
            }
            return max;
        }
    }

    public float MaxValueByDetail(string detail) {
        float max = -1;
        foreach (var paramiter in paramiters) {
            if (paramiter.Value[detail] > max) {
                max = paramiter.Value[detail];
            }
        }
        return max;
    }
    #endregion

    #region Remove
    public bool Remove(string paramiter, string detail) {
        return paramiters[paramiter].Remove(detail);
    }

    public bool Remove(string paramiter) {
        return paramiters.Remove(paramiter);
    }

    public void Clear() {
        paramiters.Clear();
    }
    #endregion
}

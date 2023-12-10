// Ecology
// 0: Terrain
// 1: Climat
// 2: Flora
// 3: Fauna



//S_Civilization civ = new();
//civ.Population = 1000;
//civ.TakenFood = 100;
//civ.Set("ProdMode", "Primitive communism", 100);
//civ.Set("ProdMode", "Slave society", 0);

//civ.Set("EcoCulture", "Hunters", 100);
//civ.Set("EcoCulture", "Farmers", 0);

//civ.Set("Government", "Leaderism", 100);
//civ.Set("Government", "Monarchy", 0);

using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.CSV;
using UnityEngine;
using UnityEngine.InputSystem;
using WorldMapStrategyKit;

[System.Serializable]
public class S_Region {
    [SerializeField] private Color color;
    // public int EventChanceIndex;
    // public int[] Events;
    [SerializeField] private List<string> neighbours = new();
    [SerializeField] private SerializedDictionary<string, S_Paramiter> ecology = new();
    [SerializeField] private SerializedDictionary<string, S_Civilization> civilizations = new();

    public Color Color { get => color; set => color = value; }

    public string[] EcologyKeys() => ecology.Keys.ToArray();
    public string[] EcologyDetailsKeys(string paramiter) => ecology[paramiter].Keys;
    public float[] EcologyDetailsValues(string paramiter) => ecology[paramiter].Values;

    public string[] CivilizationKeys() => civilizations.Keys.ToArray();
    public string[] CivilizationParamitersKeys(string civilization) => civilizations[civilization].ParamitersKeys;
    public string[] CivilizationDetailsKeys(string civilization, string paramiter) => civilizations[civilization].DetailsKeys(paramiter);
    public float[] CivilizationDetailsValues(string civilization, string paramiter) => civilizations[civilization].DetailsValues(paramiter);
    public int[] ArrayPopulation() {
        Dictionary<string, int> dict = new();
        foreach(var val in civilizations) {
            dict[val.Key] = val.Value.Population;
        }
        return dict.Values.ToArray();
    }

    #region Get
    //public S_Paramiter GetEcology(string paramiter) {
    //    if (ecology == null) return null;
    //    if (!ecology.ContainsKey(paramiter)) return null;
    //    return ecology[paramiter];
    //}
    public float GetEcology(string paramiter, string detail) {
        if (ecology == null) return -1f;
        if (!ecology.ContainsKey(paramiter)) return -1f;
        return ecology[paramiter].Get(detail);
    }
    //public S_Civilization GetCivilization(string civilization) {
    //    if (civilizations == null) return null;
    //    if (!civilizations.ContainsKey(civilization)) return null;
    //    return civilizations[civilization];
    //}
    //public SerializedDictionary<string, float> GetCivilization(string civilization, string paramiter) {
    //    if (civilizations == null) return null;
    //    if (!civilizations.ContainsKey(civilization)) return null;
    //    return civilizations[civilization].Get(paramiter);
    //}
    public float GetCivilization(string civilization, string paramiter, string detail) {
        if (civilizations == null) return -1f;
        if (!civilizations.ContainsKey(civilization)) return -1f;
        return civilizations[civilization].Get(paramiter, detail);
    }
    #endregion

    #region Set
    #region Region
    public void Set(S_Region value) {
        color = value.color;
        neighbours = new();
        for (int i = 0; i < value.neighbours.Count; ++i) {
            neighbours.Add(value.neighbours[i]);
        }
        Set(value.ecology);
        Set(value.civilizations);
    }
    #endregion
    #region Ecology
    public void Set(SerializedDictionary<string, S_Paramiter> values) {
        foreach (var value in values) {
            if (!ecology.ContainsKey(value.Key)) ecology[value.Key] = new();
            ecology[value.Key].Set(value.Value);
        }
    }

    public void Set(string eco, S_Paramiter value) {
        if (ecology == null) ecology = new();
        if (!ecology.ContainsKey(eco)) ecology[eco] = new();

        ecology[eco].Set(value);
    }

    public void Set(string eco, SerializedDictionary<string, float> values) {
        if (ecology == null) ecology = new();
        if (!ecology.ContainsKey(eco)) ecology[eco] = new();

        ecology[eco].Set(values);
    }

    public void Set(string eco, string detail, float value) {
        if (!ecology.ContainsKey(eco)) ecology[eco] = new();

        ecology[eco].Set(detail, value);
    }
    #endregion

    #region Civilization
    public void Set(SerializedDictionary<string, S_Civilization> values) {
        foreach (var value in values) {
            if (!civilizations.ContainsKey(value.Key)) civilizations[value.Key] = new();
            civilizations[value.Key].Set(value.Value);
        }
    }

    public void Set(string civilization, S_Civilization value) {
        if (!civilizations.ContainsKey(civilization)) civilizations[civilization] = new();
        civilizations[civilization].Set(value);
    }

    public void Set(string civilization, SerializedDictionary<string, SerializedDictionary<string, float>> values) {
        if (!civilizations.ContainsKey(civilization)) civilizations[civilization] = new();
        civilizations[civilization].Set(values);
    }

    public void Set(string civilization, string paramiter, SerializedDictionary<string, float> values) {
        if (!civilizations.ContainsKey(civilization)) civilizations[civilization] = new();
        civilizations[civilization].Set(paramiter, values);
    }

    public void Set(string civilization, string paramiter, string detail, float value) {
        if (!civilizations.ContainsKey(civilization)) civilizations[civilization] = new();
        civilizations[civilization].Set(paramiter, detail, value);
    }
    #endregion
    #endregion

    public void AddNeighvour(string neighbour) {
        neighbours.Add(neighbour);
    }

    public float Eat {
        get {
            float farmers = AllCivValueByDetail("Farmers");
            float hunters = AllCivValueByDetail("Hunters");
            float flora = GetEcology("Flora", "Flora");
            float fauna = GetEcology("Fauna", "Fauna");
            return farmers.Proportion(hunters) > 50 ? flora : fauna;
        }
        set {
            float farmers = AllCivValueByDetail("Farmers");
            float hunters = AllCivValueByDetail("Hunters");
            if (farmers.Proportion(hunters) > 50) Set("Flora", "Flora", value);
            else                                  Set("Fauna", "Fauna", value);
        }
    }

    #region AllValue
    public int AllPopulations {
        get {
            int all = 0;
            foreach (var civ in civilizations) {
                all += civ.Value.Population;
            }
            return all;
        }
    }

    public float AllTakenFoods {
        get {
            float all = 0;
            foreach (var civ in civilizations) {
                all += civ.Value.TakenFood;
            }
            return all;
        }
    }

    public float AllEcoValueByDetail(string detail) {
        float all = 0f;
        foreach (var paramiter in ecology) {
            all += paramiter.Value.Get(detail);
        }
        return all;
    }

    public float AllCivValueByDetail(string detail) {
        float all = 0f;
        foreach (var civilization in civilizations) {
            all += civilization.Value.AllValuesByDetail(detail);
        }
        return all;
    }
    #endregion

    #region MaxKey
    public string MaxPopulationsKey {
        get {
            string key = "";
            int max = -1;
            foreach (var civ in civilizations) {
                if (civ.Value.Population > max) {
                    max = civ.Value.Population;
                    key = civ.Key;
                }
            }
            return key;
        }
    }

    public string MaxTakenFoodKey {
        get {
            string key = "";
            float max = -1;
            foreach (var civ in civilizations) {
                if (civ.Value.Population > max) {
                    max = civ.Value.TakenFood;
                    key = civ.Key;
                }
            }
            return key;
        }
    }

    public string MaxEcoKeyByParamiter(string paramiter) {
        return ecology[paramiter].MaxKey;
    }

    public string MaxCivKeyByParamiter(string paramiter) {
        float max = -1f;
        string key = "";
        foreach (var civilization in civilizations) {
            if (civilization.Value.MaxValue > max) {
                max = civilization.Value.MaxValue;
                key = civilization.Key;
            }
        }
        return key;
    }

    public string MaxEcoKeyByDetail(string detail) {
        float max = -1f;
        string key = "";
        foreach (var paramiter in ecology) {
            if (paramiter.Value.Get(detail) > max) {
                max = paramiter.Value.Get(detail);
                key = paramiter.Key;
            }
        }
        return key;
    }

    public string MaxCivKeyByDetail(string detail) {
        float max = -1f;
        string key = "";
        foreach (var civilization in civilizations) {
            if (civilization.Value.MaxValueByDetail(detail) > max) {
                max = civilization.Value.MaxValueByDetail(detail);
                key = civilization.Key;
            }
        }
        return key;
    }
    #endregion

    #region MaxValue
    public int MaxPopulationsValue {
        get {
            int max = -1;
            foreach (var civ in civilizations) {
                if (civ.Value.Population > max) {
                    max = civ.Value.Population;
                }
            }
            return max;
        }
    }

    public float MaxTakenFoodValue {
        get {
            float max = -1f;
            foreach (var civ in civilizations) {
                if (civ.Value.Population > max) {
                    max = civ.Value.TakenFood;
                }
            }
            return max;
        }
    }

    public float MaxEcoValueByParamiter(string paramiter) {
        return ecology[paramiter].MaxValue;
    }

    public float MaxCivValueByParamiter(string paramiter) {
        float max = -1f;
        foreach (var civilization in civilizations) {
            if (civilization.Value.MaxValue > max) {
                max = civilization.Value.MaxValue;
            }
        }
        return max;
    }

    public float MaxEcoValueByDetail(string detail) {
        float max = -1f;
        foreach (var paramiter in ecology) {
            if (paramiter.Value.Get(detail) > max) {
                max = paramiter.Value.Get(detail);
            }
        }
        return max;
    }

    public float MaxCivValueByDetail(string detail) {
        float max = -1;
        foreach (var civilization in civilizations) {
            if (civilization.Value.MaxValueByDetail(detail) > max) {
                max = civilization.Value.MaxValueByDetail(detail);
            }
        }
        return max;
    }
    #endregion

    #region Remove
    public bool RemoveCivilization(string civilization) {
        return civilizations.Remove(civilization);
    }
    public bool RemoveNeighbour(string neighbour) {
        return neighbours.Remove(neighbour);
    }
    public void Clear() {
        ecology.Clear();
        civilizations.Clear();
    }
    #endregion
}
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

// Example: GetRegion => IndexRegion => GetCivilization => IndexCivilization => GetParamiter => IndexParamiter => IndexDetail
[System.Serializable]
public class S_Map {
    [SerializeField] public Dictionary<string, Dictionary<string, int>> civilizations = new();
    [SerializeField] public SerializedDictionary<string, S_Region> regions = new();

    public int CountRegionsInCivilization(string civilization) => civilizations[civilization].Count;

    #region Set
    #region Region
    public void Set(string region, S_Region value) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(value);
    }
    #endregion

    #region Ecology
    public void Set(string region, SerializedDictionary<string, S_Paramiter> values) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(values);
    }

    public void Set(string region, string eco, S_Paramiter value) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(eco, value);
    }

    public void Set(string region, string eco, SerializedDictionary<string, float> values) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(eco, values);
    }

    public void Set(string region, string eco, string detail, float value) {
        if (!regions.ContainsKey(region)) regions = new();
        regions[region].Set(eco, detail, value);
    }
    #endregion

    #region Civilization
    
    public void Set(string region, SerializedDictionary<string, S_Civilization> values) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(values);
    }
    
    public void Set(string region, string civilization, S_Civilization value) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(civilization, value);
    }
    
    public void Set(string region, string civilization, SerializedDictionary<string, SerializedDictionary<string, float>> values) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(civilization, values);
    }
    
    public void Set(string region, string civilization, string paramiter, SerializedDictionary<string, float> values) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(civilization, paramiter, values);
    }

    public void Set(string region, string civilization, string paramiter, string detail, float value) {
        if (!regions.ContainsKey(region)) regions[region] = new();
        regions[region].Set(civilization, paramiter, detail, value);
    }
    #endregion
    #endregion

    public int AllPopulations {
        get {
            int all = 0;
            foreach (var region in regions) {
                all += region.Value.AllPopulations;
            }
            return all;
        }
    }

    public string MaxEcologyKey(string paramiter) {
        string key = "";
        float max = -1f;
        foreach (var region in regions) {
            if (region.Value.MaxEcoValueByParamiter(paramiter) > max) {
                max = region.Value.MaxEcoValueByParamiter(paramiter);
                key = region.Key;
            }
        }
        return key;
    }

    public string MaxCivilizationKey(string paramiter) {
        float max = -1f;
        string key = "";
        foreach (var region in regions) {
            if (region.Value.MaxCivValueByParamiter(paramiter) > max) {
                max = region.Value.MaxCivValueByParamiter(paramiter);
                key = region.Key;
            }
        }
        return key;
    }

    public int MaxPopulationValue {
        get {
            int max = 0;
            foreach (var region in regions) {
                if (max < region.Value.AllPopulations) {
                    max = region.Value.AllPopulations;
                }
            }
            return max;
        }
    }

    public float MaxEcologyValue(string paramiter) {
        float max = -1f;
        foreach (var region in regions) {
            if (region.Value.MaxEcoValueByParamiter(paramiter) > max) {
                max = region.Value.MaxEcoValueByParamiter(paramiter);
            }
        }
        return max;
    }

    public float MaxCivilizationValue(string paramiter) {
        float max = -1f;
        foreach (var region in regions) {
            if (region.Value.MaxCivValueByParamiter(paramiter) > max) {
                max = region.Value.MaxCivValueByParamiter(paramiter);
            }
        }
        return max;
    }

    #region Remove
    public bool RemoveCivilizatin(string civilization) {
        return civilizations.Remove(civilization);
    }
    public bool RemoveRegionInCivilizatin(string civilization, string region) {
        return civilizations[civilization].Remove(region);
    }
    public bool RemoveRegion(string region) {
        return regions.Remove(region);
    }
    public void Clear() {
        civilizations.Clear();
        regions.Clear();
    }
    #endregion

    //public int AllEcologyVlaues(int paramiterIndex, int detailIndex) {
    //    int all = 0;
    //    for (int i = 0; i < Regions.Length; ++i) {
    //        all += Regions[i].Ecology[paramiterIndex].details[detailIndex].Value;
    //    }
    //    return all;
    //}

    //public int AllCivilizationVlaues(int paramiterIndex, int detailIndex) {
    //    int all = 0;
    //    for (int i = 0; i < Regions.Length; ++i) {
    //        all += Regions[i].AllCivilizationVlaues(paramiterIndex, detailIndex);
    //    }
    //    return all;
    //}
}

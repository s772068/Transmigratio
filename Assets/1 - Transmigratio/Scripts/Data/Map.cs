using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Класс для работы с картой
/// </summary>
[System.Serializable]
public class Map {
    public List<TM_Region> AllRegions;

    public void Init() {
        AllRegions = new List<TM_Region>();
        AllRegions.Clear();

        string json;

        TextAsset textAsset = Resources.Load<TextAsset>("RegionStartParams");
        json = textAsset.text;

        AllRegions = JsonConvert.DeserializeObject<List<TM_Region>>(json);
    }

    public TM_Region GetRegionBywmskId(int WMSKId) {
        foreach (TM_Region region in AllRegions) {
            if (region.Id == WMSKId) return region;
        }
        return null;
    }
}
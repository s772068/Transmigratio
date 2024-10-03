using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System;

/// <summary>
/// ����� ��� ������ � ������
/// </summary>
[System.Serializable]
public class Map {
    public Texture2D borderTexture;
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

    public void SelectRegion(int index) {
        _selectedCountry = index;
        WMSK.ToggleCountrySurface(index, true, new Color(1, 0.92f, 0.16f, 0.25f));
        WMSK.ToggleCountryOutline(index, true, borderTexture, 2f, Color.yellow, animationSpeed: 10);
    }

    public void UnselectRegion() {
        WMSK.ToggleCountrySurface(_selectedCountry, true, Color.clear);
        WMSK.ToggleCountryOutline(_selectedCountry, true, borderWidth: 0.2f, tintColor: WMSK.frontiersColor);
    }
}
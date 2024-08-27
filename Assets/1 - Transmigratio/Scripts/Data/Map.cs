using System.Collections.Generic;
using WorldMapStrategyKit;
using Newtonsoft.Json;
using UnityEngine;
using System;

/// <summary>
/// Класс для работы с картой. Через него же взаимодействие с wmsk
/// </summary>
[System.Serializable]
public class Map {
    public Texture2D borderTexture;
    public List<TM_Region> AllRegions;

    [HideInInspector] public WMSK WMSK;

    private int _selectedCountry;

    public void Init() {
        WMSK = WMSK.instance;

        AllRegions = new List<TM_Region>();
        AllRegions.Clear();

        string json;        //сюда будет записываться json

        TextAsset textAsset = Resources.Load<TextAsset>("RegionStartParams");
        json = textAsset.text;
/*
#if UNITY_EDITOR || UNITY_IOS
        path = Application.streamingAssetsPath + "/Config/RegionStartParams.json";
        Transmigratio.AddingDebugText.Invoke("Unity editor or iOS\n");
        json = File.ReadAllText(path);
#elif UNITY_ANDROID
        path = "jar:file://" + Application.dataPath + "!/assets/Config/RegionStartParams.json";
        WWW reader = new WWW(path);
        while (!reader.isDone) { }
        json = reader.text;
        Transmigratio.AddingDebugText.Invoke("Android\n");
#endif
*/

        AllRegions = JsonConvert.DeserializeObject<List<TM_Region>>(json);

        //allRegions = JsonUtility.FromJson<List<TM_Region>>(json);

        foreach (TM_Region region in AllRegions ) {
            WMSK.ToggleCountrySurface(region.Id, true, Color.clear);
            //region.Init();
        }
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
        WMSK.ToggleCountrySurface(_selectedCountry, false, new Color(1, 0.92f, 0.16f, 0.25f));
        WMSK.ToggleCountryOutline(_selectedCountry, false, borderTexture, 0.5f, Color.yellow);
    }
}
using System.Collections.Generic;
using WorldMapStrategyKit;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Класс для работы с картой. Через него же взаимодействие с wmsk
/// </summary>
[System.Serializable]
public class Map {
    public List<TM_Region> allRegions;

    [HideInInspector] public WMSK wmsk;

    public void Init() {
        wmsk = new WMSK();
        wmsk = WMSK.instance;

        wmsk.OnMarkerMouseDown = GameEvents.onMarkerMouseDown.Invoke;
        wmsk.OnMarkerMouseEnter = GameEvents.onMarkerEnter.Invoke;
        wmsk.OnMarkerMouseExit = GameEvents.onMarkerExit.Invoke;

        allRegions = new List<TM_Region>();
        allRegions.Clear();

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

        allRegions = JsonConvert.DeserializeObject<List<TM_Region>>(json);

        //allRegions = JsonUtility.FromJson<List<TM_Region>>(json);

        foreach (TM_Region region in allRegions ) { region.Init(); }
    }

    public TM_Region GetRegionBywmskId(int WMSKId) {
        foreach (TM_Region region in allRegions) {
            if (region.id == WMSKId) return region;
        }
        return null;
    }
}
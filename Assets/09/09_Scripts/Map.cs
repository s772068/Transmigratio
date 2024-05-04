using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Net;
using Newtonsoft.Json;
 
using WorldMapStrategyKit;
using AYellowpaper.SerializedCollections;
/// <summary>
/// Класс для работы с картой. Через него же взаимодействие с wmsk
/// </summary>
[System.Serializable]
public class Map
{
    public WMSK wmsk;
    public List<TM_Region> allRegions;


    public void Init()
    {
        Transmigratio.AddingDebugText.Invoke("Map init begin \n");
        wmsk = new WMSK();
        wmsk = WMSK.instance;

        allRegions = new List<TM_Region>();
        allRegions.Clear();

        string path;        //путь до json
        string json;        //сюда будет записываться json
         
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

        string s = json.Substring(0, 50);
        Transmigratio.AddingDebugText.Invoke("json: " + s + "\n");

        // allRegions = JsonConvert.DeserializeObject<List<TM_Region>>(json);

        //allRegions = (List<TM_Region>) 

        //allRegions = JsonUtility.FromJson<List<TM_Region>>(json);

        Transmigratio.AddingDebugText.Invoke("Deserialisation complete \n");

        foreach (TM_Region region in allRegions ) { region.Init(); }

        Transmigratio.AddingDebugText.Invoke("region init complete \n");

        Debug.Log("Map init");
        Transmigratio.AddingDebugText.Invoke("Map init end \n");
    }
    public TM_Region GetRegionBywmskId(int WMSKId)
    {
        foreach (TM_Region region in allRegions)
        {
            if (region.id == WMSKId) return region;
        }
        return null;
    }

    public void RefreshMap()
    {
        foreach(TM_Region region in allRegions) 
        {
            region.RefreshRegion();
        }
    }
}
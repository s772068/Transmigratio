//using System;
//using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using WorldMapStrategyKit;
using System.IO;
using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using UnityEngine.UI;
//using UnityEngine.Networking;
//using UnityEditor.PackageManager;
//using UnityEngine.InputSystem;
//using UnityEngine.UIElements;
/// <summary>
/// Создаем объект WMSK, создаем объекты-регионы из их инитов
/// </summary>
[System.Serializable]
public class Map
{
    public WMSK wmsk;

    [Header("Выбираемые регионы")]
    public TM_Region activeRegion;      //выбранный регион
    public TM_Region previousRegion;    //предыдущий выбранный регион
    public TM_Region targetRegion;      //целевой регион

    [Header("List of All Regions")]
    public List<TM_Region> allRegions;

    [Header("Оформление")]
    public CountryDecorator cd_noDec;
    public CountryDecorator cd_active;
    public CountryDecorator cd_lastClicked;
    public CountryDecorator cd_filterSelected;
    public Color pathLineCol;
    public LineMarkerAnimator lma;

    public GameObject pathPrefab;

    [Header("For the showcases")]
    public CountryDecorator cd_yellow;
    public CountryDecorator cd_red;
    public CountryDecorator cd_violet;
    public CountryDecorator cd_orange;
    public CountryDecorator cd_grey;
    public CountryDecorator cd_green;
    public CountryDecorator cd_cian;
    public CountryDecorator cd_blue;
    public List<CountryDecorator> decorators = new List<CountryDecorator>();

    public void Init()
    {
        // wmsk = new WMSK();
        // wmsk = WMSK.instance;

        wmsk.OnCountryClick += Map_OnCountryClick;
        string json;
        string path;
        //string path = Application.streamingAssetsPath + "/Config/RegionStartParams.json";
        if (Application.isMobilePlatform)
        {
            

            path = "jar:file://" + Application.dataPath + "!/assets" + "/Config/RegionStartParams.json";
        }
        else
        {
            path = Application.streamingAssetsPath + "/Config/RegionStartParams.json";
            
        }
        //string json = File.ReadAllText(Application.streamingAssetsPath + "/Config/RegionStartParams.json");
        json = File.ReadAllText(path);
        

        allRegions = new List<TM_Region>();
        allRegions.Clear();
        
        allRegions = JsonConvert.DeserializeObject<List<TM_Region>>(json);
        
        foreach (TM_Region region in allRegions) 
        { 
            //t.text += " | " + region.id + " | ";
            region.Init();
            region.RefreshRegion();
        }
        activeRegion = GetTMRegionByIdWMSK(15);
        Debug.Log("Map initialized");
    }
    public void CalcGlobalVars()
    {
        foreach (TM_Region region in allRegions) 
        {
            region.RefreshRegion();
        }
    }

    public string MakeJSONOfTurn()
    {
        return JsonConvert.SerializeObject(allRegions);
    }
    
    private void Map_OnCountryClick(int countryIndex, int regionIndex, int buttonIndex) //событие при клике на страну
    {
        //previousRegion = activeRegion;
        //ChangeRegionColor(previousRegion, cd_noDec);
        activeRegion = GetTMRegionByIdWMSK(countryIndex);
        ChangeRegionColor(activeRegion, cd_active);
    }
    public void ChangeRegionColor(TM_Region region, CountryDecorator cd)
    {;
        wmsk.decorator.SetCountryDecorator(0, wmsk.countries[region.id_WMSK].name, cd);
    }

    public TM_Region GetTMRegionByIdWMSK(int id_WMSK)
    {
        foreach (TM_Region region in allRegions) 
        { 
            if (region.id_WMSK == id_WMSK) return region; 
        }
        return previousRegion;
    }

    public void DrawPathOfMigration(TM_Region from, TM_Region to, GameObject go)
    {
        Vector3 posFrom = wmsk.countries[from.id_WMSK].centroid;
        Vector3 posTo = wmsk.countries[to.id_WMSK].centroid;

        lma = wmsk.AddLine(new Vector2[]
            {
                posFrom,
                posTo
            }, pathLineCol, 0f, 0.15f);
        lma.lineWidth = 2.5f;
        lma.dashInterval = 0.005f;
        lma.dashAnimationDuration = 1.5f;
        lma.drawingDuration = 0f;
        lma.autoFadeAfter = 0f;
        lma.fadeOutDuration = 1000f;
        //lma.arcElevation = 10f;
        lma.name = "pathline";

        Vector3 pos = Vector3.zero;
        pos.x = (posFrom.x + posTo.x) / 2;
        pos.y = (posFrom.y + posTo.y) / 2;
        wmsk.AddMarker2DSprite(go, pos, 0.02f, enableEvents: true, autoScale: false);

    }
    public TM_Region RandomPopulated()
    {
        List<TM_Region> popRegions = new List<TM_Region>();
        popRegions.Clear();
        foreach (TM_Region region in allRegions) 
        {
            if (region.isPopulated) popRegions.Add(region);
        }
        var rand = new System.Random();
        return popRegions[rand.Next(popRegions.Count)];
    }
    public TM_Region RandomPopulatedNeighbour(TM_Region originalRegion)
    {
        return GetTMRegionByIdWMSK(6);
    }

    public void SetDecorators()
    {
        
    }
}

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// цвета для диаграмм и фильтров
/// </summary>

[System.Serializable]
public class PieColor
{
    public Dictionary<string, Color32> colorDict;
    Dictionary<string, string> fromJson;
    public List<Color32> colors;

    public Color32 TM_white = new Color32(255, 255, 255, 255);
    public Color32 TM_orange = new Color32(255, 162, 0, 255);
    public Color32 TM_green = new Color32(44, 174, 0, 255);
    public Color32 TM_blue = new Color32(44, 174, 255, 255);
    public Color32 TM_cyan = new Color32(108, 255, 255, 255);
    public Color32 TM_red = new Color32(211, 20, 20, 255);
    public Color32 TM_lightgreen = new Color32(66, 236, 100, 255);
    public Color32 TM_yellow = new Color32(244, 224, 43, 255);
    public Color32 TM_brown = new Color32(213, 106, 0, 255);
    public Color32 TM_grey = new Color32(120, 120, 95, 255);

    public void Init2()
    {
        colors = new List<Color32>();
        colors.Add(TM_white);
        colors.Add(TM_orange);
        colors.Add(TM_green);
        colors.Add(TM_blue);
        colors.Add(TM_cyan);
        colors.Add(TM_red);
        colors.Add(TM_lightgreen);
        colors.Add(TM_yellow);
        colors.Add(TM_brown);
        colors.Add(TM_grey);
    }

    public void Init()
    {
        string json = File.ReadAllText(Application.streamingAssetsPath + "/Config/pieColors.json");
        fromJson = new Dictionary<string, string>();
        fromJson.Clear();
        fromJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        colorDict = new Dictionary<string, Color32>();
        colorDict.Clear();
        foreach (KeyValuePair<string, string> entry in fromJson)
        {
            colorDict.Add(entry.Key, GetColorByName(entry.Value));
        }

    }
    private Color32 GetColorByName(string key)
    {
        switch (key)
        {
            case "TM_orange":
                return TM_orange;
            case "TM_green":
                return TM_green;
            case "TM_blue":
                return TM_blue;
            case "TM_cyan":
                return TM_cyan;
            case "TM_red":
                return TM_red;
            case "TM_lightgreen":
                return TM_lightgreen;
            case "TM_yellow":
                return TM_yellow;
            case "TM_brown":
                return TM_brown;
            case "TM_grey":
                return TM_grey;
        }
        return TM_white;
    }
    public Color32 GetPieColorByKey(string key)
    {
        foreach (KeyValuePair<string, Color32> entry in colorDict)
        {
            if (entry.Key == key) return entry.Value;
        }
        return TM_white;
    }
}
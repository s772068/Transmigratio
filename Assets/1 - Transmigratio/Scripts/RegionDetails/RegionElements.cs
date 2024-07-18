using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;

public class RegionElements : MonoBehaviour {
    [SerializeField] private RegionElement element;
    [SerializeField] private Transform content;
    [SerializeField] private SerializedDictionary<string, Sprite> pictograms;

    private string activeKey;
    private Dictionary<string, RegionElement> dic = new();

    public Action<string> onClick;

    public void ClickCivTab() {
        ClearElements();
        foreach (var element in GameSettings.CivDetails) {
            CreateElement(element);
        }
    }

    public void ClickRegionTab() {
        ClearElements();
        foreach (var element in GameSettings.RegionDetails) {
            CreateElement(element);
        }
    }

    public void CreateElement(string title) {
        var region = Instantiate(element, content);
        region.Click = SelectElement;
        region.Pictogram = pictograms[title];
        region.Title = title;
        dic[title] = region;
    }

    public void ClearElements() {
        foreach(var pair in dic) {
            pair.Value.Destroy();
        }
        activeKey = "";
        dic.Clear();
    }

    public void SelectElement(string key) {
        if (activeKey != "") dic[activeKey].Deactivate();
        activeKey = key;
        dic[key].Activate();
        onClick?.Invoke(key);

    }
}

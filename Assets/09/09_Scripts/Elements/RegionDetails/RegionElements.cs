using System.Collections.Generic;
using UnityEngine;
using System;

public class RegionElements : MonoBehaviour {
    [SerializeField] private Transform content;

    private Dictionary<string, RegionElement> dic = new();

    [HideInInspector] public string activeKey;

    public Action<string> onClick;

    public void ClickCivTab() {
        ClearElements();
        foreach (var element in GameSettings.civDetails) {
            CreateElement(element);
        }
    }

    public void ClickRegionTab() {
        ClearElements();
        foreach (var element in GameSettings.regionDetails) {
            CreateElement(element);
        }
    }

    public void CreateElement(string title) {
        PrefabsLoader.Load(out RegionElement newVal, "RegionElement", content);
        newVal.onClick = OnClick;
        newVal.Title = title;
        dic[title] = newVal;
    }

    public void ClearElements() {
        foreach(var pair in dic) {
            pair.Value.Destroy();
        }
        activeKey = "";
        dic.Clear();
    }

    private void OnClick(string key) {
        if (activeKey != "") dic[activeKey].Deactivate();
        activeKey = key;
        dic[key].Activate();
        onClick?.Invoke(key);
    }
}

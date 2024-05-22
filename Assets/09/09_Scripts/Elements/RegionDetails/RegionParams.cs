using System.Collections.Generic;
using UnityEngine;
using System;

public class RegionParams : MonoBehaviour {
    [SerializeField] private Transform content;

    private List<ParamWrap> list = new();

    public Action<string> onClick;

    public void CreateParam(string title, float value) {
        ParamWrap newVal = PrefabsLoader.Load<ParamWrap>("ParamWrap", content);
        newVal.onClick = OnClick;
        newVal.Title = title;
        newVal.Value = value;
        list.Add(newVal);
    }

    public void ClearParams() {
        for (int i = 0; i < list.Count; ++i) {
            list[i].Destroy();
        }
        list.Clear();
    }

    private void OnClick(string key) => onClick?.Invoke(key);
}

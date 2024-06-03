using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class RegionParams : MonoBehaviour {
    [SerializeField] private TMP_Text title;
    [SerializeField] private Transform content;

    private Dictionary<string, ParamWrap> paramiters = new();

    public string Title { set => title.text = value; }

    public Action<string> onClick;

    public void SetParamiter(string title, float value) {
        if (!paramiters.ContainsKey(title)) {
            PrefabsLoader.Load(out ParamWrap newVal, content);
            newVal.onClick = OnClick;
            newVal.Title = title;
            paramiters[title] = newVal;
        }
        paramiters[title].Value = value;
    }

    public void ClearParams() {
        foreach (var pair in paramiters) {
            pair.Value.Destroy();
        }
        paramiters.Clear();
    }

    private void OnClick(string key) => onClick?.Invoke(key);
}

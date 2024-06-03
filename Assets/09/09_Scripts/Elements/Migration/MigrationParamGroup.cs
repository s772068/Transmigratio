using System.Collections.Generic;
using UnityEngine;
using System;

public class MigrationParamGroup : MonoBehaviour {
    [SerializeField] private Transform content;

    private Dictionary<string, MigrationParamElement> paramiters = new();
    public Action onClickInfo;

    public void AddParamiter(string title, int value) {
        if (!paramiters.ContainsKey(title)) {
            PrefabsLoader.Load(out MigrationParamElement param, content);
            param.Title = title;
            paramiters[title] = param;
        }
        paramiters[title].Value = value;
    }

    public void ClearParamiters(string title, int value) {
        foreach(var pair in paramiters) {
            pair.Value.Destroy();
        }
        paramiters.Clear();
    }
}

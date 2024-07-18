using System.Collections.Generic;
using UnityEngine;
using System;

public class MigrationParamGroup : MonoBehaviour {
    [SerializeField] private MigrationParamElement element;
    [SerializeField] private Transform content;

    private Dictionary<string, MigrationParamElement> paramiters = new();
    public Action onClickInfo;

    public void AddParamiter(string title, int value) {
        if (!paramiters.ContainsKey(title)) {
            var paramiter = Instantiate(element, content);
            // param.Title = title;
            paramiters[title] = paramiter;
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

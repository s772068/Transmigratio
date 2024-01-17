using System.Collections.Generic;
using UnityEngine;
using System;

public class GUIE_ParamsGroup : MonoBehaviour {
    [SerializeField] private GUIE_NumParamiter numParamiterPrefab;
    [SerializeField] private GUIE_StringParamiter stringParamiterPrefab;
    
    private List<GUIE_BaseParamiter> paramiters = new();
    private string _name;

    public Action<string, string> OnClick;
    public string Name { private get; set; }

    public void Click(string paramiterName) => OnClick?.Invoke(Name, paramiterName);
    
    public void AddNumParamiter(Paramiter paramiter, bool isShowLabel, bool useProgressBar) {
        GUIE_NumParamiter element = Instantiate(numParamiterPrefab, transform);
        element.Build(paramiter, isShowLabel, useProgressBar);
        element.OnClick += Click;
        paramiters.Add(element);
    }

    public void AddStringParamiter(Paramiter paramiter, bool isShowLabel) {
        GUIE_StringParamiter element = Instantiate(stringParamiterPrefab, transform);
        element.Build(paramiter, isShowLabel);
        element.OnClick += Click;
        paramiters.Add(element);
    }

    public void ClearParamiters() {
        for (int i = 0; i < paramiters.Count; ++i)
            paramiters[i].DestroyGO();
        paramiters.Clear();
    }
}

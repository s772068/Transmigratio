using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Localization.Settings;

public class GUIE_BaseParamiter : MonoBehaviour {
    [SerializeField] private Image pictogram;
    [SerializeField] private Text label;
    [SerializeField] private protected Text val;
    
    private string _name;

    public Action<string> OnClick;

    public Sprite Pictogram { set => pictogram.sprite = value; }

    public void SetLabel(string value, bool isShowLabel) {
        label.gameObject.SetActive(isShowLabel);
        _name = value;

        if (!isShowLabel) return;
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Paramiters", value);
        if (op.IsDone) label.text = op.Result;
        else op.Completed += (op) => label.text = op.Result;
    }

    public void Build(Paramiter paramiter, bool isShowLabel) {
        pictogram.sprite = paramiter.Pictogram;
        SetLabel(paramiter.Name, isShowLabel);
    }

    public void Click() => OnClick?.Invoke(_name);

    public void DestroyGO() {
        Destroy(gameObject);
    }
}

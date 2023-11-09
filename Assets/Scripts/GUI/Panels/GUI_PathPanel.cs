using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUI_PathPanel : GUI_BasePanel {
    [Header("Text")]
    [SerializeField] private Text label;
    [SerializeField] private Text pathTxt;
    [SerializeField] private Text description;
    [SerializeField] private Text breakTxt;
    [SerializeField] private Text closeTxt;
    [Header("Button")]
    [SerializeField] private Button breakBtn;
    [SerializeField] private Button closeBtn;
    [Header("Other")]
    [SerializeField] private Image icon;
    [SerializeField] private GUI_Param paramiter;
    [SerializeField] private List<Sprite> iconSprites;

    public Action OnBreak;
    public Action OnClose;

    public string Label { set => label.text = value; }
    public string Path { set => pathTxt.text = value; }
    public string Description { set => description.text = value; }
    public Sprite Icon { set => icon.sprite = value; }
    public int ParamiterValue { set => paramiter.Value = value; }
    public string BreakString { set => breakTxt.text = value; }
    public string CloseString { set => closeTxt.text = value; }

    public void Init(Data data) {
        Label = data.Label;
        Path = data.Path;
        Icon = iconSprites[data.IconIndex];
        Description = data.Description;
        BreakString = data.BreakString;
        CloseString = data.CloseString;
        paramiter.Init(data.Paramiter);
    }

    public void UpdatePanel(int value) {
        paramiter.Value = value;
    }

    public void Break() => OnBreak?.Invoke();

    public void Close() {
        OnClose?.Invoke();
        Destroy(gameObject);
    }

    [System.Serializable]
    public struct Data {
        public string Label;
        public string Path;
        public string Description;
        public string BreakString;
        public string CloseString;
        public int IconIndex;
        public GUI_Param.Data Paramiter;
    }
}

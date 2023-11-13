using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_MigrationPanel : GUI_BasePanel {
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
    [SerializeField] private GUI_Paramiter paramiter;
    [SerializeField] private List<Sprite> iconSprites;

    private float maxPopulation;
    private string pathFormat;
    private Vector3 scale = new Vector3(1, 1, 1);

    [HideInInspector] public int index;

    public Action OnBreak;
    public Action OnClose;

    public string Label { set => label.text = value; }
    public string Path { set => pathTxt.text = value; }
    public string Description { set => description.text = value; }
    public Sprite Icon { set => icon.sprite = value; }

    public float Population {
        set {
            paramiter.Fill = value / maxPopulation;
            paramiter.Value = (int)(scale.x * 100) + "%";
        }
    }

    public string BreakString { set => breakTxt.text = value; }
    public string CloseString { set => closeTxt.text = value; }

    public void Localization(SL_Migration migration, SL_System system) {
        Label = migration.Label;
        pathFormat = migration.Path;
        Description = migration.Description;
        BreakString = system.Break;
        CloseString = system.Close;
        paramiter.Label = migration.Paramiter.Label;
    }

    public void Init(S_Migration data, string from, string to) {
        Icon = iconSprites[data.IconIndex];
        Path = string.Format(pathFormat, from, to);
        maxPopulation = data.MaxPopulation;
        Population = data.Population;
    }

    public void UpdatePanel(int population) {
        Population = population;
    }

    public void Break() => OnBreak?.Invoke();
    public void Close() => OnClose?.Invoke();
}

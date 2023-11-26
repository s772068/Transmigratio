using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIE_RegionDetails : MonoBehaviour {
    [SerializeField] private Text paramiterLabel;
    [SerializeField] private GUI_PieElement pieElement;
    [SerializeField] private GUI_Legend legend;
    [SerializeField] private Transform pieContent;
    [SerializeField] private Transform legendContent;

    [HideInInspector] public int regionIndex;
    [HideInInspector] public int groupIndex;
    [HideInInspector] public int civilizationIndex;
    [HideInInspector] public int paramiterIndex;
    [HideInInspector] public int detailIndex;

    private List<GUI_PieElement> pieElements = new();
    private List<GUI_Legend> legendElements = new();
    private float allValues = 0;
    private int[] values;

    public Action OnClickDetail;

    public SettingsController Settings { private get; set; }
    public MapController Map { private get; set; }
    
    private int[] CivilizationValues =>
        civilizationIndex < 0 ? paramiterIndex == Settings.Localization.Map.Civilization.Paramiters.Length - 1 ?
            Map.data.Regions[regionIndex].ArrayPopulation :
            Map.data.Regions[regionIndex].ArrayCivilizationParamiters(paramiterIndex) :
            Map.data.Regions[regionIndex].Civilizations[civilizationIndex].Paramiters[paramiterIndex].Values;

    public void Initialization() {
        Clear();
        switch (groupIndex) {
            case 0: InitEcology(); break;
            case 1: InitCivilization(); break;
        }
        Sorting();
    }

    public void Clear() {
        allValues = 0;
        for (int i = 0; i < legendElements.Count; ++i) {
            legendElements[i].DestroyGO();
            pieElements[i].DestroyGO();
        }
        legendElements.Clear();
        pieElements.Clear();
    }

    private void InitEcology() {
        SL_Paramiter<SL_Detail[]> paramitersStrings = Settings.Localization.Map.Ecology.Value[paramiterIndex];
        paramiterLabel.text = paramitersStrings.Name;
        values = Map.data.Regions[regionIndex].Ecology[paramiterIndex].Values;
        for (int i = 0; i < values.Length; ++i) {
            AddLegend(Settings.Theme.GetEcologyColor(paramiterIndex, i),
                      Settings.Localization.Map.Ecology.Value[paramiterIndex].Value[i].Name,
                      values[i]);
        }
    }

    private void InitCivilization() {
        SL_Paramiter<SL_Detail[]> paramitersStrings = Settings.Localization.Map.Civilization.Paramiters[paramiterIndex];
        paramiterLabel.text = paramitersStrings.Name;
        values = CivilizationValues;
        for (int i = 0; i < values.Length; ++i) {
            AddLegend(Settings.Theme.GetCivilizationColor(paramiterIndex, i),
                      Settings.Localization.Map.Civilization.Paramiters[paramiterIndex].Value[i].Name,
                      values[i]);
        }
    }

    private void AddLegend(Color color, string label, int value) {
        allValues += value;
        GUI_PieElement newPieElement = Instantiate(pieElement, pieContent);
        newPieElement.Color = color;
        pieElements.Add(newPieElement);
        GUI_Legend newLegend = Instantiate(legend, legendContent);
        newLegend.ImageColor = color;
        newLegend.Label = label;
        newLegend.Value = value;
        newLegend.Index = legendElements.Count;
        newLegend.OnClick = ClickDetail;
        legendElements.Add(newLegend);
    }

    public void UpdateDetails() {
        values = groupIndex == 1 ? CivilizationValues :
            Map.data.Regions[regionIndex].Ecology[paramiterIndex].Values;
        allValues = 0;
        for (int i = 0; i < values.Length; ++i) {
            legendElements[i].Value = values[i];
            allValues += values[i];
        }
        Sorting();
    }

    private void Sorting() {
        float lastAmount = 0;
        for (int i = values.Length - 1; i >= 0; --i) {
            lastAmount += allValues == 0f ? 1f : legendElements[i].Value / allValues;
            pieElements[i].FillAmount = lastAmount;
        }
    }

    private void ClickDetail(int detailIndex) {
        this.detailIndex = detailIndex;
        OnClickDetail?.Invoke();
    }
}

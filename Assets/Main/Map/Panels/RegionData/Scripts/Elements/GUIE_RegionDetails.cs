using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIE_RegionDetails : MonoBehaviour {
    [SerializeField] private Text paramiterLabel;
    [SerializeField] private GUI_PieElement pieElement;
    [SerializeField] private GUI_Legend legend;
    [SerializeField] private Transform pieContent;
    [SerializeField] private Transform legendsPanel;
    [SerializeField] private Transform legendContent;

    [HideInInspector] public int regionIndex;
    [HideInInspector] public int groupIndex;
    [HideInInspector] public float civID;
    [HideInInspector] public int paramiterIndex;
    [HideInInspector] public int detailIndex;

    private List<GUI_PieElement> pieElements = new();
    private List<GUI_Legend> legendElements = new();
    private float allValues = 0;
    private float[] values;

    public Action OnClickDetail;

    public SettingsController Settings { private get; set; }
    public MapController Map { private get; set; }

    private float[] CivilizationValues() {
        if (civID < 0) {
            if (paramiterIndex == Settings.Localization.Map.Civilization.Paramiters.Length - 1) {
                return Map.data.GetArrayPopulations(regionIndex);
            } else {
                return Map.data.GetCivilizationDetailsByRegion(regionIndex, paramiterIndex);
            }
        } else {
            return Map.data.GetCivilizationDetails(regionIndex, civID, paramiterIndex);
        }
    }

    public void Initialization() {
        Clear();
        pieContent.gameObject.SetActive(true);
        legendsPanel.gameObject.SetActive(true);
        switch (groupIndex) {
            case 0: InitEcology(); break;
            case 1: InitCivilization(); break;
        }
        Sorting();
    }

    public void Clear() {
        paramiterLabel.text = "";
        allValues = 0;
        for (int i = 0; i < legendElements.Count; ++i) {
            legendElements[i].DestroyGO();
            pieElements[i].DestroyGO();
        }
        legendElements.Clear();
        pieElements.Clear();
        pieContent.gameObject.SetActive(false);
        legendsPanel.gameObject.SetActive(false);
    }

    private void InitEcology() {
        SL_Paramiter<SL_Detail[]> paramitersStrings = Settings.Localization.Map.Ecology.Value[paramiterIndex];
        paramiterLabel.text = paramitersStrings.Name;
        values = Map.data.GetRegion(regionIndex).GetEcologyDetails(paramiterIndex);
        for (int i = 0; i < values.Length; ++i) {
            AddLegend(Settings.Theme.GetEcologyColor(paramiterIndex, i),
                      Settings.Localization.Map.Ecology.Value[paramiterIndex].Value[i].Name,
                      (int) values[i]);
        }
    }

    private void InitCivilization() {
        SL_Paramiter<SL_Detail[]> paramitersStrings = Settings.Localization.Map.Civilization.Paramiters[paramiterIndex];
        paramiterLabel.text = paramitersStrings.Name;
        values = CivilizationValues();
        for (int i = 0; i < values.Length; ++i) {
            AddLegend(Settings.Theme.GetCivilizationColor(paramiterIndex + 1, i),
                      Settings.Localization.Map.Civilization.Paramiters[paramiterIndex].Value[i].Name,
                      (int) values[i]);
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
        values = groupIndex == 1 ? CivilizationValues() :
            Map.data.GetRegion(regionIndex).GetEcologyDetails(paramiterIndex);
        allValues = 0;
        for (int i = 0; i < values.Length; ++i) {
            legendElements[i].Value = (int) values[i];
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

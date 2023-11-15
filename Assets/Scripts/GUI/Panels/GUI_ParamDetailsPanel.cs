using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GUI_ParamDetailsPanel : GUI_BasePanel {
    [SerializeField] private Text label;
    [SerializeField] private Text close;
    [SerializeField] private GUI_PieElement pieElement;
    [SerializeField] private GUI_Legend legend;
    [SerializeField] private Transform pieContent;
    [SerializeField] private Transform legendContent;
    
    [HideInInspector] public int index;

    private List<GUI_PieElement> pieElements = new();
    private List<GUI_Legend> legendElements = new();
    private float allValues = 0;

    public string Label { set => label.text = value; }
    public string CloseTxt { set => close.text = value; }
    public void AddLegend(Color color, string label, int value) {
        allValues += value;
        GUI_PieElement newPieElement = Instantiate(pieElement, pieContent);
        newPieElement.Color = color;
        pieElements.Add(newPieElement);
        GUI_Legend newLegend = Instantiate(legend, legendContent);
        newLegend.ImageColor = color;
        newLegend.Label = label;
        newLegend.Value = value;
        legendElements.Add(newLegend);
    }

    public void SortPanel(int[] values) {
        float lastAmount = 0;
        for(int i = values.Length - 1; i >= 0; --i) {
            lastAmount += allValues == 0f ? 1f : legendElements[i].Value / allValues;
            pieElements[i].FillAmount = lastAmount;
        }
    }

    public void Close() {
        Clear();
        gameObject.SetActive(false);
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
}

using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine.UI;
using UnityEngine;
using System;

public class LayersPanel : MonoBehaviour {
    //Не рефакторить данные
    [SerializeField] private Color selectColor;
    [SerializeField] private Color unselectColor;
    [SerializeField] private SerializedDictionary<string, Color> terrain = new();
    [SerializeField] private SerializedDictionary<string, Color> climate = new();
    [SerializeField] private SerializedDictionary<float, Color> flora = new();
    [SerializeField] private SerializedDictionary<float, Color> fauna = new();
    [SerializeField] private SerializedDictionary<float, Color> population = new();
    [SerializeField] private SerializedDictionary<string, Color> ecoCulture = new();
    [SerializeField] private SerializedDictionary<string, Color> prodMode = new();
    [SerializeField] private SerializedDictionary<string, Color> government = new();
    [SerializeField] private SerializedDictionary<string, Color> civilization = new();

    private Action Paint;

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.WMSK;
    private int CountRegions => Map.AllRegions.Count;
    private TM_Region GetRegion(int index) => Map.AllRegions[index];

    public void ClickTerrain() => OnClick(() => PaintByName(terrain, (int i) => GetRegion(i).Terrain.GetMaxQuantity().key));
    public void ClickClimate() => OnClick(() => PaintByName(climate, (int i) => GetRegion(i).Climate.GetMaxQuantity().key));
    public void ClickFlora() => OnClick(() => PaintByPercent(flora, (int i) => GetRegion(i).Flora.GetMaxQuantity().value));
    public void ClickFauna() => OnClick(() => PaintByPercent(fauna, (int i) => GetRegion(i).Fauna.GetMaxQuantity().value));
    public void ClickPopulation() => OnClick(() => PaintByMax(population, (int i) => GetRegion(i).Population));
    public void ClickEcoCulture() => OnClick(() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.EcoCulture.GetMaxQuantity().key));
    public void ClickProdMode() => OnClick(() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.ProdMode.GetMaxQuantity().key));
    public void ClickGovernment() => OnClick(() => PaintByName(government, (int i) => GetRegion(i).CivMain.Government.GetMaxQuantity().key));
    public void ClickCivilization() => OnClick(() => PaintByName(government, (int i) => GetRegion(i).CivMain.Name));

    private void OnClick(Action PaintAction) {
        GameEvents.TickShow -= Paint;
        Paint = PaintAction;
        GameEvents.TickShow += Paint;
        PaintAction?.Invoke();
    }

    private void PaintByName(SerializedDictionary<string, Color> dictionary, Func<int, string> GetName) {
        if(dictionary.Count == 0) return;
        string _name;
        for (int i = 0; i < CountRegions; ++i) {
            _name = GetName(i);
            if (dictionary.ContainsKey(_name)) {
                WMSK.ToggleCountrySurface(i, true, dictionary[_name]);
            } else {
                Debug.Log($"LayersPanel.PaintByString: \"{_name}\" is not founded");
            }
        }
    }

    private void PaintByPercent(SerializedDictionary<float, Color> dictionary, Func<int, float> GetPercent) {
        if (dictionary.Count == 0) return;
        Color color = default;
        float percent;

        for (int i = 0; i < CountRegions; ++i) {
            percent = GetPercent(i);
            color = GetColorByPercent(percent, dictionary);
            WMSK.ToggleCountrySurface(i, true, color);
        }
    }

    private void PaintByMax(SerializedDictionary<float, Color> dictionary, Func<int, float> GetValue) {
        if (dictionary.Count == 0) return;
        Color color = default;
        float percent;
        float max = 0;
        for(int i = 0; i < CountRegions; ++i) {
            max = GetValue(i) > max ? GetValue(i) : max;
        }

        if(max == 0) return;

        for (int i = 0; i < CountRegions; ++i) {
            percent = GetValue(i) / max * 100;
            color = GetColorByPercent(percent, dictionary);
            WMSK.ToggleCountrySurface(i, true, color);
        }
    }

    public void Clear() {
        GameEvents.TickShow -= Paint;
        for (int i = 0; i < CountRegions; ++i) {
            WMSK.ToggleCountrySurface(i, true, Color.clear);
        }
    }

    private Color GetColorByPercent(float percent, SerializedDictionary<float, Color> dictionary) {
        foreach (KeyValuePair<float, Color> element in dictionary) {
            if (percent <= element.Key) {
                return element.Value;
            }
        }
        return default;
    }

    public void Select(Image img) => img.color = selectColor;
    public void Unselect(Image img) => img.color = unselectColor;
}

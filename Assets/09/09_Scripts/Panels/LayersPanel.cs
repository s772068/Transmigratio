using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System;

public class LayersPanel : MonoBehaviour {
    public SerializedDictionary<string, Color> terrain = new();
    public SerializedDictionary<string, Color> climate = new();
    public SerializedDictionary<float, Color> flora = new();
    public SerializedDictionary<float, Color> fauna = new();
    public SerializedDictionary<float, Color> population = new();
    public SerializedDictionary<string, Color> ecoCulture = new();
    public SerializedDictionary<string, Color> prodMode = new();
    public SerializedDictionary<string, Color> government = new();
    public SerializedDictionary<string, Color> civilization = new();

    private Action onPaint;

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.wmsk;
    private int CountRegions => Map.allRegions.Count;
    private TM_Region GetRegion(int index) => Map.allRegions[index];

    public void ClickTerrain() => OnClick(() => PaintByName(terrain, (int i) => GetRegion(i).terrain.GetMaxQuantity().key));
    public void ClickClimate() => OnClick(() => PaintByName(climate, (int i) => GetRegion(i).climate.GetMaxQuantity().key));
    public void ClickFlora() => OnClick(() => PaintByPercent(flora, (int i) => GetRegion(i).flora.GetMaxQuantity().value));
    public void ClickFauna() => OnClick(() => PaintByPercent(fauna, (int i) => GetRegion(i).fauna.GetMaxQuantity().value));
    public void ClickPopulation() => OnClick(() => PaintByMax(population, (int i) => GetRegion(i).Population));
    public void ClickEcoCulture() => OnClick(() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.ecoCulture.GetMaxQuantity().key));
    public void ClickProdMode() => OnClick(() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.prodMode.GetMaxQuantity().key));
    public void ClickGovernment() => OnClick(() => PaintByName(government, (int i) => GetRegion(i).CivMain.government.GetMaxQuantity().key));
    public void ClickCivilization() => OnClick(() => PaintByName(government, (int i) => GetRegion(i).CivMain.name));

    private void OnClick(Action PaintAction) {
        GameEvents.onTickShow += onPaint;
        onPaint = PaintAction;
        PaintAction?.Invoke();
    }

    private void PaintByName(SerializedDictionary<string, Color> dictionary, Func<int, string> GetName) {
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
        Color color = default;
        float percent;
        for (int i = 0; i < CountRegions; ++i) {
            percent = GetPercent(i);
            color = GetColorByPercent(percent, dictionary);
            WMSK.ToggleCountrySurface(i, true, color);
        }
    }

    private void PaintByMax(SerializedDictionary<float, Color> dictionary, Func<int, float> GetValue) {
        Color color = default;
        float percent;
        float max = 0;
        Debug.Log("PaintByMax");
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
        GameEvents.onTickShow -= onPaint;
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
}

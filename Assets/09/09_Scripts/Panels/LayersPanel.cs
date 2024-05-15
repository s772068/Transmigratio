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

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.wmsk;
    private int CountRegions => Map.allRegions.Count;
    private TM_Region GetRegion(int index) => Map.allRegions[index];

    // private float MaxPopulation => ?;

    public void ClickTerrain() => PaintByName(terrain, (int i) => GetRegion(i).terrain.currentMax);
    public void ClickClimate() => PaintByName(climate, (int i) => GetRegion(i).climate.currentMax);
    public void ClickFlora() => PaintByPercent(flora, (int i) => GetRegion(i).flora.richness);
    public void ClickFauna() => PaintByPercent(fauna, (int i) => GetRegion(i).fauna.richness);
    public void ClickPopulation() { } // => PaintByMax(population, MaxPopulation, (int i) => GetRegion(i).population.value);
    public void ClickEcoCulture() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.ecoCulture.currentMax);
    public void ClickProdMode() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.prodMode.currentMax);
    public void ClickGovernment() => PaintByName(government, (int i) => GetRegion(i).CivMain.government.currentMax);
    public void ClickCivilization() => PaintByName(government, (int i) => GetRegion(i).CivMain.name);

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

    private void PaintByMax(SerializedDictionary<float, Color> dictionary, float max, Func<int, float> GetValue) {
        Color color = default;
        float percent;
        for (int i = 0; i < CountRegions; ++i) {
            percent = GetValue(i) / max * 100;
            color = GetColorByPercent(percent, dictionary);
            WMSK.ToggleCountrySurface(i, true, color);
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

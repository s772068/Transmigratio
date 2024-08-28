using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System;

namespace Layers {
    public class Panel : UI.ScrollVew.Scroller {
        //Не рефакторить данные

        [Header("LayersElement")]
        [SerializeField] private protected float timeToUpdateSize;
        [SerializeField] private protected float timeToUpdateAlpha;
        [SerializeField] private List<Vector2> sizes;
        [SerializeField] private List<float> alphas;

        [Header("Colors")]
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

        private Map Map => Transmigratio.Instance.TMDB.map;
        private WMSK WMSK => Map.WMSK;
        private int CountRegions => Map.AllRegions.Count;
        private TM_Region GetRegion(int index) => Map.AllRegions[index];

        public void ClickTerrain() => OnClick(() => PaintByName(terrain, (int i) => GetRegion(i).Terrain.GetMax().key));
        public void ClickClimate() => OnClick(() => PaintByName(climate, (int i) => GetRegion(i).Climate.GetMax().key));
        public void ClickFlora() => OnClick(() => PaintByPercent(flora, (int i) => GetRegion(i).Flora.GetMax().value));
        public void ClickFauna() => OnClick(() => PaintByPercent(fauna, (int i) => GetRegion(i).Fauna.GetMax().value));
        public void ClickPopulation() => OnClick(() => PaintByMax(population, (int i) => GetRegion(i).Population));
        public void ClickEcoCulture() => OnClick(() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.EcoCulture.GetMax().key));
        public void ClickProdMode() => OnClick(() => PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.ProdMode.GetMax().key));
        public void ClickGovernment() => OnClick(() => PaintByName(government, (int i) => GetRegion(i).CivMain.Government.GetMax().key));
        public void ClickCivilization() => OnClick(() => PaintByName(government, (int i) => GetRegion(i).CivMain.Name));

        private void OnClick(Action PaintAction) {
            Timeline.TickShow -= Paint;
            Paint = PaintAction;
            Timeline.TickShow += Paint;
            PaintAction?.Invoke();
        }

        private void PaintByName(SerializedDictionary<string, Color> dictionary, Func<int, string> GetName) {
            if (dictionary.Count == 0) return;
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
            for (int i = 0; i < CountRegions; ++i) {
                max = GetValue(i) > max ? GetValue(i) : max;
            }

            if (max == 0) return;

            for (int i = 0; i < CountRegions; ++i) {
                percent = GetValue(i) / max * 100;
                color = GetColorByPercent(percent, dictionary);
                WMSK.ToggleCountrySurface(i, true, color);
            }
        }

        public void Clear() {
            Timeline.TickShow -= Paint;
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

        private protected override void SelectElement() {
            switch (GetSelectedElement<LayerElement>().index) {
                case 0: PaintByName(terrain, (int i) => GetRegion(i).Terrain.GetMax().key); break;
                case 1: PaintByName(climate, (int i) => GetRegion(i).Climate.GetMax().key); break;
                case 2: PaintByPercent(flora, (int i) => GetRegion(i).Flora.GetMax().value); break;
                case 3: PaintByPercent(fauna, (int i) => GetRegion(i).Fauna.GetMax().value); break;
                case 4: PaintByMax(population, (int i) => GetRegion(i).Population); break;
                case 5: PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.EcoCulture.GetMax().key); break;
                case 6: PaintByName(ecoCulture, (int i) => GetRegion(i).CivMain.ProdMode.GetMax().key); break;
                case 7: PaintByName(government, (int i) => GetRegion(i).CivMain.Government.GetMax().key); break;
                case 8: PaintByName(government, (int i) => GetRegion(i).CivMain.Name); break;
            }
        }

        private protected override void UpdateElements(int index) {
            UpdateSize(index);
            UpdateAlpha(index);
        }

        private void UpdateSize(int index) {
            int sizeIndex = Mathf.Abs(index - _selectedIndex);
            if (sizes.Count == 0) {
                GetElement<LayerElement>(index).UpdateSize(Vector2.one, timeToUpdateSize);
            } else if (sizeIndex < sizes.Count) {
                GetElement<LayerElement>(index).UpdateSize(sizes[sizeIndex], timeToUpdateSize);
            }
        }

        private void UpdateAlpha(int index) {
            int alphaIndex = Mathf.Abs(index - _selectedIndex);
            if (alphas.Count == 0) {
                GetElement<LayerElement>(index).UpdateAlpha(1f, timeToUpdateAlpha);
            } else if (alphaIndex < alphas.Count) {
                GetElement<LayerElement>(index).UpdateAlpha(alphas[alphaIndex], timeToUpdateAlpha);
            }
        }
    }
}

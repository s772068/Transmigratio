using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using WorldMapStrategyKit;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace Layers {
    public class Panel : UI.ScrollVew.Scroller {
        [Header("LayersPanel")]
        [SerializeField] private Image _gradient;
        [SerializeField] private RectTransform _scrollView;

        [Header("LayersElement")]
        [SerializeField] private protected float _timeToUpdateSize;
        [SerializeField] private protected float _timeToUpdateAlpha;
        [SerializeField] private GameObject _tutorial;
        [SerializeField] private List<Vector2> _sizes;
        [SerializeField] private List<float> _alphas;

        [Header("Colors")]
        [SerializeField] private SerializedDictionary<string, Color> _terrain = new();
        [SerializeField] private SerializedDictionary<string, Color> _climate = new();
        [SerializeField] private SerializedDictionary<float, Color> _flora = new();
        [SerializeField] private SerializedDictionary<float, Color> _fauna = new();
        [SerializeField] private SerializedDictionary<float, Color> _population = new();
        [SerializeField] private SerializedDictionary<string, Color> _ecoCulture = new();
        [SerializeField] private SerializedDictionary<string, Color> _prodMode = new();
        [SerializeField] private SerializedDictionary<string, Color> _government = new();
        [SerializeField] private SerializedDictionary<string, Color> _civilization = new();

        public static event Action onOpen;
        private Action Paint;

        private Map Map => Transmigratio.Instance.TMDB.map;
        private WMSK WMSK => MapData.WMSK;
        private int CountRegions => Map.AllRegions.Count;
        private TM_Region GetRegion(int index) => Map.AllRegions[index];

        private protected override void Awake() {
            base.Awake();
            Tutorial.OnShowTutorial += ShowTutorial;
            RegionDetails.StartGame.Panel.onClose += ColorBySelectedElement;
        }

        private protected override void OnEnable() {
            base.OnEnable();
            onOpen?.Invoke();
            Timeline.TickShow += ColorBySelectedElement;
        }

        private void OnDisable() {
            Timeline.TickShow -= ColorBySelectedElement;
        }

        private void ShowTutorial(string tutName) {
            if (tutName == "Layers") {
                _tutorial?.SetActive(true);
            }
        }

        public void ClickTerrain() => OnClick(() => PaintByName(_terrain, (int i) => GetRegion(i).Terrain.GetMax().key));
        public void ClickClimate() => OnClick(() => PaintByName(_climate, (int i) => GetRegion(i).Climate.GetMax().key));
        public void ClickFlora() => OnClick(() => PaintByPercent(_flora, (int i) => GetRegion(i).Flora.GetMax().value));
        public void ClickFauna() => OnClick(() => PaintByPercent(_fauna, (int i) => GetRegion(i).Fauna.GetMax().value));
        public void ClickPopulation() => OnClick(() => PaintByMax(_population, (int i) => GetRegion(i).Population));
        public void ClickEcoCulture() => OnClick(() => PaintByName(_ecoCulture, (int i) => GetRegion(i).CivMain.EcoCulture.GetMax().key));
        public void ClickProdMode() => OnClick(() => PaintByName(_ecoCulture, (int i) => GetRegion(i).CivMain.ProdMode.GetMax().key));
        public void ClickGovernment() => OnClick(() => PaintByName(_government, (int i) => GetRegion(i).CivMain.Government.GetMax().key));
        public void ClickCivilization() => OnClick(() => PaintByName(_government, (int i) => GetRegion(i).CivMain.Name));

        private void OnClick(Action PaintAction) {
            Timeline.TickShow -= Paint;
            Paint = PaintAction;
            Timeline.TickShow += Paint;
            PaintAction?.Invoke();
        }

        public void Open() {
            Timeline.TickShow += Paint;
            gameObject.SetActive(true);
            ColorBySelectedElement();
            _gradient.DOPause();
            _scrollView.DOPause();
            _gradient.DOFade(1, 1);
            _scrollView.DOAnchorPosY(0, 1);
        }

        public void Close() {
            Timeline.TickShow -= Paint;
            _gradient.DOPause();
            _scrollView.DOPause();
            _gradient.DOFade(0, 1);
            _scrollView.DOAnchorPosY(-1000, 1)
                       .OnComplete(() => gameObject.SetActive(false));
            Clear();
        }

        public void Hide() => gameObject.SetActive(false);

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

        private protected override void ColorBySelectedElement() {
            switch (GetSelectedElement<LayerElement>().index) {
                case 0: PaintByName(_terrain, (int i) => GetRegion(i).Terrain.GetMax().key); break;
                case 1: PaintByName(_climate, (int i) => GetRegion(i).Climate.GetMax().key); break;
                case 2: PaintByPercent(_flora, (int i) => GetRegion(i).Flora.GetMax().value); break;
                case 3: PaintByPercent(_fauna, (int i) => GetRegion(i).Fauna.GetMax().value); break;
                case 4: PaintByMax(_population, (int i) => GetRegion(i).Population); break;
                case 5: PaintByName(_ecoCulture, (int i) => GetRegion(i).CivMain.EcoCulture.GetMax().key); break;
                case 6: PaintByName(_ecoCulture, (int i) => GetRegion(i).CivMain.ProdMode.GetMax().key); break;
                case 7: PaintByName(_government, (int i) => GetRegion(i).CivMain.Government.GetMax().key); break;
                case 8: PaintByName(_government, (int i) => GetRegion(i).CivMain.Name); break;
            }
        }

        private protected override void UpdateElements(int index) {
            UpdateSize(index);
            UpdateAlpha(index);
        }

        private void UpdateSize(int index) {
            int sizeIndex = Mathf.Abs(index - _selectedIndex);
            if (_sizes.Count == 0) {
                GetElement<LayerElement>(index).UpdateSize(Vector2.one, _timeToUpdateSize);
            } else if (sizeIndex < _sizes.Count) {
                GetElement<LayerElement>(index).UpdateSize(_sizes[sizeIndex], _timeToUpdateSize);
            }
        }

        private void UpdateAlpha(int index) {
            int alphaIndex = Mathf.Abs(index - _selectedIndex);
            if (_alphas.Count == 0) {
                GetElement<LayerElement>(index).UpdateAlpha(1f, _timeToUpdateAlpha);
            } else if (alphaIndex < _alphas.Count) {
                GetElement<LayerElement>(index).UpdateAlpha(_alphas[alphaIndex], _timeToUpdateAlpha);
            }
        }
    }
}

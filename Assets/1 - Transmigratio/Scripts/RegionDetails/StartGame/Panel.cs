using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UI;

namespace RegionDetails.StartGame {
    public enum EParamiter { Terrain, Climate, Fauna, Flora }
    public class Panel : UI.Panel {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Element _elementPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private ButtonsRadioGroup selector;
        [SerializeField] private GameObject _tutorial;
        [SerializeField] private EParamiter _paramiter;

        private List<Element> _elements = new();

        public static event Action onStartGame;
        public static event Action onClose;

        private void Awake() {
            Tutorial.OnShowTutorial += ShowTutorial;
        }

        private void Start() {
            _label.text = Transmigratio.Instance.TMDB.map.AllRegions[MapData.RegionID].Name;
            selector.Select((int)_paramiter);
            Click((int) _paramiter);
        }

        private void ShowTutorial(string tutName) {
            if (tutName == "StartRegionDetails")
                _tutorial?.SetActive(true);
        }

        public void Click(int paramiter) {
            if (paramiter < 0 || paramiter >= Enum.GetNames(typeof(EParamiter)).Length) return;
            _paramiter = (EParamiter)paramiter;
            UpdateElements();
        }

        public void PrevRegion() => SelectRegion(MapData.RegionID - 1);
        public void NextRegion() => SelectRegion(MapData.RegionID + 1);

        public void StartGame() {
            onStartGame?.Invoke();
            Destroy(gameObject);
        }

        public void Close() {
            MapData.WMSK.ToggleCountrySurface(MapData.RegionID, true, Color.clear);
            onClose?.Invoke();
            Destroy(gameObject);
        }

        private void SelectRegion(int regionID) {
            MapData.UpdateRegion(regionID);
            _label.text = Transmigratio.Instance.TMDB.map.AllRegions[MapData.RegionID].Name;
            UpdateElements();
        }

        private void CreateParamiter() {
            _elements = Factory.Create(_elementPrefab, _content, _paramiter.ToString());
        }

        private void UpdateElements() {
            Clear();
            CreateParamiter();
        }

        private void Clear() {
            for(int i = 0; i < _elements.Count; ++i) {
                _elements[i].Destroy();
            }
            _elements.Clear();
        }
    }
}

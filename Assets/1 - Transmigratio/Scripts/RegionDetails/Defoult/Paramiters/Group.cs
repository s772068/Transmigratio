using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RegionDetails.Defoult.Paramiters {
    public class Group : MonoBehaviour {
        [SerializeField] private Element _paramiterPref;
        [SerializeField] private Transform _content;
        [SerializeField] private SerializedDictionary<string, Sprite> _elements;

        private Dictionary<string, Element> _elementsDic = new();
        private string _selected = "";

        public Action<string> onSelect;

        private void Awake() {
            foreach (var pair in _elements) {
                AddParamiter(pair.Key, pair.Value);
            }
        }

        private void AddParamiter(string name, Sprite pictogram) {
            Element element = Factory.Create(_paramiterPref, _content, name, pictogram);
            element.onSelect = OnSelect;
            _elementsDic[name] = element;
        }

        public void OnSelect(string name) {
            if (_selected == name) return;
            if (_selected != "") _elementsDic[_selected].Select(false);
            _selected = name;
            _elementsDic[name]?.Select(true);
            onSelect?.Invoke(name);
        }
    }
}

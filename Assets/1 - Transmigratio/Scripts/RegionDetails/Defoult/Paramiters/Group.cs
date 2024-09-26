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
        private Element _selectedElement;
        private bool _isActive;
        
        public bool IsActive {
            get => _isActive;
            set {
                _isActive = value;
                foreach(var pair in _elementsDic) {
                    pair.Value.isActive = value;
                }
            }
        }

        public void SetActiveParamiter(string paramiter, bool isActive) {
            if (paramiter == "All") IsActive = isActive;
            else _elementsDic[paramiter].isActive = isActive;
        }


        public Action<string> onSelect;

        private void Awake() {
            foreach(var pair in _elements) {
                AddParamiter(pair.Key, pair.Value);
            }
        }

        private void AddParamiter(string name, Sprite pictogram) {
            Element element = Factory.Create(_paramiterPref, _content, name, pictogram);
            element.onSelect = OnSelect;
            _elementsDic[name] = element;
        }

        public void OnSelect(string name) {
            _selectedElement?.Select(false);
            _selectedElement = _elementsDic[name];
            onSelect?.Invoke(name);
        }
    }
}

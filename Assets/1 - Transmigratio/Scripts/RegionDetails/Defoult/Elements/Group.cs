using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace RegionDetails.Defoult.Elements {
    public class Group : MonoBehaviour {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Element _elementPref;
        [SerializeField] private Transform _content;


        private List<Element> _elements = new();
        private Element _selectedElement;
        private bool _isActive;

        public bool IsActive {
            get => _isActive;
            set {
                _isActive = value;
                for (int i = 0; i < _elements.Count; ++i) {
                    _elements[i].IsActive = value;
                }
            }
        }

        public Action<string> onSelect;

        public string Label { set => _label.text = value; }
        public bool IsSelectable { private get; set; }

        public void UpdateElements(string paramiter) {
            Clear();
            Dictionary<string, float> dic = new();
            if (paramiter == "FloraFauna") {
                CreateElements(ref dic, "Flora");
                CreateElements(ref dic, "Fauna");
            } else {
                CreateElements(ref dic, paramiter);
            }
        }

        private void CreateElements(ref Dictionary<string, float> dic, string paramiter) {
            dic = Transmigratio.Instance.TMDB.GetParam(paramiter);
            foreach (var pair in dic) {
                Debug.Log(paramiter);
                Element element = Factory.Create(_elementPref, _content, _elements.Count, pair.Key, pair.Value, paramiter != "Civilizations");
                element.IsSelectable = IsSelectable;
                element.onSelect = OnSelect;
                element.IsActive = _isActive;
                _elements.Add(element);
            }
        }

        private void Clear() {
            Debug.Log(_elements == null);
            for (int i = 0; i < _elements.Count; ++i) {
                _elements[i].Destroy();
            }
            _selectedElement = null;
            _elements.Clear();
        }

        private void OnSelect(int index) {
            _selectedElement?.Select(false);
            _selectedElement = _elements[index];
            onSelect?.Invoke(_elements[index].name);
        }
    }
}

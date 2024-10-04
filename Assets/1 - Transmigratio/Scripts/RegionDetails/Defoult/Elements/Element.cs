using UnityEngine.EventSystems;
using UnityEngine;
using System;

namespace RegionDetails.Defoult.Elements {
    public class Element : Base.Element, IPointerClickHandler {
        [SerializeField] private GameObject _select;

        private int _index;
        public bool IsSelectable { private get; set; }

        private bool _isSelected;

        public Action<int> onSelect;

        public void Init(int index, string paramiter, string label, float value) {
            _index = index;
            Label = Localization.Load(paramiter, label);
            Debug.Log(label);
            SetValue(value, paramiter != "Civilizations");
        }

        public void Select(bool isSelected) {
            if (_isSelected == isSelected) return;
            _isSelected = isSelected;
            if (isSelected) onSelect?.Invoke(_index);
            _select.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!IsSelectable) return;
            Select(true);
        }
    }
}

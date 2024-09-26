using UnityEngine.EventSystems;
using UnityEngine;
using System;

namespace RegionDetails.Defoult.Elements {
    public class Element : Base.Element, IPointerClickHandler {
        [SerializeField] private GameObject _select;

        public int Index { private get; set; }
        public bool IsSelectable { private get; set; }
        public bool IsActive { get; set; }

        private bool _isSelected;

        public Action<int> onSelect;

        public void Select(bool isSelected) {
            if (_isSelected == isSelected) return;
            _isSelected = isSelected;
            if (isSelected) onSelect?.Invoke(Index);
            _select.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!IsActive) return;
            if (!IsSelectable) return;
            Select(true);
        }
    }
}

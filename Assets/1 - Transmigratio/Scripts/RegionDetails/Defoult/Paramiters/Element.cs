using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace RegionDetails.Defoult.Paramiters {
    public class Element : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private GameObject _select;
        [SerializeField] private Image _pictogram;

        private bool _isSelected;
        public bool isActive;

        public Action<string> onSelect;

        public string Name { set => name = value; }
        public Sprite Pictogram { set => _pictogram.sprite = value; }

        public void Select(bool isSelected) {
            if (_isSelected == isSelected) return;
            _isSelected = isSelected;
            if(isSelected) onSelect?.Invoke(name);
            _select.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!isActive) return;
            Select(true);
        }
    }
}

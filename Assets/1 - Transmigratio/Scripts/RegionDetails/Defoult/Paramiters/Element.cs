using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;
using DG.Tweening;

namespace RegionDetails.Defoult.Paramiters {
    public class Element : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private GameObject _select;
        [SerializeField] private Image _pictogram;

        private bool _isSelected;

        public Action<string> onSelect;

        public string Name { set => name = value; }
        public Sprite Pictogram { set => _pictogram.sprite = value; }

        public void Select(bool isSelected) {
            if (_isSelected == isSelected) return;
            _isSelected = isSelected;
            if (isSelected) {
                onSelect?.Invoke(name);
                _pictogram.transform.DOScale(Vector2.one * 0.8f, 0.2f).SetEase(Ease.InBack);
            } else {
                _pictogram.transform.DOScale(Vector2.one, 0.2f).SetEase(Ease.OutBack);
            }
            _select.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData) {
            Select(true);
        }
    }
}

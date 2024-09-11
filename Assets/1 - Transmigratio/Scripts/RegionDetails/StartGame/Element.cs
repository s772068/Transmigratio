using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace RegionDetails.StartGame {
    public class Element : Base.Element {
        [SerializeField] private Image _backGradient;

        public Color Color { set { if (_backGradient != null) _backGradient.color = value; } }

        public void OnPointerClick(PointerEventData eventData) {
        }
    }
}

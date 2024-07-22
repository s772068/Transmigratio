using UnityEngine.EventSystems; 
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace RegionDetails {
    public class Paramiter : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _valTxt;
        [SerializeField] private Slider _slider;

        private string _key;

        public Action<string> Click;

        public void SetTitle(string element, string _title) {
            this._title.text = Utilits.Localization.Load(element, _title);
            _key = _title;
        }

        public float Value {
            set {
                _valTxt.text = value.ToString();
                _slider.value = value;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            Click?.Invoke(_key);
        }

        public void Destroy() => Destroy(gameObject);
    }
}

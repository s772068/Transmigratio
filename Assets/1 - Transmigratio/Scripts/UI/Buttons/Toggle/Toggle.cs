using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UserInterface {
    public class Toggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _disabledSprite;
        [SerializeField] private bool _isInteractable;
        [SerializeField] private bool _isOn;
        [SerializeField] private State _deactivate;
        [SerializeField] private State _activate;

        public bool IsOn {
            get => _isOn;
            set {
                if (_isOn == value) return;
                _isOn = value;
                if (_isOn) {
                    _image.sprite = _deactivate.sprite;
                    _deactivate.onClick?.Invoke();
                } else {
                    _image.sprite = _activate.sprite;
                    _activate.onClick?.Invoke();
                }
            }
        }

        private void OnValidate() {
            if (_image == null) return;
            _image.sprite = !_isInteractable ?
                _disabledSprite : _isOn ?
                _activate.sprite : _deactivate.sprite;
        }

        private void Start() {
            if (_isOn) _activate.onClick?.Invoke();
            else _deactivate.onClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!_isInteractable) return;
            _image.sprite = _isOn ? _activate.highlighted : _deactivate.highlighted;
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!_isInteractable) return;
            _image.sprite = _isOn ? _activate.sprite : _deactivate.sprite;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!_isInteractable) return;
            _image.sprite = _isOn ? _activate.pressed : _deactivate.pressed;
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (!_isInteractable) return;
            _image.sprite = _isOn ? _deactivate.highlighted : _activate.highlighted;
            if (_isOn) _deactivate.onClick?.Invoke();
            else _activate.onClick?.Invoke();
            _isOn = !_isOn;
        }
    }

    [System.Serializable]
    public struct State {
        public Sprite sprite;
        public Sprite highlighted;
        public Sprite pressed;
        [Space]
        public UnityEvent onClick;
    }
}

using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Buttons.Radio {
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite deactiveSprite;
        [SerializeField] private Sprite highlightedSprite;

        [Space]
        public UnityEvent<int> onClick = new UnityEvent<int>();

        private bool _isActive;
        private Image _image;

        public int Index { private get; set; }
        private void Awake() {
            _image = GetComponent<Image>();
        }

        public void Activate() {
            _isActive = true;
            _image.sprite = activeSprite;
        }

        public void Deactivate() {
            _isActive = false;
            _image.sprite = deactiveSprite;
        }

        public void OnPointerClick(PointerEventData eventData) {
            onClick.Invoke(Index);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (highlightedSprite == null) return;
            if (!_isActive) _image.sprite = highlightedSprite;
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (highlightedSprite == null) return;
            if (_isActive) Activate();
            else Deactivate();
        }
    }
}

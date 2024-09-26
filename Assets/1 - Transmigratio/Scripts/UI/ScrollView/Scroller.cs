using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace UI.ScrollVew {
    public class Scroller : MonoBehaviour {
        [Header("Scroller")]
        public Scrollbar scrollbar;
        [SerializeField] private protected float timeToAlignment;
        [SerializeField] private protected List<Element> elements;

        private protected int _selectedIndex;

        private float _scrollPosition;
        private float[] _positions;
        private float _distance;
        private bool _isClicked;

        private protected T GetSelectedElement<T>() where T : Element => (T) elements[_selectedIndex];
        private protected T GetElement<T>(int index) where T : Element => (T) elements[index];

        private protected virtual void UpdateElements(int index) { }
        private protected virtual void SelectElement() { }

        private void Awake() {
            _positions = new float[elements.Count];
            _distance = 1f / (elements.Count - 1f);
            for (int i = 0; i < elements.Count; i++) {
                _positions[i] = 1 - _distance * i;
                elements[i].index = i;
            }
        }

        private protected virtual void OnEnable() {
            SelectElement();
        }

        private void Update() {
            _isClicked = Input.GetMouseButton(0);
            if (_isClicked) {
                _scrollPosition = scrollbar.value;
            }
        }

        private void FixedUpdate() {
            if(_positions == null) return;
            for (int i = 0; i < _positions.Length; i++) {
                UpdateSelectedIndex(i);
                if (_isClicked) _scrollPosition = scrollbar.value;
                else Alignment(_selectedIndex);
                UpdateElements(i);
            }
        }

        private void UpdateSelectedIndex(int index) {
            if (_scrollPosition < _positions[index] + (_distance / 2) &&
                _scrollPosition > _positions[index] - (_distance / 2) &&
                index != _selectedIndex) {
                    _selectedIndex = index;
                    SelectElement();
            }
        }

        public void Alignment(int index) {
            _scrollPosition = _positions[index];
            scrollbar.value = Mathf.Lerp(scrollbar.value, _positions[index], timeToAlignment);
        }
    }
}

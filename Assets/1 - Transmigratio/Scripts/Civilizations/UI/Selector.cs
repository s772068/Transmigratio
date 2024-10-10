using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

namespace Civilizations {
    public class Selector : MonoBehaviour {
        [SerializeField] private Transform _content;
        [SerializeField] private Element _elementPref;
        [SerializeField] private List<Element.Data> _elements;

        private Element _activeElement;
        private bool isOpened;
        private RectTransform rect;

        public static event Action<string> onClick;

        private void Awake() {
            rect = GetComponent<RectTransform>();
        }

        private void Start() {
            InitElements();
        }

        private void ClickElement(Element element) {
            _activeElement?.Disactivate();
            _activeElement = element;
            _activeElement.Activate();
            onClick?.Invoke(element.Title);
        }

        private void InitElements() {
            for(int i = 0; i < _elements.Count; ++i) {
                AddElement(_elements[i]);
            }
        }

        private void AddElement(Element.Data data) {
            Element element = Factory.Create(_elementPref, _content, data);
            element.onClick += ClickElement;
        }

        public void Open() {
            rect.DOPause();
            rect.DOAnchorPosY(isOpened ? 410 : -370, 0.5f);
            isOpened = !isOpened;
        }
    }
}

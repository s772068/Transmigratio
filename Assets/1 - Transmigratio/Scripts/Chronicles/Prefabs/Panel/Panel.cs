using System.Collections.Generic;
using UnityEngine;
using System;

namespace Chronicles.Prefabs.Panel {
    public class Panel : MonoBehaviour {
        [SerializeField] private Element elementPrefab;
        [SerializeField] private RectTransform elementContent;
        [SerializeField, Min(0)] private int countElements;

        private List<Element> _elements = new();

        public Action<Data.Panel.Element> onClickElement;

        public int CountElements => countElements;

        public List<Data.Panel.Element> Elements { set {
                for(int i = 0; i < _elements.Count; ++i) {
                    _elements[i].Destroy();
                }
                _elements.Clear();

                for (int i = 0; i < value.Count; ++i) {
                    Element element = Factory.CreatePanelElement(elementContent, elementPrefab, value[i]);
                    element.onClick = onClickElement;
                    _elements.Add(element);
                }
            }
        }
    }
}

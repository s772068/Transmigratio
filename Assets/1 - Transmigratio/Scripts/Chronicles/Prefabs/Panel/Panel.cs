using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Localization.Components;
using TMPro;
using UnityEngine.UI;

namespace Chronicles.Prefabs.Panel {
    public class Panel : UI.Panel {
        [SerializeField] private GameObject _descriptionBlock;
        [SerializeField] private Element elementPrefab;
        [SerializeField] private RectTransform elementContent;
        [SerializeField] private LocalizeStringEvent _description;
        [SerializeField] private LocalizeStringEvent _result;
        [SerializeField] private TMP_Text _desidion;
        [SerializeField] private ToggleGroup _group;

        private List<Element> _elements = new();

        public Action<Data.Panel.Element> onClickElement;

        public List<Data.Panel.Element> Elements { set {
                for(int i = 0; i < _elements.Count; ++i) {
                    _elements[i].Destroy();
                }
                _elements.Clear();

                for (int i = 0; i < value.Count; ++i) {
                    Element element = Factory.CreatePanelElement(elementContent, elementPrefab, value[i], _group);
                    element.onClick = onClickElement;
                    _elements.Add(element);
                }
            }
        }

        public void InitDescription(Data.Panel.Element element)
        {
            _descriptionBlock.gameObject.SetActive(true);
            _description.SetEntry("Test");
            _result.SetEntry("Test");
            _desidion.text = "Test Desidion";
        }
    }
}

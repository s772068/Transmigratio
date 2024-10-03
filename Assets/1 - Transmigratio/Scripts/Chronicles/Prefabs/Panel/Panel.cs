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
        [SerializeField] private LocalizeStringEvent _eventActionName;
        [SerializeField] private LocalizeStringEvent _description;
        [SerializeField] private LocalizeStringEvent _result;
        [SerializeField] private TMP_Text _desidion;
        [SerializeField] private ToggleGroup _group;

        private List<Element> _elements = new();

        public Action<Data.Panel.Element> onClickElement;
        public int Count = 0;
        public string RegionFirst = "";
        public string RegionSecond = "";

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
            Count = element.LocalVariables.Count;
            RegionFirst = element.LocalVariables.RegionFirst;
            RegionSecond = element.LocalVariables.RegionSecond;

            _descriptionBlock.SetActive(true);
            _description.SetEntry(element.EventName);
            _desidion.text = element.Desidion;
            _result.RefreshString();

            switch (element.EventName)
            {
                case "Migration":
                    _eventActionName.SetEntry("ResultMigration");
                    break;
                default:
                    _eventActionName.SetEntry("ResultDied");
                    break;
            }
        }
    }
}

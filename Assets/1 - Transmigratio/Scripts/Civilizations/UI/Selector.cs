using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using System;
using UI.CivPanel;

namespace Civilizations {
    public class Selector : MonoBehaviour {
        [SerializeField] private Transform _content;
        [SerializeField] private Element _elementPref;

        private Humanity _humanity;
        private Dictionary<string, Element> _elements = new();
        private RectTransform _rect;
        private string _active;
        private bool _isOpen;


        public static event Action<string> onSelect;
        public static event Action onUnselect;

        private void Awake() {
            _rect = GetComponent<RectTransform>();
        }

        private void Start() {
            _humanity = Transmigratio.Instance.TMDB.humanity;
        }

        private void ClickElement(Element element) {
            if(_active != null) _elements[_active]?.Disactivate();
            if(element.Title == _active) {
                _active = null;
                onUnselect?.Invoke();
            } else {
                _active = element.Title;
                _elements[_active].Activate();
                onSelect?.Invoke(element.Title);
            }
        }

        private void UpdateElements() {
            ClearElements();
            string[] civs = GetCurCivs();
            for (int i = 0; i < civs.Length; ++i) {
                AddElement(civs[i]);
            }
        }

        private string[] GetCurCivs() {
            return _humanity.Civilizations.Keys.ToArray();
        }

        private void AddElement(string title) {
            Element element = Factory.Create(_elementPref, _content, title, _humanity.GetIcon(title));
            element.onClick += ClickElement;
            if(title == _active) element.Activate();
            _elements.Add(title, element);
        }

        private void ClearElements() {
            for(int i = 0; i < _elements.Count; ++i) {
                _elements.ElementAt(i).Value.Destroy();
                _elements[_elements.ElementAt(i).Key] = null;
            }
            _elements.Clear();
        }

        public void Open() {
            _isOpen = !_isOpen;
            _rect.DOPause();
            if (_isOpen) {
                UpdateElements();
                _rect.DOAnchorPosY(-370, 0.5f);
                Timeline.TickShow += UpdateElements;
            } else {
                _rect.DOAnchorPosY(410, 0.5f).OnComplete(() => ClearElements());
                Timeline.TickShow -= UpdateElements;
                _active = null;
            }
        }
        
        public void OpenCivPanel() => CivPanel.OpenCivPanel?.Invoke(_humanity.Civilizations[_active]);
    }
}

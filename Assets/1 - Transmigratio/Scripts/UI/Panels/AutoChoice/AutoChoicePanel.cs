using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using UI;

namespace Gameplay.Scenarios.Events {
    public class AutoChoicePanel : Panel {
        [Header("AutoChoice Events List")]
        [SerializeField] private AutoChoiceElement _prefabAutoChoiceElement;
        [SerializeField] private Transform _contentElements;
        private Dictionary<Base, AutoChoiceElement> _elements = new();

        [Header("Event AutoChoice")]
        private Base _selectEvent;
        [SerializeField] private DragPanelControl _dragPanel;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Toggle _autoEnable;
        [SerializeField] private Image _eventImage;
        [SerializeField] private Transform _autoPriority;
        [Header("AutoChoice Slider")]
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _pointsTMP;
        [SerializeField] private int _maxPoints = 100;
        private int _curPoints = 0;
        private Dictionary<DragElement, Data.Desidion> _autoChoicePriority = new();

        public static event Action<Base, List<Data.Desidion>, bool> AutoChoiceUpdate;
        public static event Action<Base, bool> AutoChoiceModeUpdate;

        private protected override void OnEnable() {
            base.OnEnable();
            AddChoiceElement();
            DragPanelControl.DragElementsSorted += OnPriorityUpdate;
            AutoChoiceElement.SelectElement += OnSelectEvent;
        }

        private void Start() {
            Base selectEvent = null;
            foreach (var element in _elements) {
                selectEvent = element.Key;
                break;
            }
            if (selectEvent != null)
                OnSelectEvent(selectEvent);
        }

        private protected override void OnDisable() {
            base.OnDisable();
            DragPanelControl.DragElementsSorted -= OnPriorityUpdate;
            AutoChoiceElement.SelectElement -= OnSelectEvent;
        }

        private void AddChoiceElement() {
            foreach (var gameEvent in AutoChoice.Events) {
                if (_elements.ContainsKey(gameEvent.Key))
                    continue;

                AutoChoiceElement element = Instantiate(_prefabAutoChoiceElement, _contentElements);
                element.Init(gameEvent.Key);
                _elements.Add(gameEvent.Key, element);
            }

            if (_elements.Count > AutoChoice.Events.Count) {
                foreach (var gameEvent in _elements) {
                    if (AutoChoice.Events.ContainsKey(gameEvent.Key))
                        continue;

                    Destroy(gameEvent.Value.gameObject);
                    _elements.Remove(gameEvent.Key);
                }
            }
        }

        private void OnSelectEvent(Base selectEvent) {
            _selectEvent = selectEvent;
            _autoChoicePriority = new();
            _title.text = selectEvent.Local("Title");
            _description.text = selectEvent.Local("Description");
            _autoEnable.isOn = selectEvent.AutoChoice;
            _eventImage.sprite = selectEvent.PanelSprite;
            List<Data.Desidion> desidions = AutoChoice.Events[selectEvent];
            for (int i = 0; i < _dragPanel.Elements.Count; i++) {
                _dragPanel.Elements[i].GetComponent<AutoChoiceDesidion>().Title.text = desidions[i].Title;
                _autoChoicePriority.Add(_dragPanel.Elements[i], desidions[i]);
            }

            _curPoints = selectEvent.MaxAutoInterventionPoints;
            _pointsTMP.text = _curPoints.ToString();
            _slider.value = _slider.maxValue / _maxPoints * _curPoints;
        }

        private void OnPriorityUpdate() {
            List<Data.Desidion> newDesidions = new(AutoChoice.Events[_selectEvent]);

            int index = 0;
            foreach (var dragElement in _dragPanel.Elements) {
                newDesidions[index] = _autoChoicePriority[dragElement];
                index++;
            }

            AutoChoiceUpdate?.Invoke(_selectEvent, newDesidions, _autoEnable.isOn);
        }

        public void UpdateSlider()
        {
            _curPoints = (int)(_maxPoints * _slider.value);
            _pointsTMP.text = _curPoints.ToString();
            _selectEvent.MaxAutoInterventionPoints = _curPoints;
        }

        public void AutoChoiceEnable() => AutoChoiceModeUpdate?.Invoke(_selectEvent, _autoEnable.isOn);
    }
}

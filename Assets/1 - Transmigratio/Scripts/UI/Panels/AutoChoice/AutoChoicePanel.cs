using Events;
using Events.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class AutoChoicePanel : Panel
{
    [Header("AutoChoice Events List")]
    [SerializeField] private AutoChoiceElement _prefabAutoChoiceElement;
    [SerializeField] private Transform _contentElements;
    private Dictionary<Events.Controllers.Base, AutoChoiceElement> _elements = new();

    [Header("Event AutoChoice")]
    private Events.Controllers.Base _selectEvent;
    [SerializeField] private DragPanelControl _dragPanel;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Toggle _autoEnable;
    [SerializeField] private Image _eventImage;
    [SerializeField] private Transform _autoPriority;
    private Dictionary<DragElement, Desidion> _autoChoicePriority = new();

    public static event Action<Events.Controllers.Base, List<Desidion>, bool> AutoChoiceUpdate;
    public static event Action<Events.Controllers.Base, bool> AutoChoiceModeUpdate;

    private protected override void OnEnable()
    {
        base.OnEnable();
        AddChoiceElement();
        DragPanelControl.DragElementsSorted += OnPriorityUpdate;
        AutoChoiceElement.SelectElement += OnSelectEvent;
    }

    private void Start()
    {
        Events.Controllers.Base selectEvent = null;
        foreach (var element in _elements)
        {
            selectEvent = element.Key;
            break;
        }
        if (selectEvent != null)
            OnSelectEvent(selectEvent);
    }

    private protected override void OnDisable()
    {
        base.OnDisable();
        DragPanelControl.DragElementsSorted -= OnPriorityUpdate;
        AutoChoiceElement.SelectElement -= OnSelectEvent;
    }

    private void AddChoiceElement()
    {
        foreach(var gameEvent in AutoChoice.Events)
        {
            if (_elements.ContainsKey(gameEvent.Key))
                continue;

            AutoChoiceElement element = Instantiate(_prefabAutoChoiceElement, _contentElements);
            element.Init(gameEvent.Key);
            _elements.Add(gameEvent.Key, element);
        }

        if (_elements.Count > AutoChoice.Events.Count)
        {
            foreach (var gameEvent in _elements)
            {
                if (AutoChoice.Events.ContainsKey(gameEvent.Key))
                    continue;

                Destroy(gameEvent.Value.gameObject);
                _elements.Remove(gameEvent.Key);
            }
        }
    }

    private void OnSelectEvent(Events.Controllers.Base selectEvent)
    {
        _selectEvent = selectEvent;
        _autoChoicePriority = new();
        _title.text = selectEvent.Local("Title");
        _description.text = selectEvent.Local("Description");
        _autoEnable.isOn = selectEvent.AutoChoice;
        _eventImage.sprite = selectEvent.PanelSprite;
        List<Desidion> desidions = AutoChoice.Events[selectEvent];
        for (int i = 0; i < _dragPanel.Elements.Count; i++)
        {
            _dragPanel.Elements[i].GetComponent<AutoChoiceDesidion>().Title.text = desidions[i].Title;
            _autoChoicePriority.Add(_dragPanel.Elements[i], desidions[i]);
        }
    }

    private void OnPriorityUpdate()
    {
        List<Desidion> newDesidions = new (AutoChoice.Events[_selectEvent]);
        
        int index = 0;
        foreach (var dragElement in _dragPanel.Elements)
        {
            newDesidions[index] = _autoChoicePriority[dragElement];
            index++;
        }

        AutoChoiceUpdate?.Invoke(_selectEvent, newDesidions, _autoEnable.isOn);
    }

    public void AutoChoiceEnable() => AutoChoiceModeUpdate?.Invoke(_selectEvent, _autoEnable.isOn);
}

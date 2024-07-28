using Events;
using Events.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoChoicePanel : MonoBehaviour
{
    [Header("AutoChoice Events List")]
    [SerializeField] private AutoChoiceElement _prefabAutoChoiceElement;
    [SerializeField] private Transform _contentElements;
    private Dictionary<Events.Controllers.Base, AutoChoiceElement> _elements = new();

    [Header("Event AutoChoice")]
    private Events.Controllers.Base _selectEvent;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Toggle _autoEnable;
    [SerializeField] private Image _eventImage;
    [SerializeField] private Transform _autoPriority;
    private Dictionary<Transform, Desidion> _autoChoicePriority;

    public static Action<Events.Controllers.Base, List<Desidion>> AutoChoiceUpdate;

    private void OnEnable()
    {
        AddChoiceElement();
        DragPanelControl.DragElementsSorted += OnPriorityUpdate;
        AutoChoiceElement.SelectElement += OnSelectEvent;
    }

    private void OnDisable()
    {
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
        for (int i = 0; i < _autoPriority.childCount; i++)
        {
            Transform child = _autoPriority.GetChild(i);
            child.GetComponent<AutoChoiceDesidion>().Title.text = desidions[i].Title;
            _autoChoicePriority.Add(child, desidions[i]);
        }
    }

    private void OnPriorityUpdate()
    {
        List<Desidion> newDesidions = new(_autoChoicePriority.Count);
        foreach (var choice in _autoChoicePriority)
        {
            int index = choice.Key.GetSiblingIndex();
            newDesidions[index] = choice.Value;
        }

        AutoChoiceUpdate?.Invoke(_selectEvent, newDesidions);
    }
}

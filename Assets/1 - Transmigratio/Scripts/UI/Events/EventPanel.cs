using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Events.Controllers;
using UI;
using System;

public class EventPanel : Panel {

    [SerializeField] private Button _close;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _territory;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Image _image;
    [SerializeField] private Toggle _autoChoice;
    [SerializeField] private EventDesidion _desidion;
    [SerializeField] private Transform _desidionsContent;
    private Base _event;

    private List<EventDesidion> _desidions = new();


    public string Title { set => _title.text = value; }
    public string Territory { set => _territory.text = value; }
    public string Description { set => _description.text = value; }
    public Sprite Image { set => _image.sprite = value; }
    public Transform Desidions => _desidionsContent;
    public Button CloseBtn => _close;
    public Base Event { set => _event = value; }

    public bool AutoChoice => _autoChoice.isOn;

    public static event Action<bool> EventPanelOpen;
    public static event Action<bool> EventPanelClose;

    private protected override void OnEnable()
    {
        base.OnEnable();
        EventPanelOpen?.Invoke(true);
    }

    private protected override void OnDisable()
    {
        base.OnDisable();
        EventPanelClose?.Invoke(true);
        _event.AutoChoice = AutoChoice;
    }

    public void CloseWindow()
    {
        Destroy(gameObject);
    }
}

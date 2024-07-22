using System.Collections.Generic;
using UnityEngine.UI;
using Scenes.Game;
using UnityEngine;
using System;
using TMPro;

public class EventPanel : MonoBehaviour {
    public static event Action<bool> EventPanelOpen;
    public static event Action<bool> EventPanelClose;

    [SerializeField] private Button _close;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _territory;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Image _image;
    [SerializeField] private Toggle _dontShowAgain;
    [SerializeField] private EventDesidion _desidion;
    [SerializeField] private Transform _desidionsContent;
    private Events.Controllers.Base _event;

    private List<EventDesidion> _desidions = new();


    public string Title { set => _title.text = value; }
    public string Territory { set => _territory.text = value; }
    public string Description { set => _description.text = value; }
    public Sprite Image { set => _image.sprite = value; }
    public Transform Desidions => _desidionsContent;
    public Button CloseBtn => _close;
    public Events.Controllers.Base Event { set => _event = value; }

    public bool IsShowAgain {
        get => !_dontShowAgain.isOn;
        set => _dontShowAgain.isOn = !value;
    }

    private void OnEnable()
    {
        EventPanelOpen?.Invoke(true);
    }

    private void OnDisable()
    {
        EventPanelClose?.Invoke(true);
        _event.IsShowAgain = IsShowAgain;
    }

    public void CloseWindow()
    {
        Transmigratio.Instance.IsClickableMarker = true;
        Destroy(gameObject);
    }
}

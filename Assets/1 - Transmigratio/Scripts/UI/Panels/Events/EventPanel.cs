using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Gameplay.Scenarios.Events;
using UI;
using System;

public class EventPanel : Panel {
    [SerializeField] private Button _close;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Image _image;
    [SerializeField] private EventDesidion _desidion;
    [SerializeField] private Transform _desidionsContent;

    [Header("Tutorial")]
    [SerializeField] private GameObject _bgTutorial;
    [SerializeField] private GameObject _tutorial;

    private Base _event;

    private List<EventDesidion> _desidions = new();


    public string Title { set => _title.text = value; }
    public TMP_Text Description => _description;
    public Sprite Image { set => _image.sprite = value; }
    public Transform Desidions => _desidionsContent;
    public Button CloseBtn => _close;
    public Base Event { set => _event = value; }

    public static event Action<bool> EventPanelOpen;
    public static event Action<bool> EventPanelClose;

    private protected override void OnEnable()
    {
        base.OnEnable();
        Tutorial.OnShowTutorial += ShowTutorial;
        EventPanelOpen?.Invoke(true);
    }

    private protected override void OnDisable()
    {
        base.OnDisable();
        Tutorial.OnShowTutorial -= ShowTutorial;
        EventPanelClose?.Invoke(true);
    }

    public void OpenAutoChoice() => Transmigratio.Instance.HUD.OpenAutoChoice();

    private void ShowTutorial(string tutorial)
    {
        if (tutorial == "Event")
        {
            _bgTutorial.SetActive(true);
            _tutorial.SetActive(true);
        }
    }
}

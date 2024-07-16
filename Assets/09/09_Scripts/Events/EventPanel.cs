using System.Collections.Generic;
using UnityEngine.UI;
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

    private List<EventDesidion> _desidions = new();

    public string Title { set => _title.text = value; }
    public string Territory { set => _territory.text = value; }
    public string Description { set => _description.text = value; }
    public Sprite Image { set => _image.sprite = value; }
    public Transform Desidions => _desidionsContent;
    public Button CloseBtn => _close;

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
    }

    public void Open() {
        Transmigratio.Instance.IsClickableMarker = false;
        ClearDesidions();
        gameObject.SetActive(true);
    }

    public void Close() {
        Transmigratio.Instance.IsClickableMarker = true;
        gameObject.SetActive(false);
        EventPanelClose?.Invoke(true);
        Timeline.Instance.Resume();
    }

    public void CloseWindow()
    {
        Transmigratio.Instance.IsClickableMarker = true;
        Destroy(gameObject);
    }

    public void AddDesidion(Action onClick, string title, int points) {
        var _desidion = Instantiate(this._desidion, _desidionsContent);
        _desidion.ActionClick = onClick;
        _desidion.Title = title;
        _desidion.Points = points;
        _desidions.Add(_desidion);
    }

    public void ClearDesidions() {
        for (int i = 0; i < _desidions.Count; ++i) {
            _desidions[i].Destroy();
        }
        _desidions.Clear();
    }
}

using UnityEngine;
using System;
using TMPro;
using Events.Data;

public class EventDesidion : MonoBehaviour {
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _points;
    private bool _init = false;
    private Desidion _desidion;

    public event Func<Func<int>, bool> ActionClick;
    public event Action Close;

    public string Title { set => _title.text = value; }
    public int Points { set => _points.text = $"{value} {Localization.Load("Events", "Points")}"; }

    public void Init(Desidion desidion)
    {
        if (_init) return;

        _init = true;
        _desidion = desidion;
    }

    public void Click()
    {
        if (ActionClick.Invoke(_desidion.Cost))
            Close?.Invoke();
    }
}

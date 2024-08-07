using UnityEngine;
using System;
using TMPro;
using Events.Data;

public class EventDesidion : MonoBehaviour {
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _points;
    private bool _init = false;
    private Desidion _desidion;
    private CivPiece _piece;

    public event Func<CivPiece, Func<CivPiece, int>, bool> ActionClick;
    public event Action Close;

    public string Title { set => _title.text = value; }
    public int Points { set => _points.text = $"{value} {Localization.Load("Events", "Points")}"; }

    public void Init(CivPiece piece, Desidion desidion)
    {
        if (_init) return;

        _init = true;
        _desidion = desidion;
        _piece = piece;
    }

    public void Click()
    {
        if (ActionClick.Invoke(_piece, _desidion.CostFunc))
            Close?.Invoke();
    }
}

using Gameplay.Scenarios.Events.Data;
using UnityEngine;
using System;
using TMPro;

public class EventDesidion : MonoBehaviour {
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _points;
    private bool _init = false;
    private DesidionPiece _desidionP;
    private DesidionRegion _desidionR;
    private CivPiece _piece;
    private TM_Region _region;

    public event Action Close;

    public string Title { set => _title.text = value; }
    public int Points { set => _points.text = $"<sprite=\"Icons-Inter\" name=\"Intervention\">{value}"; }

    public void Init(CivPiece piece, DesidionPiece desidion)
    {
        if (_init) return;

        _init = true;
        _desidionP = desidion;
        _piece = piece;
    }
    public void Init(TM_Region region, DesidionRegion desidion)
    {
        if (_init) return;

        _init = true;
        _desidionR = desidion;
        _region = region;
    }

    public void Click()
    {
        if (_desidionP.ActionClick != null) {
            if (_desidionP.ActionClick.Invoke(_piece, _desidionP.CostFunc))
                Close?.Invoke();
        }            
        else {
            if (_desidionR.ActionClick.Invoke(_region, _desidionR.CostFunc))
                Close?.Invoke();
        }
    }
}

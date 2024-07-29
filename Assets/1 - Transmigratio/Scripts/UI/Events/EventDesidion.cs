using UnityEngine;
using System;
using TMPro;
using Events.Data;

public class EventDesidion : MonoBehaviour {
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _points;
    private Desidion _desidion;

    public Action<Func<int>> ActionClick;

    public string Title { set => _title.text = value; }
    public int Points { set => _points.text = $"{value} {Localization.Load("Events", "Points")}"; }

    public void Click() => ActionClick?.Invoke(_desidion.Cost);
}

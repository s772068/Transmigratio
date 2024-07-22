using UnityEngine;
using System;
using TMPro;

public class EventDesidion : MonoBehaviour {
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _points;

    public Action ActionClick;

    public string Title { set => _title.text = value; }
    public int Points { set => _points.text = $"{value} {Utilits.Localization.Load("Events", "Points")}"; }

    public void Click() => ActionClick?.Invoke();
}

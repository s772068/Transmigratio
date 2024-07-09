using UnityEngine;
using System;
using TMPro;

public class EventDesidion : MonoBehaviour {
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text points;

    public Action<int> onClick;

    public int Index { private get; set; }
    public string Title { set => title.text = value; }
    public int Points { set => points.text = $"{value} {Localization.Load("Events", "Points")}"; }

    public void Click() => onClick?.Invoke(Index);

    public void Destroy() {
        Destroy(gameObject);
    }
}

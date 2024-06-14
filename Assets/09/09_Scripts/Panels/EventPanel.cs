using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class EventPanel : MonoBehaviour {
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text territory;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;
    [SerializeField] private Toggle dontShowAgain;
    [SerializeField] private Transform desidionsContent;

    private List<EventDesidion> desidions = new();

    public Action<int> onClick;

    public CivPiece Piece { private get; set; }
    public string Title { set => title.text = value; }
    public string Territory { set => territory.text = value; }
    public string Description { set => description.text = value; }
    public Sprite Image { set => image.sprite = value; }
    public bool IsShowAgain {
        get => !dontShowAgain.isOn;
        set => dontShowAgain.isOn = !value;
    }

    public void Open() {
        Timeline.Instance.Pause();
        gameObject.SetActive(true);
    }

    public void Close() {
        Timeline.Instance.Play();
        gameObject.SetActive(false);
    }

    public void AddDesidion(string title, int points) {
        PrefabsLoader.Load(out EventDesidion desidion, desidionsContent);
        desidion.Title = title;
        desidion.Points = points;
        desidion.onClick = onClick;
        desidion.Index = desidions.Count;
        desidions.Add(desidion);
    }

    public void ClearDesidions() {
        for (int i = 0; i < desidions.Count; ++i) {
            desidions[i].Destroy();
        }
        desidions.Clear();
    }
}

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class RegionElement : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private Image background;
    [SerializeField] private Image pictogram;
    [SerializeField] private TMP_Text title;

    private string key;

    public Action<string> onClick;

    public string Title {
        set {
            Debug.Log($"RegionElement Title: {value}");
            title.text = StringLoader.Load("Details", value);
            pictogram.sprite = SpritesLoader.LoadPictogram(value);
            key = value;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        onClick?.Invoke(key);
    }

    public void Destroy() => Destroy(gameObject);

    public void Activate() {
        Color color = background.color;
        color.a = 1;
        background.color = color;
    }

    public void Deactivate() {
        Color color = background.color;
        color.a = 0;
        background.color = color;
    }
}

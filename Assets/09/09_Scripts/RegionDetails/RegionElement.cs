using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class RegionElement : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private Image _background;
    [SerializeField] private Image _pictogram;
    [SerializeField] private TMP_Text _title;

    private string _key;

    public Action<string> Click;

    public Sprite Pictogram {
        set => _pictogram.sprite = value;
    }

    public string Title {
        set {
            Debug.Log($"RegionElement Title: {value}");
            _title.text = Localization.Load("Details", value);
            _key = value;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        Click?.Invoke(_key);
    }

    public void Destroy() => Destroy(gameObject);

    public void Activate() {
        Color color = _background.color;
        color.a = 1;
        _background.color = color;
    }

    public void Deactivate() {
        Color color = _background.color;
        color.a = 0;
        _background.color = color;
    }
}

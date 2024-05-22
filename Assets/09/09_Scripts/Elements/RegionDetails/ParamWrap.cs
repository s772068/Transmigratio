using UnityEngine.Localization.Settings;
using UnityEngine.EventSystems; 
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class ParamWrap : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text valTxt;
    [SerializeField] private Slider slider;

    private string key;

    public Action<string> onClick;

    public string Title {
        set {
            title.text = StringLoader.Load(value);
            key = value;
        }
    }
    public float Value { set {
            valTxt.text = value.ToString();
            slider.value = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        onClick?.Invoke(key);
    }

    public void Destroy() => Destroy(gameObject);
}

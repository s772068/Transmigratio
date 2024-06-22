using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class MigrationParamElement : MonoBehaviour {
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text valueTxt;
    [SerializeField] private Slider slider;

    public Action<string> onClickInfo;

    // public string Title { set => title.text = StringLoader.Load(value); }
    public int Value {
        set {
            valueTxt.text = $"{value}";
            slider.value = value;
        }
    }

    public void ClickInfo() => onClickInfo?.Invoke(title.text);

    public void Destroy() {
        Destroy(gameObject);
    }
}

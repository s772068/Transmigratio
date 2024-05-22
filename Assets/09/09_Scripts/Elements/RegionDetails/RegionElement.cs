using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Localization.Settings;

public class RegionElement : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private Image background;
    [SerializeField] private Image pictogram;
    [SerializeField] private TMP_Text title;

    private string key;

    public Action<string> onClick;

    public string Title {
        set {
            title.text = GetLocalizationString(value);
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

    private string GetLocalizationString(string val) {
        return LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", val);
    }
}

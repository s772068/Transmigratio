using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIP_Notifications : MonoBehaviour, IGameConnecter {
    [SerializeField] private Transform content;
    [SerializeField] private GUIE_Notification elementPref;

    private SettingsController settings;

    public GameController GameController {
        set {
            value.Get(out settings);
        }
    }

    public void Open() {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void AddEvent(I_Event e) {
        GUIE_Notification element = Instantiate(elementPref, content);
        element.Notification = $"В регионе {settings.Localization.Map.Countries.Value[e.Region]} : {settings.Localization.Events[e.Index].Name}";
    }

    public void Init() {
    }
}

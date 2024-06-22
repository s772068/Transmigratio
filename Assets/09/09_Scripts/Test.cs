using UnityEngine.Localization.Settings;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class Test : MonoBehaviour {
    [SerializeField] private TMP_Text text;
    private void Awake() {
        text = GetComponent<TMP_Text>();
    }
    void Start() {
        text.text = LocalizationSettings.StringDatabase.GetLocalizedString("EcoCulture", "hunters");
    }
}

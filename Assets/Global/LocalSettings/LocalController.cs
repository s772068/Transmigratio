using UnityEngine.Localization.Settings;
using System.Collections;
using UnityEngine;

public class LocalController : MonoBehaviour, IGameConnecter {
    private bool active = false;

    public int Locale {
        set {
            if (active) return;
            StartCoroutine(SetLocale(value));
        }
    }

    private IEnumerator SetLocale(int localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        active = false;
    }

    public GameController GameController { set { } }

    public void Init() {
    }
}

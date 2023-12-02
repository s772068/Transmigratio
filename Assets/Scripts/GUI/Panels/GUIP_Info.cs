using UnityEngine.UI;
using UnityEngine;

public class GUIP_Info : MonoBehaviour, IGameConnecter {
    [SerializeField] private Text infoTxt;

    private SettingsController settings;
    private WmskController wmsk;

    public GameController GameController {
        set {
            value.Get(out settings);
            value.Get(out wmsk);
        }
    }

    public void Open() {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            Localization();
        }
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    private void Localization() {
        infoTxt.text = string.Format(settings.Localization.Info.Value[0].Value, settings.Localization.Map.Countries.Value[wmsk.SelectedIndex]);
    }

    public void Init() { }
}

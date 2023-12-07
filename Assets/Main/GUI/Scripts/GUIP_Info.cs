using UnityEngine.UI;
using UnityEngine;

public class GUIP_Info : MonoBehaviour, IGameConnecter {
    [SerializeField] private Text infoTxt;

    private TimelineController timeline;
    private SettingsController settings;
    private WmskController wmsk;

    public GameController GameController {
        set {
            value.Get(out timeline);
            value.Get(out settings);
            value.Get(out wmsk);
        }
    }

    public void Open(int regionIndex) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            Localization(regionIndex);
            timeline.Pouse();
        }
    }

    public void Close() {
        timeline.Play();
        gameObject.SetActive(false);
    }

    private void Localization(int regoinIndex) {
        string info = settings.Localization.Info.Value[0].Value;
        infoTxt.text = string.Format(info, settings.Localization.Map.Countries.Value[regoinIndex]);
    }

    public void Init() {
        timeline.OnSelectRegion += Open;
    }
}

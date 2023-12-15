using UnityEngine;

public class InfoController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUIP_Info panel;

    private TimelineController timeline;
    private SettingsController settings;
    private GUIP_Region region;

    private bool isShowFactAboutEarth;
    
    public GameController GameController {
        set {
            value.Get(out timeline);
            value.Get(out settings);
            value.Get(out region);
        }
    }

    public void Init() {
        timeline.OnSelectRegion += SelectRegion;
        panel.OnOpen += timeline.Pouse;
        isShowFactAboutEarth = true;
        StartGame();
    }

    // TODO: В settings.Localization.Info добавить count, для сокращения этих функций в одну
    // Представить эти данные в функции в виде params
    // Перед передачей в panel.Info проверить количество входимых данных, если меньше положенного, заканчивать функцию
    public void StartGame() {
        panel.Info = settings.Localization.Info.Value[0].Value;
        panel.Open();
    }

    public void SelectRegion(int regionIndex) {
        panel.Info = string.Format(settings.Localization.Info.Value[1].Value, settings.Localization.Map.Countries.Value[regionIndex]);
        panel.Open();
        panel.OnClose += timeline.Play;
        timeline.OnSelectRegion += SelectRegion;
    }

    public void RegionInfo() {
        panel.Info = settings.Localization.Info.Value[2].Value;
        panel.Open();
    }

    public void EventInfo() {
        panel.Info = settings.Localization.Info.Value[3].Value;
        panel.Open();
    }

    public void EventResult(string info) {
        panel.Info = info;
        panel.Open();
    }

    public void FactAboutEarth() {
        if (!isShowFactAboutEarth) return;
        panel.Info = settings.Localization.Info.Value[4].Value;
        panel.Open();
    }

    public void EndGame() {
        panel.Info = settings.Localization.Info.Value[5].Value;
        panel.Open();
        panel.OnClose += settings.Exit;
    }
}

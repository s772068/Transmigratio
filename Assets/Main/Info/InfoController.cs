using UnityEngine;
using UnityEngine.UI;

public class InfoController : MonoBehaviour, IGameConnecter {
    [SerializeField] private GUIP_Info panel;

    private TimelineController timeline;
    private SettingsController settings;
    private MapController map;
    private GUIP_Region region;

    public Button RegionDetailsButton;

    private bool isShowFactAboutEarth;
    private bool isUpgradeCivilization;

    
    public GameController GameController {
        set {
            value.Get(out timeline);
            value.Get(out settings);
            value.Get(out region);
            value.Get(out map);
        }
    }

    public void Init() {
        timeline.OnSelectRegion += StartGame;
        Welcome();
        panel.OnOpen += timeline.PauseAndDisableTimeButtons;
        map.OnUpgradeCivilization += UpgradeCivilization;
        isShowFactAboutEarth = true;
        isUpgradeCivilization = true;
        
    }

    // TODO: В settings.Localization.Info добавить count, для сокращения этих функций в одну
    // Представить эти данные в функции в виде params
    // Перед передачей в panel.Info проверить количество входимых данных, если меньше положенного, заканчивать функцию
    public void Welcome() {
        panel.Info = settings.Localization.Info.Value[0].Value;
        panel.Open();;
        //timeline.Pause();
        //timeline.StopTheGame();
        timeline.PauseAndDisableTimeButtons();

        Debug.Log("Welcome()");
    }

    public void StartGame(int regionIndex) {
        panel.Info = string.Format(settings.Localization.Info.Value[1].Value, settings.Localization.Map.Countries.Value[regionIndex]);
        panel.Open();
        panel.OnClose = timeline.Play;
        //timeline.PauseAndDisableTimeButtons();
        RegionDetailsButton.interactable = true;
        Debug.Log("StartGame()");
    }

    public void RegionInfo() {
        panel.Info = settings.Localization.Info.Value[2].Value;
        panel.Open();
        //panel.OnClose = timeline.Pause;
        timeline.Pause();
        Debug.Log("RegionInfo()");
    }

    public void EventInfo() {
        panel.Info = settings.Localization.Info.Value[3].Value;
        panel.Open();
        panel.OnClose = timeline.Pause;
        timeline.Pause();
        Debug.Log("EventInfo()");
    }

    public void EventResult(string info) {
        panel.Info = info;
        panel.Open();
        panel.OnClose = timeline.TurnPlay;
        timeline.Pause();
        Debug.Log("EventResult()");
    }

    public void FactAboutEarth() {
        if (!isShowFactAboutEarth) return;
        panel.Info = settings.Localization.Info.Value[4].Value;
        panel.Open();
        panel.OnClose = timeline.TurnPlay;
        timeline.Pause();
        Debug.Log("FactAboutEarth()");
    }

    public void UpgradeCivilization(float civID) {
        if (!isUpgradeCivilization) return;
        string format = settings.Localization.Info.Value[5].Value;
        panel.Info = string.Format(format, settings.Localization.Map.Countries.Value[(int)(civID * 100)]);
        panel.Open();
        panel.OnClose = timeline.TurnPlay;
        timeline.Pause();
        isUpgradeCivilization = false;
    }

    public void EndGame() {
        panel.Info = settings.Localization.Info.Value[6].Value;
        panel.Open();
        panel.OnClose = timeline.Pause;
        panel.OnClose += settings.Exit;
    }
}

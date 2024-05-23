using UnityEngine.Localization.Settings;
using UnityEngine;
using TMPro;

public class TopPanel : MonoBehaviour {
    [SerializeField] private TMP_Text populationTxt;
    [SerializeField] private TMP_Text yearTxt;

    private int Population => Transmigratio.Instance.tmdb.humanity.TotalEarthPop;
    private int Tick => Timeline.Instance.Tick;

    private void Awake() {
        GameEvents.onTickShow += UpdatePopulation;
        GameEvents.onTickShow += UpdateYear;
    }

    private void OnDestroy() {
        GameEvents.onTickShow -= UpdatePopulation;
        GameEvents.onTickShow -= UpdateYear;
    }

    private void UpdatePopulation() {
        populationTxt.text = StringLoader.Load("population") + Population.ToString("### ### ###");
    }

    private void UpdateYear() {
        int year = 40000 - Tick * GameSettings.yearsByTick;
        yearTxt.text = year.ToString("### ###") + StringLoader.Load("BCE");
    }
}

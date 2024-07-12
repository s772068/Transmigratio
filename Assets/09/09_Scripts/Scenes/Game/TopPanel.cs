using UnityEngine;
using TMPro;

public class TopPanel : MonoBehaviour {
    [SerializeField] private TMP_Text _populationTxt;
    [SerializeField] private TMP_Text _yearTxt;

    private int Population => Transmigratio.Instance.TMDB.humanity.TotalEarthPop;
    private int Tick => Timeline.Instance.Tick;

    private void Awake() {
        GameEvents.TickShow += UpdatePopulation;
        GameEvents.TickShow += UpdateYear;
    }

    private void OnDestroy() {
        GameEvents.TickShow -= UpdatePopulation;
        GameEvents.TickShow -= UpdateYear;
    }

    private void UpdatePopulation() {
        _populationTxt.text = $"{Localization.Load("Base", "Population")}: {Population.ToString("### ### ###")}";
    }

    private void UpdateYear() {
        int year = 40000 - Tick * GameSettings.YearsByTick;
        _yearTxt.text = $"{year.ToString("### ###")} {Localization.Load("Base", "BCE")}";
    }
}

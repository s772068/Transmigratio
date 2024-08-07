using UnityEngine;
using TMPro;

public class TopPanel : MonoBehaviour {
    [SerializeField] private TMP_Text _populationTxt;
    [SerializeField] private TMP_Text _yearTxt;

    private int Population => Transmigratio.Instance.TMDB.humanity.TotalEarthPop;
    private int Tick => Timeline.Instance.Tick;

    private void Awake() {
        Timeline.TickShow += UpdatePopulation;
        Timeline.TickShow += UpdateYear;
    }

    private void OnDestroy() {
        Timeline.TickShow -= UpdatePopulation;
        Timeline.TickShow -= UpdateYear;
    }

    private void UpdatePopulation() {
        _populationTxt.text = $"{Population.ToString("### ### ###")}";
    }

    private void UpdateYear() {
        int year = Transmigratio.Instance.TMDB.Year;
        _yearTxt.text = $"{year.ToString("### ###")} {Localization.Load("Base", "BCE")}";
    }
}

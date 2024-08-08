using UnityEngine;
using TMPro;

public class TopPanel : MonoBehaviour {
    [SerializeField] private TMP_Text _yearTxt;

    private int Tick => Timeline.Instance.Tick;

    private void Awake() {
        Timeline.TickShow += UpdateYear;
    }

    private void OnDestroy() {
        Timeline.TickShow -= UpdateYear;
    }

    private void UpdateYear() {
        int year = Transmigratio.Instance.TMDB.Year;
        _yearTxt.text = $"{year.ToString("### ###")} {Localization.Load("Base", "BCE")}";
    }
}

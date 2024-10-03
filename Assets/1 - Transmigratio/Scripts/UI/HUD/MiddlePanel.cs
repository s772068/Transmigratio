using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MiddlePanel : MonoBehaviour {
    [SerializeField] private TMP_Text _populationTxt;
    [SerializeField] private Slider _populationSlider;

    private int Population => Transmigratio.Instance.TMDB.humanity.TotalEarthPop;
    private int MaxPopulation => Transmigratio.Instance.TMDB.maxPopulation;

    private void Awake() {
        Timeline.TickShow += UpdatePopulation;
    }

    private void OnDestroy() {
        Timeline.TickShow -= UpdatePopulation;
    }

    private void UpdatePopulation() {
        _populationTxt.text = $"{Population.ToString("### ### ###")}";
        _populationSlider.value = Population / MaxPopulation;
    }
}

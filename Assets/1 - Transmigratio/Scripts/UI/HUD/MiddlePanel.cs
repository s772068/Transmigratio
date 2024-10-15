using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MiddlePanel : MonoBehaviour {
    [SerializeField] private TMP_Text _populationTxt;
    [SerializeField] private Slider _populationSlider;

    private Humanity _humanity;
    private string _selectedCiv;

    private void Start() {
        _humanity = Transmigratio.Instance.TMDB.humanity;
        _populationSlider.maxValue = _humanity.maxPopulation;

        Timeline.TickShow += UpdatePopulation;
        Civilizations.Selector.onSelect += SelectCivilization;
        Civilizations.Selector.onUnselect += UnselectCivilization;
    }

    private void OnDestroy() {
        Timeline.TickShow -= UpdatePopulation;
        Civilizations.Selector.onSelect -= SelectCivilization;
        Civilizations.Selector.onUnselect -= UnselectCivilization;
    }

    private void UpdatePopulation() {
        UpdatePopulation(_selectedCiv == null ?
            _humanity.TotalEarthPop : _humanity.Civilizations[_selectedCiv].Population);
    }

    private void UpdatePopulation(int population) {
        _populationTxt.text = $"{population.ToString("### ### ###")}";
        _populationSlider.value = population;
    }

    private void SelectCivilization(string civ) {
        _selectedCiv = civ;
        UpdatePopulation(_humanity.Civilizations[_selectedCiv].Population);
        Debug.Log($"Select: {_humanity.Civilizations[civ].Population}");
    }

    private void UnselectCivilization() {
        _selectedCiv = null;
        UpdatePopulation(_humanity.TotalEarthPop);
        Debug.Log($"Unselect: {_humanity.TotalEarthPop}");
    }
}

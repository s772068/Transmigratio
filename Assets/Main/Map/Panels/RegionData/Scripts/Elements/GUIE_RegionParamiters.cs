using UnityEngine.UI;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class GUIE_RegionParamiters : MonoBehaviour {
    [SerializeField] private Text regionName;
    [SerializeField] private Image portrait;
    [SerializeField] private GUIE_ParamsGroup ecologyGroup;
    [SerializeField] private GUIE_ParamsGroup civilizationGroup;
    
    [HideInInspector] public int regionIndex;
    [HideInInspector] public int civilizationIndex;
    [HideInInspector] public int groupIndex;
    [HideInInspector] public int paramiterIndex;

    public Action OnClickParamiter;
    public Action OnClickParamiterInfo;
    public int State {  private get; set; }

    public SettingsController Settings { private get; set; }
    public MapController Map { private get; set; }

    public void ShowEcology() {
        ecologyGroup.gameObject.SetActive(true);
        civilizationGroup.gameObject.SetActive(false);
        UpdateEcology();
    }

    public void ShowCivilization() {
        civilizationGroup.gameObject.SetActive(true);
        ecologyGroup.gameObject.SetActive(false);
        UpdateCivilization();
    }

    public void ShowPortrait(bool isShow) {
        portrait.gameObject.SetActiveFlexalon(isShow);
    }

    public void ShowParamiter(bool isShow, int _groupIndex, int _paramiterIndex) {
        switch (_groupIndex) {
            case 0: ecologyGroup.Show(_paramiterIndex, isShow); break;
            case 1: civilizationGroup.Show(_paramiterIndex, isShow); break;
        }
    }

    public void LocalizationName() => regionName.text = Settings.Localization.Map.Countries.Value[regionIndex];
    public void LocalizationPartret() => portrait.sprite = Settings.Theme.GetCivilizationSprite(3, -1);

    public void UpdateEcology() {
        UpdateEcology(0);
        UpdateEcology(1);
        ecologyGroup.SetParamiterValue(2, Map.data.GetRegion(regionIndex).GetEcologyParamiter(2).MaxDetail);
        ecologyGroup.SetParamiterValue(3, Map.data.GetRegion(regionIndex).GetEcologyParamiter(3).MaxDetail);
    }

    public void UpdateCivilization() {
        int population = civilizationIndex < 0 ?
            Map.data.GetRegion(regionIndex).GetAllPopulations() :
            Map.data.GetRegion(regionIndex).GetPopulation(civilizationIndex);
        if (population > 0) {
            civilizationGroup.SetParamiterValue(0, population.ToString());
            for (int i = 0; i < civilizationGroup.CountParamiters - 2; ++i) {
                UpdateCivilization(i);
            }
            if (State == 0) {
                civilizationGroup.SetParamiterValue(civilizationGroup.CountParamiters - 1,
                    Settings.Localization.Map.Civilization.
                    Paramiters[Settings.Localization.Map.Civilization.Paramiters.Length - 1].
                    Value[Map.data.GetMaxPopulationsIndex(regionIndex)].Name);
            } else {
                portrait.sprite = Settings.Theme.GetCivilizationSprite(civilizationGroup.CountParamiters - 1, civilizationIndex);
                UpdateCivilization(civilizationGroup.CountParamiters - 2);
            }
        } else {
            civilizationGroup.SetParamiterValue(0, Settings.Localization.Map.Civilization.EmptyPopulation);
            for (int i = 1; i < civilizationGroup.CountParamiters; ++i) {
                civilizationGroup.Show(i, false);
            }
        }
    }

    private void UpdateEcology(int _paramiterIndex) {
        int maxValueIndex = Map.data.GetRegion(regionIndex).GetEcologyParamiter(_paramiterIndex).MaxIndex;
        string str = Settings.Localization.Map.Ecology.Value[_paramiterIndex].Value[maxValueIndex].Name;
        ecologyGroup.SetParamiterValue(_paramiterIndex, str);
    }

    private void UpdateCivilization(int _paramiterIndex) {
        int currentCivilizationIndex = civilizationIndex < 0 ?
            Map.data.GetRegion(regionIndex).GetCivilizationMaxIndex(_paramiterIndex) :
            civilizationIndex;
        if (_paramiterIndex < Map.data.GetRegion(regionIndex).GetCountCivilizationParamiters(currentCivilizationIndex)) {
            int maxValueIndex = Map.data.GetRegion(regionIndex).GetCivilizationMaxIndex(currentCivilizationIndex, _paramiterIndex);
            civilizationGroup.SetParamiterValue(_paramiterIndex + 1,
                Settings.Localization.Map.Civilization.Paramiters[_paramiterIndex].Value[maxValueIndex].Name);
        } else {
            civilizationGroup.Show(_paramiterIndex + 1, false);
        }
    }

    public void ClickEcologyParam(int _paramiterIndex) {
        groupIndex = 0;
        paramiterIndex = _paramiterIndex;
        OnClickParamiter?.Invoke();
    }

    public void ClickCivilizationParam(int _paramiterIndex) {
        groupIndex = 1;
        paramiterIndex = _paramiterIndex;
        OnClickParamiter?.Invoke();
    }
}

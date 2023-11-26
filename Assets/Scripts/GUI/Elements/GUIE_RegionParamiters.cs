using UnityEngine.UI;
using UnityEngine;
using System;

public class GUIE_RegionParamiters : MonoBehaviour {
    [SerializeField] private Text regionName;
    [SerializeField] private Image img;
    [SerializeField] private GUIE_ParamsGroup ecologyGroup;
    [SerializeField] private GUIE_ParamsGroup civilizationGroup;
    
    [HideInInspector] public int regionIndex;
    [HideInInspector] public int civilizationIndex;
    [HideInInspector] public int groupIndex;
    [HideInInspector] public int paramiterIndex;

    public Action OnClickParamiter;
    public Action OnClickParamiterInfo;

    public SettingsController Settings { private get; set; }
    public MapController Map { private get; set; }

    public void ShowParamiter(bool isShow, int _groupIndex, int _paramiterIndex) {
        switch (_groupIndex) {
            case 0: ecologyGroup.Show(isShow, _paramiterIndex); break;
            case 1: civilizationGroup.Show(isShow, _paramiterIndex); break;
        }
    }

    public void LocalizationName() => regionName.text = Settings.Localization.Map.Countries[regionIndex];
    public void LocalizationPartret() => img.sprite = Settings.Theme.GetCivilizationSprite(3, -1);

    public void LocalizationEcology() {
        ecologyGroup.Label = Settings.Localization.Map.Ecology.Name;
        for (int i = 0; i < Settings.Localization.Map.Ecology.Value.Length; ++i) {
            ecologyGroup.SetParamiterLabel(i, Settings.Localization.Map.Ecology.Value[i].Name);
        }
    }

    public void LocalizationCivilization() {
        civilizationGroup.Label = Settings.Localization.Map.Civilization.GroupName;
        civilizationGroup.SetParamiterLabel(0, Settings.Localization.Map.Civilization.Population);
        for (int i = 1; i < civilizationGroup.CountParamiters; ++i) {
            civilizationGroup.SetParamiterLabel(i, Settings.Localization.Map.Civilization.Paramiters[i - 1].Name);
        }
    }

    public void UpdateEcology() {
        UpdateEcology(0);
        UpdateEcology(1);
        ecologyGroup.SetParamiterValue(2, Map.data.Regions[regionIndex].Ecology[2].MaxValue.ToString());
        ecologyGroup.SetParamiterValue(3, Map.data.Regions[regionIndex].Ecology[3].MaxValue.ToString());
    }

    public void UpdateCivilization() {
        int population = civilizationIndex < 0 ?
            Map.data.Regions[regionIndex].AllPopulations :
            Map.data.Regions[regionIndex].Civilizations[civilizationIndex].Population;
        if (population > 0) {
            civilizationGroup.SetParamiterValue(0, population.ToString());
            for (int i = 0; i < civilizationGroup.CountParamiters - 2; ++i) {
                UpdateCivilization(i);
            }
            if (civilizationIndex < 0) {
                civilizationGroup.SetParamiterValue(civilizationGroup.CountParamiters - 1,
                    Settings.Localization.Map.Civilization.
                    Paramiters[Settings.Localization.Map.Civilization.Paramiters.Length - 1].
                    Value[Map.data.Regions[regionIndex].MaxPopulationsIndex].Name);
            } else {
                UpdateCivilization(civilizationGroup.CountParamiters - 2);
            }
        } else {
            civilizationGroup.SetParamiterValue(0, Settings.Localization.Map.Civilization.EmptyPopulation);
            for (int i = 1; i < civilizationGroup.CountParamiters; ++i) {
                civilizationGroup.Show(false, i);
            }
        }
    }

    private void UpdateEcology(int _paromiterIndex) {
        int maxValueIndex = Map.data.Regions[regionIndex].Ecology[_paromiterIndex].MaxIndex;
        string str = Settings.Localization.Map.Ecology.Value[_paromiterIndex].Value[maxValueIndex].Name;
        ecologyGroup.SetParamiterValue(_paromiterIndex, str);
    }

    private void UpdateCivilization(int _paromiterIndex) {
        int currentCivilizationIndex = civilizationIndex < 0 ?
            Map.data.Regions[regionIndex].MaxCivilizationIndex(_paromiterIndex) :
            civilizationIndex;
        int maxValueIndex = Map.data.Regions[regionIndex].Civilizations[currentCivilizationIndex].Paramiters[_paromiterIndex].MaxIndex;
        civilizationGroup.Show(true, _paromiterIndex + 1);
        civilizationGroup.SetParamiterValue(_paromiterIndex + 1,
            Settings.Localization.Map.Civilization.Paramiters[_paromiterIndex].Value[maxValueIndex].Name);
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

using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;
using WorldMapStrategyKit;

public class GUIE_RegionParamiters : MonoBehaviour {
    [SerializeField] private Text regionName;
    [SerializeField] private Image portrait;
    [SerializeField] private Sprite pictogram;
    [SerializeField] private GUIE_ParamsGroup group;
    [SerializeField] private MapController mapController;
    //[SerializeField] private GUIE_ParamsGroup ecologyGroup;
    //[SerializeField] private GUIE_ParamsGroup civilizationGroup;
    
    [HideInInspector] public int regionIndex;
    [HideInInspector] public float civID;
    [HideInInspector] public int groupIndex;
    //[HideInInspector] public int paramiterIndex;

    public Action<string, string> OnClick;
    //public Action OnClickParamiterInfo;
    public int State {  private get; set; }

    public SettingsController Settings { private get; set; }
    public MapController Map { private get; set; }

    public void ShowEcology(Region region) {
        group.Name = "Ecology";
        group.AddNumParamiter(region.Terrain, false, true);
        group.AddNumParamiter(region.Climate, false, true);
        group.AddNumParamiter(region.Flora, true, false);
        group.AddNumParamiter(region.Fauna, true, false);
    }

    public void ShowCivilizations(Map map) {
        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        //var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Civilization", "ProdMode");
        //if (op.IsDone) group.AddParamiter(pictogram, op.Result);
        //else op.Completed += (op) => group.AddParamiter(pictogram, op.Result);

        //civilizationGroup.gameObject.SetActive(true);
        //ecologyGroup.gameObject.SetActive(false);
        //UpdateCivilization();
    }

    public void ShowCivilization(Civilization civ) {
        group.Name = "Civilization";
        //group.AddNumParamiter(civ.terrain, false, true);
        //group.AddNumParamiter(civ.climate, false, true);
        //group.AddNumParamiter(civ.flora, true, false);
        //group.AddNumParamiter(civ.fauna, true, false);
        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        //var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Civilization", "ProdMode");
        //if (op.IsDone) group.AddParamiter(pictogram, op.Result);
        //else op.Completed += (op) => group.AddParamiter(pictogram, op.Result);

        //civilizationGroup.gameObject.SetActive(true);
        //ecologyGroup.gameObject.SetActive(false);
        //UpdateCivilization();
    }

    public void ShowPortrait(bool isShow) {
        //portrait.gameObject.SetActiveFlexalon(isShow);
    }

    public void ShowParamiter(bool isShow, int _groupIndex, int _paramiterIndex) {
        //switch (_groupIndex) {
        //    case 0: ecologyGroup.Show(_paramiterIndex, isShow); break;
        //    case 1: civilizationGroup.Show(_paramiterIndex, isShow); break;
        //}
    }

    // public void LocalizationName() => regionName.text = Settings.Localization.Map.Countries.Value[regionIndex];
    // public void LocalizationPartret() => portrait.sprite = Settings.Theme.GetCivilizationSprite(3, -1);

    //public void UpdateEcology() {
    //    //UpdateEcology(0);
    //    //UpdateEcology(1);
    //    //ecologyGroup.SetParamiterValue(2, (int) Map.data.GetRegion(regionIndex).GetEcologyParamiter(2).MaxDetail);
    //    //ecologyGroup.SetParamiterValue(3, (int) Map.data.GetRegion(regionIndex).GetEcologyParamiter(3).MaxDetail);
    //}

    //public void UpdateCivilization() {
    //    //int population = civID < 0 ?
    //    //    (int) Map.data.GetRegion(regionIndex).GetAllPopulations() :
    //    //    (int) Map.data.GetRegion(regionIndex).GetPopulation(civID);
    //    //if (population > 0) {
    //    //    civilizationGroup.SetParamiterValue(0, population.ToString());
    //    //    for (int i = 0; i < civilizationGroup.CountParamiters - 2; ++i) {
    //    //        UpdateCivilization(i);
    //    //    }
    //    //    if (State == 0) {
    //    //        civilizationGroup.SetParamiterValue(civilizationGroup.CountParamiters - 1,
    //    //            Settings.Localization.Map.Civilization.
    //    //            Paramiters[Settings.Localization.Map.Civilization.Paramiters.Length - 1].
    //    //            Value[(int) Map.data.GetMaxPopulationIndex(regionIndex)].Name);
    //    //    } else {
    //    //        portrait.sprite = Settings.Theme.GetCivilizationSprite(civilizationGroup.CountParamiters - 1, (int)civID);
    //    //        UpdateCivilization(civilizationGroup.CountParamiters - 2);
    //    //    }
    //    //} else {
    //    //    civilizationGroup.SetParamiterValue(0, Settings.Localization.Map.Civilization.EmptyPopulation);
    //    //    for (int i = 1; i < civilizationGroup.CountParamiters; ++i) {
    //    //        civilizationGroup.Show(i, false);
    //    //    }
    //    //}
    //}

    //private void UpdateEcology(int _paramiterIndex) {
    //    //int maxValueIndex = Map.data.GetEcologyParamiter(regionIndex, _paramiterIndex).MaxIndex;
    //    //string str = Settings.Localization.Map.Ecology.Value[_paramiterIndex].Value[maxValueIndex].Name;
    //    //ecologyGroup.SetParamiterValue(_paramiterIndex, str);
    //}

    //private void UpdateCivilization(int _paramiterIndex) {
    //    //float currentCivID = civID < 0 ?
    //    //    Map.data.GetCivilizationMaxIndex(regionIndex, _paramiterIndex) : civID;
    //    //if (_paramiterIndex < Map.data.GetCountCivilizationParamiters(currentCivID)) {
    //    //    int maxValueIndex = Map.data.GetCivilizationMaxIndex(currentCivID, _paramiterIndex);
    //    //    civilizationGroup.SetParamiterValue(_paramiterIndex + 1,
    //    //        Settings.Localization.Map.Civilization.Paramiters[_paramiterIndex].Value[maxValueIndex].Name);
    //    //} else {
    //    //    civilizationGroup.Show(_paramiterIndex + 1, false);
    //    //}
    //}

    public void Click(string groupIndex, string paramiterIndex) {
        print($"Group:{groupIndex}, Paramiter: {paramiterIndex}");
        OnClick?.Invoke(groupIndex, paramiterIndex);
    }

    public void ClickEcologyParam(int _paramiterIndex) {
        //groupIndex = 0;
        //paramiterIndex = _paramiterIndex;
        //OnClickParamiter?.Invoke();
    }

    public void ClickCivilizationParam(int _paramiterIndex) {
        //groupIndex = 1;
        //paramiterIndex = _paramiterIndex;
        //OnClickParamiter?.Invoke();
    }

    public void Start() {
        ShowEcology(mapController.data.GetRegion(0));
        group.OnClick += Click;
    }
}

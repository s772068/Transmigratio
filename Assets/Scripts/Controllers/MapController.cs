using UnityEngine;

public class MapController : BaseController, ISave {
    [SerializeField] private GUI_CountryPanel countryPanel;
    [SerializeField] private GUI_ParamDetailsPanel paramDetailsPanel;
    public S_Country[] countries;

    private SettingsController settings;

    public override GameController GameController {
        set {
            settings = value.Get<SettingsController>();
            value.Get<WmskController>().OnClick += OpenCountryPanel;
        }
    }

    private BaseMapUpdater[] updaters = {
        // new MU_Add(),
        // new MU_Multy()
    };

    public void Save() {
        IOHelper.SaveToJson(new S_Map() {
            countries = countries
        });
    }

    public void Load() {
        IOHelper.LoadFromJson(out S_Map data);
        countries = data.countries;
    }

    public void UpdateParams() {
        for (int i = 0; i < updaters.Length; ++i) {
            updaters[i].Update(ref countries);
            for (int j = 0; j < countries.Length; ++j) {
                updaters[i].Calc(ref countries[j]);
            }
        }
        if (countryPanel.gameObject.activeSelf) UpdateCountryPanel();
        if (paramDetailsPanel.gameObject.activeSelf) UpdateParamDetails();
    }

    public void OpenCountryPanel(int index) {
        if (countryPanel.gameObject.activeSelf) return;
        countryPanel.gameObject.SetActive(true);
        countryPanel.index = index;
        LocalizationCountryPanel();
        UpdateCountryPanel();
        countryPanel.OnClose = () => countryPanel.gameObject.SetActive(false);
        countryPanel.OnClickParam = (int paramIndex) => OpenParamDetailsPanel(paramIndex);
    }

    private void LocalizationCountryPanel() {
        countryPanel.Localization(settings.Localization.Countries[countryPanel.index],
                           settings.Localization.System,
                           settings.Localization.Map);
    }

    private void UpdateCountryPanel() {
        countryPanel.Terrain = settings.Localization.Terrains[FindMaxIndex(countries[countryPanel.index].Terrain)];
        countryPanel.Climate = settings.Localization.Climates[FindMaxIndex(countries[countryPanel.index].Climate)];
        countryPanel.Flora = countries[countryPanel.index].Flora;
        countryPanel.Fauna = countries[countryPanel.index].Fauna;
        countryPanel.Population = countries[countryPanel.index].Population;
        countryPanel.Production = settings.Localization.Productions[FindMaxIndex(countries[countryPanel.index].Production)];
        countryPanel.Economics = settings.Localization.Economics[FindMaxIndex(countries[countryPanel.index].Economics)];
        countryPanel.Goverment = settings.Localization.Goverments[FindMaxIndex(countries[countryPanel.index].Goverment)];
        countryPanel.Civilization = settings.Localization.Civilizations[FindMaxIndex(countries[countryPanel.index].Civilization)];
    }

    public void OpenParamDetailsPanel(int paramIndex) {
        if (paramDetailsPanel.gameObject.activeSelf) paramDetailsPanel.Clear();
        else paramDetailsPanel.gameObject.SetActive(true);
        paramDetailsPanel.Clear();
        paramDetailsPanel.index = paramIndex;
        paramDetailsPanel.CloseTxt = settings.Localization.System.Close;
        InitParamDetails();
        SortParamDetails();
    }

    private void InitParamDetails() {
        switch (paramDetailsPanel.index) {
            case 0:
                paramDetailsPanel.Label = settings.Localization.Map.Terrain;
                for(int i = 0; i < countries[countryPanel.index].Terrain.Length; ++i) {
                    paramDetailsPanel.AddLegend(settings.Theme.TerrainColor(i),
                                                settings.Localization.Terrains[i],
                                                countries[countryPanel.index].Terrain[i]);
                }
                break;
            case 1:
                paramDetailsPanel.Label = settings.Localization.Map.Climate;
                for (int i = 0; i < countries[countryPanel.index].Climate.Length; ++i) {
                    paramDetailsPanel.AddLegend(settings.Theme.ClimateColor(i),
                                                settings.Localization.Climates[i],
                                                countries[countryPanel.index].Climate[i]);
                }
                break;
            case 2:
                paramDetailsPanel.Label = settings.Localization.Map.Production;
                for (int i = 0; i < countries[countryPanel.index].Production.Length; ++i) {
                    paramDetailsPanel.AddLegend(settings.Theme.ProductionColor(i),
                                                settings.Localization.Productions[i],
                                                countries[countryPanel.index].Production[i]);
                }
                break;
            case 3:
                paramDetailsPanel.Label = settings.Localization.Map.Economics;
                for (int i = 0; i < countries[countryPanel.index].Economics.Length; ++i) {
                    paramDetailsPanel.AddLegend(settings.Theme.EconomicsColor(i),
                                                settings.Localization.Economics[i],
                                                countries[countryPanel.index].Economics[i]);
                }
                break;
            case 4:
                paramDetailsPanel.Label = settings.Localization.Map.Goverment;
                for (int i = 0; i < countries[countryPanel.index].Goverment.Length; ++i) {
                    paramDetailsPanel.AddLegend(settings.Theme.GovermentColor(i),
                                                settings.Localization.Goverments[i],
                                                countries[countryPanel.index].Goverment[i]);
                }
                break;
            case 5:
                paramDetailsPanel.Label = settings.Localization.Map.Civilization;
                for (int i = 0; i < countries[countryPanel.index].Civilization.Length; ++i) {
                    paramDetailsPanel.AddLegend(settings.Theme.CivilizationColor(i),
                                                settings.Localization.Civilizations[i],
                                                countries[countryPanel.index].Civilization[i]);
                }
                break;
        }
    }

    private void UpdateParamDetails() {
        switch (paramDetailsPanel.index) {
            case 0: paramDetailsPanel.UpdatePanel(countries[countryPanel.index].Terrain); break;
            case 1: paramDetailsPanel.UpdatePanel(countries[countryPanel.index].Climate); break;
            case 2: paramDetailsPanel.UpdatePanel(countries[countryPanel.index].Production); break;
            case 3: paramDetailsPanel.UpdatePanel(countries[countryPanel.index].Economics); break;
            case 4: paramDetailsPanel.UpdatePanel(countries[countryPanel.index].Goverment); break;
            case 5: paramDetailsPanel.UpdatePanel(countries[countryPanel.index].Civilization); break;
        }
    }

    private void SortParamDetails() {
        switch (paramDetailsPanel.index) {
            case 0: paramDetailsPanel.SortPanel(countries[countryPanel.index].Terrain); break;
            case 1: paramDetailsPanel.SortPanel(countries[countryPanel.index].Climate); break;
            case 2: paramDetailsPanel.SortPanel(countries[countryPanel.index].Production); break;
            case 3: paramDetailsPanel.SortPanel(countries[countryPanel.index].Economics); break;
            case 4: paramDetailsPanel.SortPanel(countries[countryPanel.index].Goverment); break;
            case 5: paramDetailsPanel.SortPanel(countries[countryPanel.index].Civilization); break;
        }
    }


    private int FindMaxIndex(int[] arr) {
        int res = -1;
        int value = -1;
        for (int i = 0; i < arr.Length; ++i) {
            if (arr[i] > value) {
                value = arr[i];
                res = i;
            }
        }
        return res;
    }
}

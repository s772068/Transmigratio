using UnityEngine;

public class MapController : BaseController, ISave {
    [SerializeField] private GUI_CountryPanel countryPanel;
    [SerializeField] private GUI_ParamDetailsPanel paramDetailsPanel;
    public S_Value<int>[] maxMapParamIndexes;
    public S_Country[] countries;

    private SettingsController settings;

    public override GameController GameController {
        set {
            settings = value.Get<SettingsController>();
            value.Get<WmskController>().OnClick += OpenCountryPanel;
        }
    }

    private BaseMapUpdater[] updaters = {
        /*
        new MU_Add(),
        new MU_Multy()
        */
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
        countryPanel.Localization(settings.Localization.Map.Countries[countryPanel.index],
                           settings.Localization.System,
                           settings.Localization.Map);
    }

    private void UpdateCountryPanel() {
        countryPanel.Flora = countries[countryPanel.index].Flora;
        countryPanel.Fauna = countries[countryPanel.index].Fauna;
        countryPanel.Population = countries[countryPanel.index].Population;
        for (int i = 0; i < countries[countryPanel.index].Paramiters.Length; ++i) {
            countryPanel.SetParam(i, settings.Localization.Map.Paramiters[i].Value[countries[countryPanel.index].Paramiters[i].MaxValueIndex]);
        }
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
        paramDetailsPanel.Label = settings.Localization.Map.Paramiters[paramDetailsPanel.index].Name;
        for (int i = 0; i < countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value.Length; ++i) {
            paramDetailsPanel.AddLegend(settings.Theme.GetColor(paramDetailsPanel.index, i),
                                        settings.Localization.Map.Paramiters[paramDetailsPanel.index].Value[i],
                                        countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value[i]);
        }
    }

    private void UpdateParamDetails() {
        paramDetailsPanel.UpdatePanel(countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value);
    }

    private void SortParamDetails() {
        paramDetailsPanel.SortPanel(countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value);
    }

    /*InitCountries
    private void InitCountries() {
        WorldMapStrategyKit.WMSK wmsk = WorldMapStrategyKit.WMSK.instance;
        for (int i = 0; i < countries.Length; ++i) {
            countries[i].Name = settings.Localization.Map.Countries[i];
            if (i > wmsk.countries.Length - 1) break;
            countries[i].Neighbours = new int[wmsk.countries[i].neighbours.Length];
            for (int j = 0; j < wmsk.countries[i].neighbours.Length; ++j) {
                countries[i].Neighbours[j] = wmsk.countries[i].neighbours[j];
            }
        }
    }

    public override void Init() {
        InitCountries();
    }
    */
}

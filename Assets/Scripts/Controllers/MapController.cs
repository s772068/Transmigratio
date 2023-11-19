using UnityEngine.UI;
using UnityEngine;

public class MapController : BaseController, ISave {
    [SerializeField] private Text populationTxt;
    [SerializeField] private GUI_CountryPanel countryPanel;
    [SerializeField] private GUI_ParamDetailsPanel paramDetailsPanel;
    [SerializeField] private GUI_ShortRegionInfo shortRegionInfo;
    public S_Map data;

    private EventsController events;
    private SettingsController settings;

    public override GameController GameController {
        set {
            events = value.Get<EventsController>();
            settings = value.Get<SettingsController>();
            value.Get<WmskController>().OnClick = InitShortRegion;
            shortRegionInfo.OnClick = OpenCountryPanel;
        }
    }

    private BaseMapUpdater[] updaters = {
        /*
        new MU_Add(),
        new MU_Multy()
        */
    };

    public void Save() {
        IOHelper.SaveToJson(data);
    }

    public void Load() {
        IOHelper.LoadFromJson(out data);
    }

    public void UpdateParams() {
        for (int i = 0; i < updaters.Length; ++i) {
            updaters[i].Update(ref data);
        }
        if (countryPanel.gameObject.activeSelf) UpdateCountryPanel();
        if (paramDetailsPanel.gameObject.activeSelf) UpdateParamDetails();
        populationTxt.text = settings.Localization.Map.Population + "\n" + data.AllPopulation;
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
        countryPanel.Flora = data.Countries[countryPanel.index].Flora;
        countryPanel.Fauna = data.Countries[countryPanel.index].Fauna;
        countryPanel.Population = data.Countries[countryPanel.index].Population;
        for (int i = 0; i < data.Countries[countryPanel.index].Paramiters.Length; ++i) {
            countryPanel.SetParam(i, settings.Localization.Map.Paramiters[i].Value[data.Countries[countryPanel.index].Paramiters[i].MaxValueIndex]);
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
        for (int i = 0; i < data.Countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value.Length; ++i) {
            paramDetailsPanel.AddLegend(settings.Theme.GetColor(paramDetailsPanel.index, i),
                                        settings.Localization.Map.Paramiters[paramDetailsPanel.index].Value[i],
                                        data.Countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value[i]);
        }
    }

    public void InitShortRegion(int index) {
        shortRegionInfo.CountryName = settings.Localization.Map.Countries[index];
        shortRegionInfo.PopulationName = settings.Localization.Map.Population + "\n" + data.Countries[index].Population;
        shortRegionInfo.EventName = data.Countries[index].Events.Count > 0 ? events.GetEventName(data.Countries[index].Events[0]) : "";
    }

    private void UpdateParamDetails() {
        paramDetailsPanel.UpdatePanel(data.Countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value);
    }

    private void SortParamDetails() {
        paramDetailsPanel.SortPanel(data.Countries[countryPanel.index].Paramiters[paramDetailsPanel.index].Value);
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

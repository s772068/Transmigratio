using UnityEngine;

public class MapController : BaseController, ISave {
    [SerializeField] private GUI_CountryPanel panel;
    public S_Country[] countries;

    private LocalizationController localization;

    public override GameController GameController {
        set {
            localization = value.Get<LocalizationController>();
            value.Get<WmskController>().OnClick += OpenPanel;
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
        if (panel.gameObject.activeSelf) InitPanel(countries[panel.index]);
    }

    public void OpenPanel(int index) {
        if (panel.gameObject.activeSelf) return;
        panel.gameObject.SetActive(true);
        panel.index = index;
        LocalizationPanel(index);
        InitPanel(countries[index]);
        panel.OnClose = () => panel.gameObject.SetActive(false);
        // panel.OnClickResult = (int index) => ClickResult(countryIndex, eventIndex, index);
    }

    private void LocalizationPanel(int index) {
        panel.Localization(localization.Localization.Countries[index],
                           localization.Localization.System,
                           localization.Localization.Map);
    }

    private void InitPanel(S_Country country) {
        panel.Terrain = localization.Localization.Terrains[FindMaxIndex(country.Terrain)];
        panel.Climate = localization.Localization.Climates[FindMaxIndex(country.Climate)];
        panel.Flora = country.Flora;
        panel.Fauna = country.Fauna;
        panel.Population = country.Population;
        panel.Production = localization.Localization.Production[FindMaxIndex(country.Production)];
        panel.Economics = localization.Localization.Economics[FindMaxIndex(country.Economics)];
        panel.Goverment = localization.Localization.Goverments[FindMaxIndex(country.Goverment)];
        panel.Civilization = localization.Localization.Civilizations[FindMaxIndex(country.Civilization)];
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

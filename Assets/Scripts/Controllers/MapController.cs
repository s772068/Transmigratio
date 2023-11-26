using UnityEngine.UI;
using UnityEngine;
using System;

public class MapController : BaseController, ISave {
    [SerializeField] private Text populationTxt;
    public S_Map data;

    private EventsController events;
    private SettingsController settings;

    public Action OnUpdate;

    public override GameController GameController {
        set {
            events = value.Get<EventsController>();
            settings = value.Get<SettingsController>();
            // value.Get<WmskController>().OnClick = InitShortRegion;
            // shortRegionInfo.OnClick = OpenRegionPanel;
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
        OnUpdate?.Invoke();
        // if (regionPanel.gameObject.activeSelf) UpdateRegionPanel();
        // if (paramDetailsPanel.gameObject.activeSelf) UpdateParamDetails();
        // populationTxt.text = settings.Localization.Map.Civilization.Population + "\n" + AllPopulation;
    }

    /*InitCountries*/
    private void InitCountries() {
        WorldMapStrategyKit.WMSK wmsk = WorldMapStrategyKit.WMSK.instance;
        for (int i = 0; i < data.Regions.Length; ++i) {
            data.Regions[i].Name = settings.Localization.Map.Countries[i];
            if (i > wmsk.countries.Length - 1) break;
            data.Regions[i].Neighbours = new int[wmsk.countries[i].neighbours.Length];
            for (int j = 0; j < wmsk.countries[i].neighbours.Length; ++j) {
                data.Regions[i].Neighbours[j] = wmsk.countries[i].neighbours[j];
            }
        }
    }

    public override void Init() {
        InitCountries();
    }
}

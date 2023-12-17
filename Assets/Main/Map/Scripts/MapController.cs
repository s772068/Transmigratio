using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

public class MapController : MonoBehaviour, ISave, IGameConnecter {
    public S_Map data;
    
    private TimelineController timeline;
    private SettingsController settings;

    public Action OnUpdate;

    private IUpdater[] updaters = {
    //    new MU_FoodAppropriated(),
    //    new MU_PopulationGrowth()
    };

    public GameController GameController {
        set {
            value.Get(out timeline);
            value.Get(out settings);
        }
    }

    public void Save() {
        IOHelper.SaveToJson(data);
    }

    public void Load() {
        IOHelper.LoadFromJson(out data);
    }

    public void UpdateParams() {
        for (int i = 0; i < updaters.Length; ++i) {
            updaters[i].Update(data);
        }
        OnUpdate?.Invoke();
    }

    private void InitCountries() {
        WorldMapStrategyKit.WMSK wmsk = WorldMapStrategyKit.WMSK.instance;
        for (int i = 0; i < data.CountRegions; ++i) {
            if (i > wmsk.countries.Length - 1) break;
            for (int j = 0; j < wmsk.countries[i].neighbours.Length; ++j) {
                data.GetRegion(i).AddNeighbour(wmsk.countries[i].neighbours[j]);
            }
            string[] names = {
                "Plain",
                "Forest",
                "Desert",
                "Mountain",
                "Steppe",
                "Tundra"
            };
        }
    }

    private void EmergenceFirstCivilization(int regionIndex) {
        if(regionIndex < 0 || regionIndex > data.CountRegions - 1) return;
        
        S_Paramiter prodMode = new();
        prodMode.AddDetail(100);
        prodMode.AddDetail(0);

        S_Paramiter economics = new();
        economics.AddDetail(100);
        economics.AddDetail(0);
        
        S_Paramiter goverment = new();
        goverment.AddDetail(100);
        goverment.AddDetail(0);

        data.AddCivilizationParamiter(0, prodMode);
        data.AddCivilizationParamiter(0, economics);
        data.AddCivilizationParamiter(0, goverment);
        data.SetPopulation(regionIndex, 0, 1000);
    }

    public void Init() {
        InitCountries();
        timeline.OnUpdateData += UpdateParams;
        timeline.OnSelectRegion += EmergenceFirstCivilization;
        // data.GetRegion(0).GetCivilization(0).Population = 100;
    }

    private void OnDestroy() {
        timeline.OnUpdateData -= UpdateParams;
        timeline.OnSelectRegion -= EmergenceFirstCivilization;
    }
}

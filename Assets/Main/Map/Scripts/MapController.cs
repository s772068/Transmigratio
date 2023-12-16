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
        
        S_Paramiter paramiter = new();
        paramiter.AddDetail(100);
        paramiter.AddDetail(0);

        S_Civilization civilization = new();
        civilization.SetID(0);
        civilization.SetPopulation(1000);
        civilization.SetTakenFood(100);
        civilization.SetGovernmentObstacle(0.4f);
        civilization.AddParamiter(paramiter);
        civilization.AddParamiter(paramiter);
        civilization.AddParamiter(paramiter);

        data.AddCivilization(regionIndex, civilization);
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

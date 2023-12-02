using UnityEngine;
using System;

public class MapController : MonoBehaviour, ISave, IGameConnecter {
    public S_Map data;
    
    private TimelineController timeline;
    private SettingsController settings;

    public Action OnUpdate;

    private IUpdater[] updaters = {
        new MU_FoodAppropriated(),
        new MU_PopulationGrowth()
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
        for (int i = 0; i < data.Regions.Length; ++i) {
            data.Regions[i].Name = settings.Localization.Map.Countries.Value[i];
            if (i > wmsk.countries.Length - 1) break;
            data.Regions[i].Neighbours = new int[wmsk.countries[i].neighbours.Length];
            for (int j = 0; j < wmsk.countries[i].neighbours.Length; ++j) {
                data.Regions[i].Neighbours[j] = wmsk.countries[i].neighbours[j];
            }
        }
    }

    private void EmergenceFirstCivilization(int regionIndex) {
        // data[1, 0, 1, -1, -1, -1] = 100;
        data.Regions[regionIndex].Civilizations = data.Regions[regionIndex].Civilizations.Add(new() {
            Stage = 0,
            Population = 1000,
            Paramiters = new S_Paramiter[] {
                    new() {
                        Details = new int[] { 100, 0 }
                    },
                    new() {
                        Details = new int[] { 100, 0 }
                    },
                    new() {
                        Details = new int[] { 100, 0 }
                    }
                }
        });
    }

    public void Init() {
        InitCountries();
        timeline.OnTick = UpdateParams;
        timeline.OnSelectRegion += EmergenceFirstCivilization;
    }

    private void OnDestroy() {
        timeline.OnSelectRegion -= EmergenceFirstCivilization;
    }
}

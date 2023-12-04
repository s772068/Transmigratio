using UnityEngine.UI;
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
        if(regionIndex < 0 || regionIndex > data.Regions.Length - 1) return;
        data.Regions[regionIndex].Civilizations = data.Regions[regionIndex].Civilizations.Add(new() {
            Stage = 0,
            Population = 1000,
            TakenFood = 100,
            Paramiters = new S_Paramiter[] {
                    new() {
                        Name = "ProdMode",
                        details = new S_Value<int>[] {
                            new() { Name = "Primitive communism", Value = 100 },
                            new() { Name = "Slave society", Value = 0 }
                        },
                        detailsNamesIndexes = new() {
                            { "Primitive communism", 0 },
                            { "Slave society", 1 }
                        }
                    },
                    new() {
                        Name = "EcoCulture",
                        details = new S_Value<int>[] {
                            new() { Name = "Hunters-collectors", Value = 100 },
                            new() { Name = "Farmers", Value = 0}
                        },
                        detailsNamesIndexes = new() {
                            { "Hunters-collectors", 0 },
                            { "Farmers", 1 }
                        }
                    },
                    new() {
                        Name = "Government",
                        details = new S_Value<int>[] {
                            new() { Name = "Leaderism", Value = 100 },
                            new() { Name = "Monarchy", Value = 0 }
                        },
                        detailsNamesIndexes = new() {
                            { "Leaderism communism", 0 },
                            { "Slave Monarchy", 1 }
                        }
                    }
                },
            ParamitersNameIndexes = new() {
                {"ProdMode", 0 },
                {"EcoCulture", 1 },
                {"Government", 2 },
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

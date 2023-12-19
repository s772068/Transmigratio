using UnityEngine;
using System;

public class MapController : MonoBehaviour, ISave, IGameConnecter {
    public S_Map data;
    
    private TimelineController timeline;

    public Action OnUpdate;

    private IUpdater[] updaters = {
        new MU_FoodAppropriated(),
        new MU_PopulationGrowth()
    };

    public GameController GameController {
        set {
            value.Get(out timeline);
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

    private void EmergenceFirstCivilization(int regionIndex) {
        if(regionIndex < 0 || regionIndex > data.CountRegions - 1) return;

        float civID = (regionIndex + 1) / 100f;

        S_Paramiter prodMode = new();
        prodMode.AddDetail(100);
        prodMode.AddDetail(0);

        S_Paramiter economics = new();
        economics.AddDetail(100);
        economics.AddDetail(0);
        
        S_Paramiter goverment = new();
        goverment.AddDetail(100);
        goverment.AddDetail(0);

        data.AddCivilizationParamiter(civID, prodMode);
        data.AddCivilizationParamiter(civID, economics);
        data.AddCivilizationParamiter(civID, goverment);
        data.SetReserveFood(civID, 100);
        data.SetGovernmentObstacle(civID, 0.4f);
        data.SetPopulation(regionIndex, civID, 1000);
    }

    public void InitEcology() {
        IOHelper.LoadFromJson(out InitEcology_Root init);
        for(int i = 0; i < data.CountRegions; ++i) {
            data.SetEcologyDetail(i, 0, 0, init.regions[i].terrain.plain);
            data.SetEcologyDetail(i, 0, 1, init.regions[i].terrain.forest);
            data.SetEcologyDetail(i, 0, 2, init.regions[i].terrain.desert);
            data.SetEcologyDetail(i, 0, 3, init.regions[i].terrain.mountain);
            data.SetEcologyDetail(i, 0, 4, init.regions[i].terrain.steppe);
            data.SetEcologyDetail(i, 0, 5, init.regions[i].terrain.tundra);

            data.SetEcologyDetail(i, 1, 0, init.regions[i].climate.ecuatorial);
            data.SetEcologyDetail(i, 1, 1, init.regions[i].climate.tropical);
            data.SetEcologyDetail(i, 1, 2, init.regions[i].climate.moderate);
            data.SetEcologyDetail(i, 1, 3, init.regions[i].climate.polar);

            float.TryParse(init.regions[i].flora, out float flora);
            data.SetEcologyDetail(i, 2, 0, flora);

            float.TryParse(init.regions[i].flora, out float fauna);
            data.SetEcologyDetail(i, 3, 0, fauna);
        }
    }

    public void Init() {
        //InitEcology();
        timeline.OnUpdateData += UpdateParams;
        timeline.OnSelectRegion += EmergenceFirstCivilization;
    }

    private void OnDestroy() {
        timeline.OnUpdateData -= UpdateParams;
        timeline.OnSelectRegion -= EmergenceFirstCivilization;
    }
}

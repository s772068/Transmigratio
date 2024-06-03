using UnityEngine;

public struct VolcanoEvent : I_Event {
    private const float MIGRATE_PERCENT = 10f;
    private const int COST_INTERVENTION = 25;

    private OldMigrationController migration;
    private ResourcesController resources;
    private SettingsController settings;
    private InfoController info;
    private MapController map;

    private int region;
    private int migrateRegion;
    private int migratePopulation;

    public int Index => 0;
    public int Region => region;
    public int MigrateRegion => migrateRegion;
    public int MigratePopulation => migratePopulation;
    public int CountResults => 3;

    public GameController Game {
        set {
            value.Get(out migration);
            value.Get(out resources);
            value.Get(out settings);
            value.Get(out info);
            value.Get(out map);
        }
    }

    public bool TryActivate() {
        // Population > 0
        // Раз в заданное время происходит в случайном регионе
        if(map.data.CountCivilizations == 0) return false;
        map.data.GetRandomCivilization().GetRandomRegion(out region);
        return map.data.GetPopulations(region) > 0;
    }

    public bool CheckBuild(int index) =>
        index switch {
            0 => true,
            1 => CheckMigration(),
            2 => resources.intervention <= 100 - COST_INTERVENTION,
            _ => false
        };

    private bool CheckMigration() {
        if (!map.data.GetRandomNeighbour(Region, out migrateRegion)) return false;
        migratePopulation = (int) (map.data.GetRegion(migrateRegion).GetAllPopulations() * MIGRATE_PERCENT / 100f);
        return true;
    }

    public bool Use(int result) =>
        result switch {
            0 => Activate(),
            1 => Migration(),
            2 => Intervene(),
            _ => false
        };

    private bool Activate() {
        // (Населиние && Флора && Фауна) / 2
        Debug.Log("Activate");
        map.data.SetEcologyDetail(Region, 2, 0, map.data.GetEcologyDetail(Region, 2, 0) / 2);
        map.data.SetEcologyDetail(Region, 3, 0, map.data.GetEcologyDetail(Region, 3, 0) / 2);
        for(int i = 0; i < map.data.GetCountCivilizations(Region); ++i) {
            float civID = map.data.GetCivilizationID(Region, 0);
            map.data.SetPopulation(Region, civID, map.data.GetPopulations(region) / 2);
        }
        info.EventResult(settings.Localization.Events[Index].Results[0].Info);
        return true;
    }

    private bool Migration() {
        Debug.Log("Migration");
        // Все переходят в соседний регион
        float civID = map.data.GetMaxPopulationIndex(Region);
        if (migration.HasMigration(Region)) {
            migration.AmplifyMigration(Region);
            info.EventResult(settings.Localization.Events[Index].Results[1].Info);
        } else {
            migration.CreateMigration(region, MigrateRegion, civID);
            info.EventResult(settings.Localization.Events[Index].Results[1].Info);
        }
        return true;
    }

    private bool Intervene() {
        Debug.Log("Intervene");
        // (Флора && Фауна) / 2
        map.data.SetEcologyDetail(Region, 2, 0, map.data.GetEcologyDetail(Region, 2, 0) / 2);
        map.data.SetEcologyDetail(Region, 3, 0, map.data.GetEcologyDetail(Region, 3, 0) / 2);
        info.EventResult(settings.Localization.Events[Index].Results[2].Info);
        resources.intervention += COST_INTERVENTION;
        // Realise volcanoes action
        return true;
    }
}

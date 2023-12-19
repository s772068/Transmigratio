using System;
using Unity.VisualScripting;
using UnityEngine;
using WorldMapStrategyKit;

public struct VolcanoEvent : I_Event {
    private const float MIGRATE_PERCENT = 10f;
    private const int COST_INTERVENTION = 3;

    private MigrationController migration;
    private ResourcesController resources;
    private SettingsController settings;
    private InfoController info;
    private MapController map;

    private int region;
    private int migrateRegion;
    private int migratePopulation;

    private float[] chances; // = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f};

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

    public bool Init() {
        map.data.GetRandomCivilization().GetRandomRegion(out region);
        return true;
    }

    public bool TryActivate() {
        // Population > 0
        // Раз в заданное время происходит в случайном регионе
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
        map.data.SetEcologyDetail(Region, 0, 2, map.data.GetEcologyDetail(Region, 0, 2) / 2);
        map.data.SetEcologyDetail(Region, 0, 3, map.data.GetEcologyDetail(Region, 0, 3) / 2);
        for(int i = 0; i < map.data.GetCountCivilizations(Region); ++i) {
            float civID = map.data.GetCivilizationID(Region, 0);
            map.data.SetPopulation(Region, civID, map.data.GetPopulations(region) / 2);
        }
        info.EventResult(settings.Localization.Events[0].Results[0].Info);
        return true;
    }

    private bool Migration() {
        Debug.Log("Migration");
        // Все переходят в соседний регион
        info.EventResult(settings.Localization.Events[0].Results[1].Info);
        float civID = map.data.GetMaxPopulationIndex(Region);
        migration.CreateMigration(region, MigrateRegion, civID);
        return true;
    }

    private bool Intervene() {
        Debug.Log("Intervene");
        // (Флора && Фауна) / 2
        map.data.SetEcologyDetail(Region, 0, 2, map.data.GetEcologyDetail(Region, 0, 2) / 2);
        map.data.SetEcologyDetail(Region, 0, 3, map.data.GetEcologyDetail(Region, 0, 3) / 2);
        info.EventResult(settings.Localization.Events[0].Results[2].Info);
        resources.intervention += COST_INTERVENTION;
        // Realise volcanoes action
        return true;
    }
}

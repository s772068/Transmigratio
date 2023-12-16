using System;
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

    public int Index { get; set; }
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
        region = Randomizer.Random(map.data.CountRegions);
        return true;
    }

    public bool TryActivate() {
        //if(region.GetCountCivilizations() == 0) return false;
        //if (region.GetEventChanceIndex() > -1) {
        //    if (random.NextDouble() <= chances[region.GetEventChanceIndex()]) {
        //        region.SetEventChanceIndex(-1);
        //        return true;
        //    } else {
        //        region.SetEventChanceIndex(region.GetEventChanceIndex() + 1);
        //        return false;
        //    }
        //} else return false;
        return true;
    }

    public bool CheckBuild(int index) =>
        index switch {
            0 => true,
            1 => CheckMigration(),
            2 => resources.intervention >= COST_INTERVENTION,
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
        Debug.Log("Activate");
        info.EventResult(settings.Localization.Events[0].Results[0].Info);
        return true;
    }

    private bool Migration() {
        Debug.Log("Migration");
        info.EventResult(settings.Localization.Events[0].Results[1].Info);
        MigrationData migrationData = new MigrationData {
            From = Region,
            To = MigrateRegion
        };
        //migration.StartMigration(migrationData);
        return true;
    }

    private bool Intervene() {
        Debug.Log("Intervene");
        info.EventResult(settings.Localization.Events[0].Results[2].Info);
        resources.intervention += COST_INTERVENTION;
        // Realise volcanoes action
        return true;
    }
}

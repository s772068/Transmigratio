using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public struct HungerEvent : I_Event {
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
        //    if (map.data.GetRegion(Region).GetCountCivilizations() == 0) return false;
        //    // int farmers = region.AllCivilizationVlaues(1,0);
        //    // int hunters = region.AllCivilizationVlaues(1,1);
        //    // int flora = region.Ecology[2][0];
        //    // int fauna = region.Ecology[3][0];
        //    // int eat = Proportion(farmers, hunters) > 50 ? flora : fauna;
        //    // return eat <= 0;
        return true;
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
            0 => Nothing(),
            1 => Migration(),
            2 => Intervene(),
            _ => false
        };

    public bool Nothing() {
        info.EventResult(settings.Localization.Events[0].Results[0].Info);
        return true;
    }

    public bool Migration() {
        info.EventResult(settings.Localization.Events[0].Results[1].Info);
        map.data.GetRandomCivilizationID(region, out float civID);
        migration.CreateMigration(region, MigrateRegion, civID);
        return true;
    }

    public bool Intervene() {
        info.EventResult(settings.Localization.Events[0].Results[0].Info);
        resources.intervention += COST_INTERVENTION;
        // Realise volcanoes action
        return true;
    }
}

public struct HungerEvent : I_Event {
    private const float MIGRATE_PERCENT = 10f;
    private const int COST_INTERVENTION = 3;

    private MigrationController migration;
    private ResourcesController resources;
    private SettingsController settings;
    private InfoController info;
    private MapController map;

    private int region;
    private float civID;
    private int migrateRegion;
    private int migratePopulation;

    public int Index => 1;
    public int Region => region;
    public float CivID => civID;
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
        return true;
    }

    public bool TryActivate() {
        // Population > 0
        // RequestFood = _population / 150
        // RequestFood < ReserveFood
        bool isBreak = false;
        for(int i = 0; i < map.data.CountRegions; ++i) {
            for(int j = 0; j < map.data.GetCountCivilizations(i); ++j) {
                region = i;
                civID = map.data.GetCivilizationID(region, j);
                if(map.data.GetPopulation(region, civID) > 0 &&
                    map.data.GetPopulation(region, civID) / 150 < map.data.GetReserveFood(civID)) { isBreak = true; break; }
            }
            if (isBreak) break;
        }
        return false;
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
        // Ничего не делать
        info.EventResult(settings.Localization.Events[0].Results[0].Info);
        return true;
    }

    public bool Migration() {
        // Миграция в регион с большей фауной
        info.EventResult(settings.Localization.Events[0].Results[1].Info);
        float civID = map.data.GetMaxPopulationIndex(Region);
        migration.CreateMigration(region, MigrateRegion, civID);
        return true;
    }

    public bool Intervene() {
        // +100 к ReserveFood
        info.EventResult(settings.Localization.Events[0].Results[0].Info);
        resources.intervention += COST_INTERVENTION;
        // Realise volcanoes action
        return true;
    }
}

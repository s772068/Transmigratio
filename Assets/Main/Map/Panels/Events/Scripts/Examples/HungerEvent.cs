using Unity.VisualScripting;
using UnityEngine;

public struct HungerEvent : I_Event {
    private const float MIGRATE_PERCENT = 10f;
    private const int COST_INTERVENTION = 25;

    private MigrationController migration;
    private ResourcesController resources;
    private SettingsController settings;
    private InfoController info;
    private MapController map;

    private int _region;
    private float _civID;

    private int _migrateRegion;
    private int _migratePopulation;

    private float _reserveFood;
    private float _population;
    private float _governmentObstacle;
    private float _requestFood;
    private float _givenFood;
    private float _populationGrowth;

    public int Index => 1;
    public int Region => _region;
    public float CivID => _civID;
    public int MigrateRegion => _migrateRegion;
    public int MigratePopulation => _migratePopulation;
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
        // RequestFood = _population / 150
        // RequestFood < ReserveFood
        for(int i = 0; i < map.data.CountRegions; ++i) {
            for(int j = 0; j < map.data.GetCountCivilizations(i); ++j) {
                _region = i;
                _civID = map.data.GetCivilizationID(_region, j);
                _reserveFood = map.data.GetReserveFood(_civID);
                _population = map.data.GetPopulations(_civID);
                _governmentObstacle = map.data.GetGovernmentObstacle(_civID);
                _requestFood = _population / 150;
                _givenFood = _reserveFood > _requestFood ? _requestFood : _reserveFood;
                _populationGrowth = (int) (((_requestFood - _givenFood) == 0f ? _population : -_population) / 100 * _givenFood * _governmentObstacle);

                if (_population > 0 && _populationGrowth < 0) {
                    return true;
                }
            }
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
        if (!map.data.GetRandomNeighbour(Region, out _migrateRegion)) return false;
        _migratePopulation = (int) (map.data.GetRegion(_migrateRegion).GetAllPopulations() * MIGRATE_PERCENT / 100f);
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
        info.EventResult(settings.Localization.Events[Index].Results[0].Info);
        return true;
    }

    public bool Migration() {
        // Миграция в регион с большей фауной
        if (migration.HasMigration(Region)) {
            migration.AmplifyMigration(Region);
            info.EventResult(settings.Localization.Events[Index].Results[1].Info);
        } else {
            migration.CreateMigration();
            info.EventResult(settings.Localization.Events[Index].Results[1].Info);
        }
        return true;
    }

    public bool Intervene() {
        // +100 к ReserveFood
        map.data.SetReserveFood(_civID, map.data.GetReserveFood(_civID) + 100);
        info.EventResult(settings.Localization.Events[Index].Results[2].Info);
        resources.intervention += COST_INTERVENTION;
        // Realise volcanoes action
        return true;
    }
}

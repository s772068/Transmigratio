using Unity.VisualScripting.FullSerializer;

public struct MU_PopulationGrowth : IUpdater {
    private S_Map _map;

    private int _regionIndex;
    private int _civIndex;

    private int _population;
    private float _takenFood;

    private int _populationGrowth;
    private float _takenFoodForTick;
    private float _governmentObstacle;

    // private int Population { set => _map[1, _regionIndex, 4, _civIndex, 1, -1, -1] = value; }
    // private int TakenFood { set => _map[1, _regionIndex, 4, _civIndex, 2, -1, -1] = value;}

    public void Update(S_Map map) {
        _map = map;
        for(_regionIndex = 0; _regionIndex < map.CountRegions; ++_regionIndex) {
            Update(map.GetRegion(_regionIndex));
        }
    }

    private void Update(S_Region region) {
        for (_civIndex = 0; _civIndex < region.GetCountCivilizations(); ++_civIndex) {
            Update(region.GetCivilization(_civIndex));
        }
    }

    private void Update(S_Civilization civilization) {
        _takenFood = civilization.GetTakenFood();
        _population = civilization.GetPopulation();
        for (int i = 0; i < civilization.GetCountParamiters(); ++i) {
            _governmentObstacle = civilization.GetGovernmentObstacle();
            if (_takenFood > (int) (_population / 100f)) _takenFoodForTick = _population / 10;
            _populationGrowth = (int) ((_population / 100f) * _takenFoodForTick * _governmentObstacle);
            // UnityEngine.Debug.Log("Population: " + (_population / 100f));
            // UnityEngine.Debug.Log("takenFoodForTick: " + _takenFoodForTick);
            // UnityEngine.Debug.Log("GovernmentObstacle: " + _governmentObstacle);
            _takenFood -= _takenFoodForTick;
            _population += _populationGrowth;
        }
        // TakenFood = _takenFood;
        // Population = _population;

        // UnityEngine.Debug.Log(_population);
    }
}

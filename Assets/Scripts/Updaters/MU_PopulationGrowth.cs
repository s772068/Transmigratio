using Unity.VisualScripting.FullSerializer;

public struct MU_PopulationGrowth : IUpdater {
    private S_Map _map;

    private int _regionIndex;
    private int _civIndex;

    private int _population;
    private int _takenFood;

    private int _takenFoodForTick;
    private int _populationGrowth;
    private float _governmentObstacle;

    private int Population { set => _map[1, _regionIndex, 4, _civIndex, 1, -1, -1] = value; }
    private int TakenFood { set => _map[1, _regionIndex, 4, _civIndex, 2, -1, -1] = value;}

    public void Update(S_Map map) {
        _map = map;
        for(_regionIndex = 0; _regionIndex < map.Regions.Length; ++_regionIndex) {
            Update(map.Regions[_regionIndex]);
        }
    }

    private void Update(S_Region region) {
        for (_civIndex = 0; _civIndex < region.Civilizations.Length; ++_civIndex) {
            Update(region.Civilizations[_civIndex]);
        }
    }

    private void Update(S_Civilization civilization) {
        _takenFood = civilization.TakenFood;
        _population = civilization.Population;
        for (int i = 0; i < civilization.Paramiters.Length; ++i) {
            _governmentObstacle = civilization.Stage switch {
                0 => 0.4f,
                1 => 0.45f,
                _ => 0f
            };
            if (_takenFood > (int) (_population / 100f)) _takenFoodForTick = _population / 10;
            _populationGrowth = (int) ((_population / 100f) * _takenFoodForTick * _governmentObstacle);
            // UnityEngine.Debug.Log("Population: " + (_population / 100f));
            // UnityEngine.Debug.Log("takenFoodForTick: " + _takenFoodForTick);
            // UnityEngine.Debug.Log("GovernmentObstacle: " + _governmentObstacle);
            _takenFood -= _takenFoodForTick;
            _population += _populationGrowth;
        }
        TakenFood = _takenFood;
        Population = _population;
        // UnityEngine.Debug.Log(_population);
    }
}

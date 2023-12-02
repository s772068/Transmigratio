public struct MU_PopulationGrowth : IUpdater {
    //void SetPopulation(ref S_Region region, int civilizationStage, int value) {
    //  UnityEngine.Debug.Log(region.Civilizations[region.StageToIndex(civilizationStage)].Population);
    //  UnityEngine.Debug.Log(region.Civilizations[region.StageToIndex(civilizationStage)].Population);
    //UnityEngine.Debug.Log(region.Civilizations.ToArray()[region.StageToIndex(civilizationStage)].Population);
    //UnityEngine.Debug.Log(value);
    //region.Civilizations.ToArray()[region.StageToIndex(civilizationStage)].Population = value;
    //region.Civilizations[region.StageToIndex(civilizationStage)].Population += value;
    //UnityEngine.Debug.Log(region.Civilizations.ToArray()[region.StageToIndex(civilizationStage)].Population);
    //}
    

    private int Population {
        get => _map[1, regionIndex, 5, 0, 1, -1, -1];
        set => _map[1, regionIndex, 5, 0, 1, -1, -1] = value;
    }
    private int TakenFood {
        get => _map[1, regionIndex, 1, -1, -1, -1, -1];
        set => _map[1, regionIndex, 1, -1, -1, -1, -1] = value;
    }

    private int Leaderism => _map[1, regionIndex, 5, 0, 2, 2, 0];
    private int Monarchy => _map[1, regionIndex, 5, 0, 2, 2, 1];

    private S_Map _map;
    private int regionIndex;
    private int state;

    public void Update(S_Map map) {
        _map = map;
        for(int i = 0; i < _map.Regions.Length; ++i) {
            regionIndex = i;
            Population = 19;
            TakenFood = 19;
        }
        //UnityEngine.Debug.Log("Leaderism" + Leaderism);
        //UnityEngine.Debug.Log("Monarchy" + Monarchy);
        // UnityEngine.Debug.Log(_map.Regions[0].Civilizations.Length);


            //_map[1, 0, 1, 0, 0, 0] = 19; true
    }

    private void UpdateRegion(S_Region region) {

    }
}

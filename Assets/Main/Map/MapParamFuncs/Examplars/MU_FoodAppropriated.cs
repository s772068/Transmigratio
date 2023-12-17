public struct MU_FoodAppropriated : IUpdater {
    private S_Map _map;

    private int _regionIndex;
    private int _civIndex;

    private int _flora;
    private int _fauna;
    private int _hunters;
    private int _farmers;
    private int _population;
    private int _takenFood;
    private int _pc;
    private int _slavery;

    private float _kr;
    private float _floraKr;
    private float _faunaKr;
    private float _prodMedeK;
    private float _farmersProportion;
    private float _huntersProportion;
    private float _civNumberRegOfPresence;

    // int TakenFood {
    //     get => _map[1, _regionIndex, 4, _civIndex, 2, -1, -1];
    //     set => _map[1, _regionIndex, 4, _civIndex, 2, -1, -1] = value;
    // }
    // int Flora {
    //     get => _map[1, _regionIndex, 3, 2, 0, -1, -1];
    //     set => _map[1, _regionIndex, 3, 2, 0, -1, -1] = value;
    // }
    // int Fauna {
    //     get => _map[1, _regionIndex, 3, 3, 0, -1, -1];
    //     set => _map[1, _regionIndex, 3, 3, 0, -1, -1] = value;
    // }

    public void Update(S_Map map) {
        _map = map;
        _civNumberRegOfPresence = map.CountCivilizations;
        for (_regionIndex = 0; _regionIndex < map.CountRegions; ++_regionIndex) {
            //Update(map.GetRegion(_regionIndex));
        }
    }
    //public void Update(S_Region region) {
        //for (_civIndex = 0; _civIndex < region.GetCountCivilizations(); ++_civIndex) {
        //    //_flora = region.Ecology[2][0];
        //    //_fauna = region.Ecology[3][0];

        //    _floraKr = _flora > 50 ? 1.1f : 0.9f;
        //    _faunaKr = _fauna > 50 ? 1.1f : 0.9f;

        //    Update(region.GetCivilization(_civIndex));
        //}
    //}

    //public void Update(S_Civilization civilization) {
        //_population = civilization.GetPopulation();

        //// _pc = civilization.Paramiters[0][0]; ;
        //// _slavery = civilization.Paramiters[0][1];
        //// 
        //// _hunters = civilization.Paramiters[1][0];
        //// _farmers = civilization.Paramiters[1][1];

        //_farmersProportion = _farmers.Proportion(_hunters);
        //_huntersProportion = _hunters.Proportion(_farmers);

        //_kr = _floraKr * _farmersProportion +
        //      _faunaKr * _huntersProportion;

        //_prodMedeK = _pc.Proportion(_slavery) * 0.6f +
        //    _slavery.Proportion(_pc) * 0.7f;

        // TakenFood = _takenFood = (int) (_population / 61f * _kr * _prodMedeK);
        // 
        // Flora = (int) (_flora -
        //                _takenFood * _farmersProportion /
        //                (100 * _civNumberRegOfPresence));
        // Fauna = (int) (_fauna -
        //                _takenFood * _huntersProportion /
        //                (100 * _civNumberRegOfPresence));
    //}
}

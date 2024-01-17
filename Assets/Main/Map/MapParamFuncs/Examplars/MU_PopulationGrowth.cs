public struct MU_PopulationGrowth : IUpdater {
    //private int _regionIndex;
    private float _civID;

    private float _population;
    private float _populationGrowth;
    private float _takenFood;
    private float _reserveFood;
    private float _requestFood;
    private float _givenFood;
    private float _governmentObstacle;

    public void Update(Map map) {
        for (int i = 0; i < map.CountCivilizations; ++i) {
            _civID = map.GetArrayCivilizationsID()[i];
            _governmentObstacle = map.GetGovernmentObstacle(_civID);
            _reserveFood = map.GetReserveFood(_civID);
            _population = map.GetPopulations(_civID);
            _takenFood = map.GetTakenFood(_civID);

            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"CivID: {_civID}");
            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"GovernmentObstacle: {_governmentObstacle}");
            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"ReserveFood: {_reserveFood}");
            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"Population: {_population}");
            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"TakenFood: {_takenFood}");

            _requestFood = _population / 150;
            _givenFood = _reserveFood > _requestFood ? _requestFood : _reserveFood;

            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"RequestFood: {_population} / 150 = {_reserveFood}");
            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"GivenFood: {_reserveFood} > {_requestFood} ? {_requestFood} : {_reserveFood} = {_givenFood}");

            _populationGrowth = (int) (((_requestFood - _givenFood) == 0f ? _population : -_population) / 100 * _givenFood * _governmentObstacle);

            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"PopulationGrowth: (({_requestFood} - {_givenFood}) == 0 ? {_population} : {-_population}) / 100 * {_givenFood} * {_governmentObstacle} = {_populationGrowth}");
            // if(_civID == 0.16f || _civID == 0.18f) Debug.Log($"ReserveFood: {_reserveFood} + {_takenFood} - {_givenFood} = {_reserveFood + _takenFood - _givenFood}");

            map.SetPopulation(_civID, _population + _populationGrowth);
            map.SetReserveFood(_civID, _reserveFood + _takenFood - _givenFood);


        }
    }
}

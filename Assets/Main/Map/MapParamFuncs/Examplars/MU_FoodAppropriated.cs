using UnityEngine;
using WorldMapStrategyKit;

public struct MU_FoodAppropriated : IUpdater {
    private int _regionIndex;

    private float _population;
    private float _flora;
    private float _fauna;
    private float _hunters;
    private float _farmers;
    private float _pc;
    private float _slavery;
    private float _takenFood;
    private float _kr;
    private float _floraKr;
    private float _faunaKr;
    private float _prodModeK;
    private float _PCProdModeK; 
    private float _slaveryProdModeK;
    private float _PCProportion;
    private float _slaveryPropotion;
    private float _farmersProportion;
    private float _huntersProportion;

    public void Update(S_Map map) {
        _PCProdModeK = 0.6f; //константа 
        _slaveryProdModeK = 0.7f; //константа
        for (_regionIndex = 0; _regionIndex < map.CountRegions; ++_regionIndex) {
            _population = map.GetPopulations(_regionIndex);
            if (_population == 0) continue;

            _pc      = map.GetCivilizationAllParamtier(_regionIndex, 0, 0);
            _slavery = map.GetCivilizationAllParamtier(_regionIndex, 0, 1);
            _hunters = map.GetCivilizationAllParamtier(_regionIndex, 1, 0);
            _farmers = map.GetCivilizationAllParamtier(_regionIndex, 1, 1);

            _PCProportion      = _pc.Proportion(_slavery);
            _slaveryPropotion  = _slavery.Proportion(_pc);
            _huntersProportion = _hunters.Proportion(_farmers);
            _farmersProportion = _farmers.Proportion(_hunters);

            _takenFood = GetTakenFood(map.GetRegion(_regionIndex));

            map.SetTakenFood(_regionIndex, _takenFood);

            map.SetEcologyDetail(_regionIndex, 2, 0,
                _flora -
                _takenFood * _farmersProportion / 10);
            map.SetEcologyDetail(_regionIndex, 3, 0,
                _fauna -
                _takenFood * _huntersProportion / 10);

            // Update(map.GetRegion(_regionIndex));
        }
    }

    public float GetTakenFood(S_Region region) {
        _flora = region.GetEcologyDetail(2, 0);
        _fauna = region.GetEcologyDetail(3, 0);

        // if (_regionIndex == 0) Debug.Log("Fauna: " + _fauna);

        _floraKr = _flora > 50 ? 1.5f : 0.5f;
        _faunaKr = _fauna > 50 ? 1.5f : 0.5f;
        _kr = _faunaKr * _huntersProportion + _floraKr * _farmersProportion;

        // if (_regionIndex == 0) Debug.Log($"FaunaKr: {_faunaKr}");
        // if (_regionIndex == 0) Debug.Log($"KR: {_faunaKr} * {_huntersProportion} + {_floraKr} * {_farmersProportion} = {_kr}");

        _prodModeK = _PCProdModeK * _PCProportion + _slaveryProdModeK * _slaveryPropotion;
        // if (_regionIndex == 0) Debug.Log($"ProdModeK: {_PCProdModeK} * {_PCProportion} + {_slaveryProdModeK} * {_slaveryPropotion} = {_prodModeK}");
        // if (_regionIndex == 0) Debug.Log($"TakenFood: {region.GetAllPopulations()} / 100 * {_kr} * {_prodModeK} = {region.GetAllPopulations() / 100f * _kr * _prodModeK}");
        // if (_regionIndex == 0) Debug.Log($"--------");

        return _population / 100f * _kr * _prodModeK;
    }
}

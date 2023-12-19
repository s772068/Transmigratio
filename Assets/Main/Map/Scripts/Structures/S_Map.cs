using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;
using System.Linq;

[Serializable]
public class S_Map {
    [SerializeField] private SerializedDictionary<float, S_Civilization> civilizations = new();
    [SerializeField] private List<S_Region> _regions = new();

    #region Civilizations
    public int GetCountRegionsInCivilization(float civID) => civilizations[civID].CountRegions;
    public int CountCivilizations => civilizations.Count;
    public float[] GetArrayCivilizationsID() => civilizations.Keys.ToArray();
    public S_Civilization GetCivilization(int id) => civilizations[id];
    public S_Civilization GetRandomCivilization() => civilizations.Values.ToArray()[civilizations.Count];
    public bool GetRandomRegion(float civID, out int region) => civilizations[civID].GetRandomRegion(out region);
    public int GetCivilizationMaxIndex(float civID, int paramiter) => civilizations[civID].GetMaxIndex(paramiter);
    public void AddCivilizationsRegion(float civID, int region) {
        if (civilizations == null) civilizations = new();
        if (!civilizations.ContainsKey(civID)) civilizations.Add(civID, new());
        civilizations[civID].AddRegion(region);
    }
    public void RemoveCivilization(float civID) => civilizations.Remove(civID);
    public void RemoveCivilizationsRegion(int civID, int region) {
        if (civilizations[civID].HasRegion(region))
            civilizations[civID].RemoveRegion(region);
        
    }

    #endregion

    #region Regions
    public int CountRegions => _regions.Count;
    public float GetCivilizationID(int region, int index) => _regions[region].GetCivilizationID(index);
    public S_Region GetRegion(int index) => _regions[index];
    public Color GetColor(int region) => _regions[region].GetColor();
    public string GetName(int region) => _regions[region].GetName();


    // OnSetColor?.Invoke(region, GetColor(region), value);
    public void SetColor(int region, Color value) => _regions[region].SetColor(value);
    public void SetName(int region, string value) => _regions[region].SetName(value);


    #region Neighbour
    public int GetCountNeighbours(int region) => _regions[region].GetCountNeighbours();
    public bool GetRandomNeighbour(int region, out int neighbour) => _regions[region].GetRandomNeighbour(out neighbour);
    public int GetNeighbour(int region, int index) => _regions[region].GetNeighbour(index);
    public void AddNeighbour(int region, int value) => _regions[region].AddNeighbour(value);
    public void RemoveNeighbour(int region, int value) => _regions[region].RemoveNeighbour(value);
    public void RemoveNeighbourAt(int region, int index) => _regions[region].RemoveNeighbourAt(index);
    public void ClearNeighbours(int region) => _regions[region].ClearNeighbours();
    #endregion


    #region Ecology
    public int GetCountEcologyParamiters(int region) => _regions[region].GetCountEcologyParamiters();
    public int GetCountEcologyDetails(int region, int paramiter) => _regions[region].GetCountEcologyDetails(paramiter);


    public S_Paramiter[] GetEcologyParamiters(int region) => _regions[region].GetEcologyParamiters();
    public float[] GetEcologyDetails(int region, int paramiter) => _regions[region].GetEcologyDetails(paramiter);


    public S_Paramiter GetEcologyParamiter(int region, int index) => _regions[region].GetEcologyParamiter(index);
    public float GetEcologyDetail(int region, int paramiter, int detail) => _regions[region].GetEcologyDetail(paramiter, detail);


    public void SetEcologyParamiter(int region, int paramiter, S_Paramiter value) => _regions[region].SetEcologyParamiter(paramiter, value);
    public void SetEcologyDetail(int region, int paramiter, int detail, float value) => _regions[region].SetEcologyDetail(paramiter, detail, value);
    public void SetEcologyParamiters(int region, S_Paramiter[] paramiters) => _regions[region].SetEcologyParamiters(paramiters);
    public void SetEcologyDetails(int region, int paramiter, float[] details) => _regions[region].SetEcologyDetails(paramiter, details);


    public void AddEcologyParamiter(int region, S_Paramiter value) => _regions[region].AddEcologyParamiter(value);
    public void AddEcologyDetail(int region, int paramiter, float value) => _regions[region].AddEcologyDetail(paramiter, value);


    public void RemoveEcologyParamiter(int region, S_Paramiter value) => _regions[region].RemoveEcologyParamiter(value);
    // public void RemoveEcologyDetail(int region, int paramiter, float value) => _regions[region].RemoveEcologyDetail(paramiter, value);


    public void RemoveEcologyParamiterAt(int region, int value) => _regions[region].RemoveEcologyParamiterAt(value);
    public void RemoveEcologyDetailAt(int region, int paramiter, int index) => _regions[region].RemoveEcologyDetailAt(paramiter, index);


    public void ClearEcologyParamiters(int region) => _regions[region].ClearEcologyParamiters();
    public void ClearEcologyDetails(int region, int paramiter) => _regions[region].ClearEcologyDetails(paramiter);



    public float GetRichness(int region, int paramiter) => _regions[region].GetEcologyRichness(paramiter);
    public float GetEcologyAllValues(int paramiter, int detail) {
        float all = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            all += _regions[i].GetEcologyParamiter(paramiter).GetValue(detail);
        }
        return all;
    }

    public float GetMaxEcologyDetail(int region, int paramiter) => _regions[region].GetEcologyMaxDetail(paramiter);
    public float GetMaxEcologyDetail(int paramiter) {
        float max = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetEcologyMaxDetail(paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }

    public float GetEcologyMaxIndex(int region, int paramiter) => _regions[region].GetEcologyMaxIndex(paramiter);
    public float GetEcologyMaxIndex(int paramiter) {
        float max = -1;
        int index = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetEcologyMaxDetail(paramiter);
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }
    #endregion


    #region Civilizations
    public int GetCountCivilizations(int region) => _regions[region].GetCountCivilizations();
    public int GetCountCivilizationParamiters(float civID) => civilizations[civID].GetCountParamiters();
    public int GetCountCivilizationDetails(float civID, int paramiter) => civilizations[civID].GetCountDetails(paramiter);


    public float[] GetArrayPopulations(int region) => _regions[region].GetArrayPopulations();
    public float[] GetCivilizationDetails(int region, float civID, int paramiter) => civilizations[civID].GetDetails(paramiter);
    public float[] GetCivilizationDetailsByRegion(int region, int paramiter) {
        float[] arr = new float[civilizations[civilizations.Keys.ToArray()[0]].GetCountDetails(paramiter)];
        foreach (var val in civilizations) {
            if (val.Value.HasRegion(region)) {
                for (int detailIndex = 0; detailIndex < civilizations[val.Key].GetCountDetails(paramiter); ++detailIndex) {
                    arr[detailIndex] += civilizations[val.Key].GetDetail(paramiter, detailIndex);
                }
            }
        }
        return arr;
    }

    public bool HasCivilization(int region, float civID) => _regions[region].HasCivilization(civID);

    public bool GetRandomCivilizationID(int region, out float civID) => _regions[region].GetRandomCivilizationID(out civID);
    public float GetPopulation(int region, float civID) => _regions[region].GetPopulation(civID);
    public float GetTakenFood(int region) => _regions[region].GetTakenFood();
    public float GetTakenFood(float civID) {
        float res = 0f;
        for(int i = 0; i < civilizations[civID].CountRegions; ++i) {
            res += GetTakenFood(civilizations[civID].GetRegion(i));
        }
        return res;
    }
    public float GetAllTakenFood() {
        float res = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            res += _regions[i].GetTakenFood();
        }
        return res;
    }

    public float GetReserveFood(float civID) => civilizations[civID].GetReserveFood();
    public float GetGovernmentObstacle(float civID) => civilizations[civID].GetGovernmentObstacle();

    public void CopyCivilization(int from, int to, float civID) {
        if(civID < 1) {
            if(!civilizations.ContainsKey((to + 1) / 100f)) {
                S_Civilization civ = new();
                civ.SetReserveFood(100f);
                civ.SetGovernmentObstacle(civilizations[civID].GetGovernmentObstacle());
                civ.AddRegion(to);
                civ.CopyParamiters(civilizations[civID]);
                
                civilizations.Add((to + 1) / 100f, civ);
                _regions[to].SetPopulation((to + 1) / 100f, 0);
            }
            // civilizations[(to + 1) / 100f] = civ;.Copy(civilizations[civID]);
        } else {
            if (!civilizations.ContainsKey(civID))
                _regions[to].SetPopulation(civID, 0);
        }
    }

    public void SetPopulation(int region, float civID, float population) { AddCivilizationsRegion(civID, region); _regions[region].SetPopulation(civID, population); }
    public void SetTakenFood(int region, float value) => _regions[region].SetTakenFood(value);
    public void SetReserveFood(float civID, float value) => civilizations[civID].SetReserveFood(value);
    public void SetGovernmentObstacle(float civID, float value) => civilizations[civID].SetGovernmentObstacle(value);
    public void SetPopulation(float civID, float _populationGrowth) {
        int countRegionsInCivilization = civilizations[civID].CountRegions;
        for (int j = 0; j < countRegionsInCivilization; ++j) {
            SetPopulation(GetRegionIndexFromCivilization(civID, j), civID, _populationGrowth / countRegionsInCivilization);
        }
    }


    public void AddCivilizationParamiter(float civID, S_Paramiter value) {
        if (civilizations == null) civilizations = new();
        if (!civilizations.ContainsKey(civID)) civilizations.Add(civID, new());
        civilizations[civID].AddParamiter(value);
    }
    
    public float GetPopulations(int region) => _regions[region].GetAllPopulations();
    public float GetPopulations(float civID) {
        float res = 0;
        for(int i = 0; i < civilizations[civID].CountRegions; ++i) {
            res += GetPopulation(civilizations[civID].GetRegion(i), civID);
        }
        return res;
    }
    public float GetPopulations() {
        float res = 0;
        for(int i = 0; i < _regions.Count; ++i) {
            res += _regions[i].GetAllPopulations();
        }
        return res;
    }
    public int GetRegionIndexFromCivilization(float civID, int index) => civilizations[civID].GetRegion(index);

    public float GetCivilizationAllByDetails(float civID, int detail) => civilizations[civID].GetAllByDetail(detail);
    public float GetCivilizationAllParamtier(float civID, int paramiter, int detail) => civilizations[civID].GetAllByDetail(detail);
    public float GetCivilizationAllParamtier(int region, int paramiter, int detail) {
        float all = 0f;
        for(int i = 0; i < _regions[region].GetCountCivilizations(); ++i) {
            all += GetCivilizationAllParamtier(
                _regions[region].GetCivilizationID(i),
                paramiter,
                detail);
        }
        return all;
    }

    public float GetMaxPopulations(int region) => _regions[region].GetAllPopulations();
    public float GetMaxPopulations() {
        float max = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetAllPopulations();
            if (max < value) {
                max = value;
            }
        }
        return max;
    }


    public float GetMaxPopulationIndex(int region) => _regions[region].GetMaxPopulationsIndex();
    public int GetMaxPopulationsIndex() {
        float max = -1;
        int index = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetAllPopulations();
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public float GetCivilizationMaxIndex(int paramiter) {
        float max = -1;
        float index = -1;
        foreach (var civilization in civilizations) {
            float value = civilization.Value.GetMaxDetail(paramiter);
            if (max < value) {
                max = value;
                index = civilization.Key;
            }
        }
        return index;
    }

    public float GetCivilizationMaxIndex(int region, int paramiter) {
        float max = -1;
        float index = -1;
        foreach (var civilization in civilizations) {
            if (civilization.Value.HasRegion(region)) {
                float value = civilization.Value.GetMaxDetail(paramiter);
                if (max < value) {
                    max = value;
                    index = civilization.Key;
                }
            }
        }
        return index;
    }
    #endregion

    //public float GetEat(int region) => _regions[region].GetEat();
    //public void SetEat(int region, int value) => _regions[region].SetEat(value);
    //public float GetEat(int region) {
    //    float farmers = GetCivilizationAllValues(1, 0);
    //    float hunters = GetCivilizationAllValues(1, 1);
    //    float flora = GetEcologyDetail(2, 0);
    //    float fauna = GetEcologyDetail(3, 0);
    //    return farmers.Proportion(hunters) > 50 ? flora : fauna;
    //}
    //public void SetEat(float value) {
    //    float farmers = GetAllValues(1, 0);
    //    float hunters = GetAllValues(1, 1);
    //    if (farmers.Proportion(hunters) > 50) SetEcologyDetail(2, 0, value);
    //    else SetEcologyDetail(3, 0, value);
    //}

    public void ClearAll() => _regions.Clear();

    #endregion
}

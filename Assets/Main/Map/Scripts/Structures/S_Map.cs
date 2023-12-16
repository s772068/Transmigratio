using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;
using System.Linq;

[Serializable]
public class S_Map {
    [SerializeField] private SerializedDictionary<int, List<int>> civilizations = new();
    [SerializeField] private List<S_Region> _regions = new();

    #region Civilizations
    public int CountCivilizations => civilizations.Count;
    public int[] GetCivilizationKeys() => civilizations.Keys.ToArray();
    public int[] GetCivilization(int id) => civilizations[id].ToArray();
    public int GetCivilizationsRegion(int civilizationId, int indexRegion) => civilizations[civilizationId][indexRegion];
    public void AddCivilizationsRegion(int civilizationId, int value) {
        if (civilizations == null) civilizations = new();
        if (!civilizations.ContainsKey(civilizationId)) civilizations.Add(civilizationId, new());
        if (civilizations[civilizationId] == null) civilizations[civilizationId].Add(new());
        for(int i = 0; i < civilizations[civilizationId].Count; ++i) {
            if (civilizations[civilizationId][i] == value) return;
        }
        civilizations[civilizationId].Add(value);
    }
    public void RemoveCivilizationsRegion(int civilizationId, int value) {
        if (!civilizations[civilizationId].Contains(value)) return;
        civilizations[civilizationId].Remove(value);
        if (civilizations[civilizationId].Count == 0)
            civilizations.Remove(civilizationId);
    }
    public void RemoveCivilizationsRegionAt(int civilizationId, int regionIndex) {
        civilizations[civilizationId].RemoveAt(regionIndex);
        if (civilizations[civilizationId].Count == 0)
            civilizations.Remove(civilizationId);
    }

    #endregion

    #region Regions
    public int CountRegions => _regions.Count;
    public S_Region GetRegion(int index) => _regions[index];
    public Color GetColor(int region) => _regions[region].GetColor();
    public string GetName(int region) => _regions[region].GetName();
    public int GetEventChanceIndex(int region) => _regions[region].GetEventChanceIndex();


    // OnSetColor?.Invoke(region, GetColor(region), value);
    public void SetColor(int region, Color value) => _regions[region].SetColor(value);
    public void SetName(int region, string value) => _regions[region].SetName(value);
    public void SetEventChanceIndex(int region, int value) => _regions[region].SetEventChanceIndex(value);


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
    public void RemoveEcologyDetail(int region, int paramiter, float value) => _regions[region].RemoveEcologyDetail(paramiter, value);


    public void RemoveEcologyParamiterAt(int region, int value) => _regions[region].RemoveEcologyParamiterAt(value);
    public void RemoveEcologyDetailAt(int region, int paramiter, int index) => _regions[region].RemoveEcologyDetailAt(paramiter, index);


    public void ClearEcologyParamiters(int region) => _regions[region].ClearEcologyParamiters();
    public void ClearEcologyDetails(int region, int paramiter) => _regions[region].ClearEcologyDetails(paramiter);



    public float GetRichness(int region, int paramiter) => _regions[region].GetEcologyRichness(paramiter);
    public float GetEcologyAllValues(int paramiter, int detail) {
        float all = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            all += _regions[i].GetEcologyParamiter(paramiter).GetDetail(detail);
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
    public int GetCountCivilizationParamiters(int region, int civilization) => _regions[region].GetCountCivilizationParamiters(civilization);
    public int GetCountCivilizationDetails(int region, int civID, int paramiter) => _regions[region].GetCountCivilizationDetails(civID, paramiter);


    public S_Civilization[] GetCivilizations(int region) => _regions[region].GetArrayCivilizations();
    public S_Paramiter[] GetCivilizationParamiters(int region, int civID) => _regions[region].GetArrayCivilizationParamiters(civID);
    public float[] GetCivilizationDetails(int region, int civID, int paramiter) => _regions[region].GetArrayCivilizationDetails(civID, paramiter);
    public float[] GetArrayPopulations(int region) => _regions[region].GetArrayPopulations();
    public float[] ArrayCivilizationParamiters(int region, int paramiter) => _regions[region].GetArrayCivilizationByParamiters(paramiter);


    public bool HasCivilization(int region, int civID) => _regions[region].HasCivilization(civID);
    public bool HasCivilization(int region, S_Civilization civilization) => _regions[region].HasCivilization(civilization);

    public bool GetRandomCivilizationID(int region, out int civID) => _regions[region].GetRandomCivilizationID(out civID);
    public int GetCivilizationID(int region, int id) => _regions[region].GetCivilizationID(id);
    public int GetPopulation(int region, int civID) => _regions[region].GetPopulation(civID);
    public float GetTakenFood(int region, int civID) => _regions[region].GetTakenFood(civID);
    public float GetGovernmentObstacle(int region, int civID) => _regions[region].GetGovernmentObstacle(civID);
    public S_Civilization GetCivilization(int region, int civID) => _regions[region].GetCivilization(civID);
    public S_Paramiter GetCivilizationParamiter(int region, int civID, int paramiter) => _regions[region].GetCivilizationParamiter(civID, paramiter);
    public float GetCivilizationDetail(int region, int civID, int paramiter, int detail) => _regions[region].GetCivilizationDetail(civID, paramiter, detail);
    public float GetAllTakenFood(int civID) {
        float res = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            res += _regions[i].GetTakenFood(civID);
        }
        return res;
    }
    public float GetGovernmentObstacle(int civID) {
        float res = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            res += _regions[i].GetGovernmentObstacle(civID);
        }
        return res;
    }
    public float GetTakenFood() {
        float res = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            for (int j = 0; j < _regions[i].GetCountCivilizations(); ++j) {
                res += _regions[i].GetTakenFood(j);
            }
        }
        return res;
    }
    public float GetGovernmentObstacle() {
        int res = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            for (int j = 0; j < _regions[i].GetCountCivilizations(); ++j) {
                _regions[i].GetGovernmentObstacle(j);
            }
        }
        return res;
    }

    public void CopyCivilization(int region, int civID, S_Civilization civilization) {
        RemoveCivilizationsRegion(civilization.GetID(), region);
        AddCivilizationsRegion(civID, region);
        _regions[region].CopyCivilization(civID, civilization);
    }

    public void SetCivilizationID(int region, int civID, int value) => _regions[region].SetCivilizationID(civID, value);
    public void SetPopulation(int region, int civID, int value) => _regions[region].SetPopulation(civID, value);
    public void SetTakenFood(int region, int civID, int value) => _regions[region].SetTakenFood(civID, value);
    public void SetGovernmentObstacle(int region, int civID, int value) => _regions[region].SetGovernmentObstacle(civID, value);
    public void SetCivilization(int region, int civID, S_Civilization value) => _regions[region].SetCivilization(civID, value);
    public void SetCivilizationParamiter(int region, int civID, int paramiter, S_Paramiter value) => _regions[region].SetCivilizationParamiter(civID, paramiter, value);
    public void SetCivilizationDetail(int region, int civID, int paramiter, int detail, int value) => _regions[region].SetCivilizationDetail(civID, paramiter, detail, value);
    public void SetCivilizations(int region, S_Civilization[] civilizations) => _regions[region].SetCivilizations(civilizations);
    public void SetCivilizationParamiters(int region, int civID, S_Paramiter[] paramiters) => _regions[region].SetCivilizationsParamiters(civID, paramiters);
    public void SetCivilizationDetails(int region, int civID, int paramiter, int[] details) => _regions[region].SetCivilizationsDetails(civID, paramiter, details);


    public void AddCivilization(int region, S_Civilization value) { AddCivilizationsRegion(value.GetID(), region); _regions[region].AddCivilization(value.GetID(), value); }
    public void AddCivilizationParamiter(int region, int civID, S_Paramiter value) => _regions[region].AddCivilizationParamiter(civID, value);
    public void AddCivilizationDetail(int region, int civID, int paramiter, int value) => _regions[region].AddCivilizationDetail(civID, paramiter, value);


    public void RemoveCivilization(int region, S_Civilization civilization) { RemoveCivilizationsRegion(civilization.GetID(), region); _regions[region].RemoveCivilization(civilization); }
    public void RemoveCivilization(int region, int civID) { RemoveCivilizationsRegion(civID, region); _regions[region].RemoveCivilization(civID); }
    public void RemoveCivilizationParamiter(int region, int civID, S_Paramiter value) => _regions[region].RemoveCivilizationParamiter(civID, value);
    public void RemoveCivilizationDetail(int region, int civID, int paramiter, int value) => _regions[region].RemoveCivilizationDetail(civID, paramiter, value);


    public void RemoveCivilizationParamiterAt(int region, int civID, int paramiter) => _regions[region].RemoveCivilizationParamiterAt(civID, paramiter);
    public void RemoveCivilizationDetailAt(int region, int civID, int paramiter, int detail) => _regions[region].RemoveCivilizationDetailAt(civID, paramiter, detail);


    public void ClearCivilizations(int region) => _regions[region].ClearCivilizations();
    public void ClearCivilizationParamiters(int region, int civID) => _regions[region].ClearCivilizationParamiters(civID);
    public void ClearCivilizationDetails(int region, int civID, int paramiter) => _regions[region].ClearCivilizationDetails(civID, paramiter);

    
    public int GetPopulations(int region, int civID) => _regions[region].GetCivilization(civID).GetPopulation();
    public int GetPopulations(int region) => _regions[region].GetAllPopulations();
    public int GetPopulations() {
        int res = 0;
        for(int i = 0; i < _regions.Count; ++i) {
            res += _regions[i].GetAllPopulations();
        }
        return res;
    }

    public float GetRichnessParamiter(int region, int civID, int paramiter) => _regions[region].GetRichness(civID, paramiter);
    public float GetRichnessParamiter(int civilization, int paramiter) {
        float all = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            all += _regions[i].GetRichness(civilization, paramiter);
        }
        return all;
    }

    public float GetCivilizationAllValues(int region, int paramiter, int detail) => _regions[region].GetAllValues(paramiter, detail);
    public float GetCivilizationAllValues(int paramiter, int detail) {
        float all = 0;
        for (int i = 0; i < _regions.Count; ++i) {
            all += _regions[i].GetAllValues(paramiter, detail);
        }
        return all;
    }


    public int GetMaxPopulations(int region) => _regions[region].GetAllPopulations();
    public int GetMaxPopulations() {
        int max = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            int value = _regions[i].GetAllPopulations();
            if (max < value) {
                max = value;
            }
        }
        return max;
    }
    public float GetMaxCivilizationDetail(int region, int civID, int paramiter) => _regions[region].GetMaxCivilizationDetail(civID, paramiter);
    public float GetMaxCivilizationDetailByCivilization(int civilization, int paramiter) {
        float max = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetMaxCivilizationDetail(civilization, paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }

    public float GetMaxCivilizationDetailByRegion(int region, int paramiter) => _regions[region].GetMaxCivilizationDetail(paramiter);
    public float GetMaxCivilizationDetail(int paramiter) {
        float max = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetMaxCivilizationDetail(paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }


    public int GetMaxPopulationsIndex(int region) => _regions[region].GetMaxPopulationsIndex();
    public int GetMaxPopulationsIndex() {
        int max = -1;
        int index = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            int value = _regions[i].GetAllPopulations();
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public float GetCivilizationMaxIndex(int region, int civilization, int paramiter) => _regions[region].GetCivilizationMaxIndex(paramiter);
    public float GetCivilizationMaxIndexByCivilization(int civilization, int paramiter) {
        float max = -1;
        int index = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetMaxCivilizationDetail(civilization, paramiter);
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public float GetCivilizationMaxIndexByRegion(int region, int paramiter) => _regions[region].GetCivilizationMaxIndex(paramiter);

    public float GetCivilizationMaxIndex(int paramiter) {
        float max = -1;
        int index = -1;
        for (int i = 0; i < _regions.Count; ++i) {
            float value = _regions[i].GetMaxCivilizationDetail(paramiter);
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }
    #endregion


    public float GetEat(int region) => _regions[region].GetEat();
    public void SetEat(int region, int value) => _regions[region].SetEat(value);

    public void AddEvent(int region, int value) => _regions[region].AddEvent(value);
    public void RemoveEvent(int region, int index) => _regions[region].RemoveEvent(index);
    public void ClearEvent(int region) => _regions[region].ClearEvent();

    public void ClearAll() => _regions.Clear();
    #endregion
}

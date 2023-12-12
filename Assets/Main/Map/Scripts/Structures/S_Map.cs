using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct S_Map {
    [SerializeField] private int[][] civilizations;
    [SerializeField] private List<S_Region> regions;

    #region Civilizations
    public int CountCivilizations => civilizations.Length;
    public int CountRegions => regions.Count;
    public int[] GetCivilization(int index) => civilizations[index];
    public S_Region GetRegion(int index) => regions[index];
    public int GetCivilizationsRegion(int indexCivilization, int indexRegion) => civilizations[indexCivilization][indexRegion];
    
    #endregion

    #region Regions
    public Color GetColor(int region) => regions[region].GetColor();
    public string GetName(int region) => regions[region].GetName();
    public int GetEventChanceIndex(int region) => regions[region].GetEventChanceIndex();


    // OnSetColor?.Invoke(region, GetColor(region), value);
    public void SetColor(int region, Color value) => regions[region].SetColor(value);
    public void SetName(int region, string value) => regions[region].SetName(value);
    public void SetEventChanceIndex(int region, int value) => regions[region].SetEventChanceIndex(value);


    #region Neighbour
    public int GetCountNeighbours(int region) => regions[region].GetCountNeighbours();
    public int GetNeighbour(int region, int index) => regions[region].GetNeighbour(index);
    public void AddNeighbour(int region, int value) => regions[region].AddNeighbour(value);
    public void RemoveNeighbour(int region, int value) => regions[region].RemoveNeighbour(value);
    public void RemoveNeighbourAt(int region, int index) => regions[region].RemoveNeighbourAt(index);
    public void ClearNeighbours(int region) => regions[region].ClearNeighbours();
    #endregion


    #region Ecology
    public int GetCountEcologyParamiters(int region) => regions[region].GetCountEcologyParamiters();
    public int GetCountEcologyDetails(int region, int paramiter) => regions[region].GetCountEcologyDetails(paramiter);


    public S_Paramiter[] GetEcologyParamiters(int region) => regions[region].GetEcologyParamiters();
    public int[] GetEcologyDetails(int region, int paramiter) => regions[region].GetEcologyDetails(paramiter);


    public S_Paramiter GetEcologyParamiter(int region, int index) => regions[region].GetEcologyParamiter(index);
    public int GetEcologyDetail(int region, int paramiter, int detail) => regions[region].GetEcologyDetail(paramiter, detail);


    public void SetEcologyParamiter(int region, int paramiter, S_Paramiter value) => regions[region].SetEcologyParamiter(paramiter, value);
    public void SetEcologyDetail(int region, int paramiter, int detail, int value) => regions[region].SetEcologyDetail(paramiter, detail, value);
    public void SetEcologyParamiters(int region, S_Paramiter[] paramiters) => regions[region].SetEcologyParamiters(paramiters);
    public void SetEcologyDetails(int region, int paramiter, int[] details) => regions[region].SetEcologyDetails(paramiter, details);


    public void AddEcologyParamiter(int region, S_Paramiter value) => regions[region].AddEcologyParamiter(value);
    public void AddEcologyDetail(int region, int paramiter, int value) => regions[region].AddEcologyDetail(paramiter, value);


    public void RemoveEcologyParamiter(int region, S_Paramiter value) => regions[region].RemoveEcologyParamiter(value);
    public void RemoveEcologyDetail(int region, int paramiter, int value) => regions[region].RemoveEcologyDetail(paramiter, value);


    public void RemoveEcologyParamiterAt(int region, int value) => regions[region].RemoveEcologyParamiterAt(value);
    public void RemoveEcologyDetailAt(int region, int paramiter, int index) => regions[region].RemoveEcologyDetailAt(paramiter, index);


    public void ClearEcologyParamiters(int region) => regions[region].ClearEcologyParamiters();
    public void ClearEcologyDetails(int region, int paramiter) => regions[region].ClearEcologyDetails(paramiter);



    public int GetRichness(int region, int paramiter) => regions[region].GetEcologyRichness(paramiter);
    public int GetEcologyAllValues(int paramiter, int detail) {
        int all = 0;
        for (int i = 0; i < regions.Count; ++i) {
            all += regions[i].GetEcologyParamiter(paramiter).GetDetail(detail);
        }
        return all;
    }

    public int GetMaxEcologyDetail(int region, int paramiter) => regions[region].GetEcologyMaxDetail(paramiter);
    public int GetMaxEcologyDetail(int paramiter) {
        int max = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetEcologyMaxDetail(paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }

    public int GetEcologyMaxIndex(int region, int paramiter) => regions[region].GetEcologyMaxIndex(paramiter);
    public int GetEcologyMaxIndex(int paramiter) {
        int max = -1;
        int index = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetEcologyMaxDetail(paramiter);
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }
    #endregion


    #region Civilizations
    public int GetCountCivilizations(int region) => regions[region].GetCountCivilizations();
    public int GetCountCivilizationParamiters(int region, int civilization) => regions[region].GetCountCivilizationParamiters(civilization);
    public int GetCountCivilizationDetails(int region, int civilization, int paramiter) => regions[region].GetCountCivilizationDetails(civilization, paramiter);


    public S_Civilization[] GetCivilizations(int region) => regions[region].GetArrayCivilizations();
    public S_Paramiter[] GetCivilizationParamiters(int region, int civilization) => regions[region].GetArrayCivilizationParamiters(civilization);
    public int[] GetCivilizationDetails(int region, int civilization, int paramiter) => regions[region].GetArrayCivilizationDetails(civilization, paramiter);
    public int[] GetPopulations(int region) => regions[region].GetArrayPopulations();
    public int[] ArrayCivilizationParamiters(int region, int paramiter) => regions[region].GetArrayCivilizationByParamiters(paramiter);


    public int GetCivilizationPopulation(int region, int civilization) => regions[region].GetPopulation(civilization);
    public int GetRegionTakenFood(int region, int civilization) => regions[region].GetTakenFood(civilization);
    public float GetRegionGovernmentObstacle(int region, int civilization) => regions[region].GetGovernmentObstacle(civilization);
    public S_Civilization GetCivilization(int region, int civilization) => regions[region].GetCivilization(civilization);
    public S_Paramiter GetCivilizationParamiter(int region, int civilization, int paramiter) => regions[region].GetCivilizationParamiter(civilization, paramiter);
    public int GetCivilizationDetail(int region, int civilization, int paramiter, int detail) => regions[region].GetCivilizationDetail(civilization, paramiter, detail);
    public int GetAllTakenFood(int civilization) {
        int res = 0;
        for (int i = 0; i < regions.Count; ++i) {
            res += regions[i].GetTakenFood(civilization);
        }
        return res;
    }
    public float GetGovernmentObstacle(int civilization) {
        float res = 0;
        for (int i = 0; i < regions.Count; ++i) {
            res += regions[i].GetGovernmentObstacle(civilization);
        }
        return res;
    }
    public int GetTakenFood() {
        int res = 0;
        for (int i = 0; i < regions.Count; ++i) {
            for (int j = 0; j < regions[i].GetCountCivilizations(); ++j) {
                res += regions[i].GetTakenFood(j);
            }
        }
        return res;
    }
    public float GetGovernmentObstacle() {
        int res = 0;
        for (int i = 0; i < regions.Count; ++i) {
            for (int j = 0; j < regions[i].GetCountCivilizations(); ++j) {
                regions[i].GetGovernmentObstacle(j);
            }
        }
        return res;
    }

    public void SetPopulation(int region, int index, int value) => regions[region].SetPopulation(index, value);
    public void SetTakenFood(int region, int index, int value) => regions[region].SetTakenFood(index, value);
    public void SetGovernmentObstacle(int region, int index, int value) => regions[region].SetGovernmentObstacle(index, value);
    public void SetCivilization(int region, int index, S_Civilization value) => regions[region].SetCivilization(index, value);
    public void SetCivilizationParamiter(int region, int civilization, int paramiter, S_Paramiter value) => regions[region].SetCivilizationParamiter(civilization, paramiter, value);
    public void SetCivilizationDetail(int region, int civilization, int paramiter, int detail, int value) => regions[region].SetCivilizationDetail(civilization, paramiter, detail, value);
    public void SetCivilizations(int region, S_Civilization[] civilizations) => regions[region].SetCivilizations(civilizations);
    public void SetCivilizationParamiters(int region, int civilization, S_Paramiter[] paramiters) => regions[region].SetCivilizationsParamiters(civilization, paramiters);
    public void SetCivilizationDetails(int region, int civilization, int paramiter, int[] details) => regions[region].SetCivilizationsDetails(civilization, paramiter, details);


    public void AddCivilization(int region, S_Civilization value) => regions[region].AddCivilization(value);
    public void AddCivilizationParamiter(int region, int civilization, S_Paramiter value) => regions[region].AddCivilizationParamiter(civilization, value);
    public void AddCivilizationDetail(int region, int civilization, int paramiter, int value) => regions[region].AddCivilizationDetail(civilization, paramiter, value);


    public void RemoveCivilization(int region, S_Civilization value) => regions[region].RemoveCivilization(value);
    public void RemoveCivilizationParamiter(int region, int civilization, S_Paramiter value) => regions[region].RemoveCivilizationParamiter(civilization, value);
    public void RemoveCivilizationDetail(int region, int civilization, int paramiter, int value) => regions[region].RemoveCivilizationDetail(civilization, paramiter, value);


    public void RemoveCivilizationAt(int region, int value) => regions[region].RemoveCivilizationAt(value);
    public void RemoveCivilizationParamiterAt(int region, int civilization, int paramiter) => regions[region].RemoveCivilizationParamiterAt(civilization, paramiter);
    public void RemoveCivilizationDetailAt(int region, int civilization, int paramiter, int detail) => regions[region].RemoveCivilizationDetailAt(civilization, paramiter, detail);


    public void ClearCivilizations(int region) => regions[region].ClearCivilizations();
    public void ClearCivilizationParamiters(int region, int civilization) => regions[region].ClearCivilizationParamiters(civilization);
    public void ClearCivilizationDetails(int region, int civilization, int paramiter) => regions[region].ClearCivilizationDetails(civilization, paramiter);

    
    public int GetAllPopulations(int region) => regions[region].GetAllPopulations();
    public int GetAllPopulations() {
        int res = 0;
        for(int i = 0; i < regions.Count; ++i) {
            res += regions[i].GetAllPopulations();
        }
        return res;
    }

    public int GetRichnessParamiter(int region, int civilization, int paramiter) => regions[region].GetRichness(civilization, paramiter);
    public int GetRichnessParamiter(int civilization, int paramiter) {
        int all = 0;
        for (int i = 0; i < regions.Count; ++i) {
            all += regions[i].GetRichness(civilization, paramiter);
        }
        return all;
    }

    public int GetCivilizationAllValues(int region, int paramiter, int detail) => regions[region].GetAllValues(paramiter, detail);
    public int GetCivilizationAllValues(int paramiter, int detail) {
        int all = 0;
        for (int i = 0; i < regions.Count; ++i) {
            all += regions[i].GetAllValues(paramiter, detail);
        }
        return all;
    }


    public int GetMaxPopulations(int region) => regions[region].GetAllPopulations();
    public int GetMaxPopulations() {
        int max = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetAllPopulations();
            if (max < value) {
                max = value;
            }
        }
        return max;
    }
    public int GetMaxCivilizationDetail(int region, int civilization, int paramiter) => regions[region].GetMaxCivilizationDetail(civilization, paramiter);
    public int GetMaxCivilizationDetailByCivilization(int civilization, int paramiter) {
        int max = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetMaxCivilizationDetail(civilization, paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }

    public int GetMaxCivilizationDetailByRegion(int region, int paramiter) => regions[region].GetMaxCivilizationDetail(paramiter);
    public int GetMaxCivilizationDetail(int paramiter) {
        int max = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetMaxCivilizationDetail(paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }


    public int GetMaxPopulationsIndex(int region) => regions[region].GetMaxPopulationsIndex();
    public int GetMaxPopulationsIndex() {
        int max = -1;
        int index = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetAllPopulations();
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public int GetCivilizationMaxIndex(int region, int civilization, int paramiter) => regions[region].GetCivilizationMaxIndex(paramiter);
    public int GetCivilizationMaxIndexByCivilization(int civilization, int paramiter) {
        int max = -1;
        int index = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetMaxCivilizationDetail(civilization, paramiter);
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }

    public int GetCivilizationMaxIndexByRegion(int region, int paramiter) => regions[region].GetCivilizationMaxIndex(paramiter);

    public int GetCivilizationMaxIndex(int paramiter) {
        int max = -1;
        int index = -1;
        for (int i = 0; i < regions.Count; ++i) {
            int value = regions[i].GetMaxCivilizationDetail(paramiter);
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }
    #endregion


    public int GetEat(int region) => regions[region].GetEat();
    public void SetEat(int region, int value) => regions[region].SetEat(value);

    public void AddEvent(int region, int value) => regions[region].AddEvent(value);
    public void RemoveEvent(int region, int index) => regions[region].RemoveEvent(index);
    public void ClearEvent(int region) => regions[region].ClearEvent();

    public void ClearAll() => regions.Clear();
    #endregion
}

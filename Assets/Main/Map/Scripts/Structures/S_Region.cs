// Ecology
// 0: Terrain
// 1: Climat
// 2: Flora
// 3: Fauna

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_Region {
    [SerializeField] private string _name;
    [SerializeField] private Color _color;
    [SerializeField] private int _eventChanceIndex;
    [SerializeField] private List<S_Paramiter> _ecology;
    [SerializeField] private List<S_Civilization> _civilizations;
    [SerializeField] private List<int> _neighbours;
    [SerializeField] private List<int> events;

    public string GetName() => _name;
    public Color GetColor() => _color;
    public int GetEventChanceIndex() => _eventChanceIndex;

    public void SetName(string value) => _name = value;
    public void SetColor(Color value) => _color = value;
    public void SetEventChanceIndex(int value) => _eventChanceIndex = value;

    #region Lists
    #region Neighbours
    public int GetCountNeighbours() => _neighbours.Count;
    public int GetNeighbour(int index) => _neighbours[index];
    public void AddNeighbour(int value) => _neighbours.Add(value);
    public void RemoveNeighbour(int value) => _neighbours.Remove(value);
    public void RemoveNeighbourAt(int index) => _neighbours.RemoveAt(index);
    public void ClearNeighbours() => _neighbours.Clear();
    #endregion

    #region Ecology
    public int GetCountEcologyParamiters() => _ecology.Count;
    public int GetCountEcologyDetails(int paramiter) => _ecology[paramiter].GetCountDetails();
    
    
    public S_Paramiter[] GetEcologyParamiters() => _ecology.ToArray();
    public int[] GetEcologyDetails(int paramiter) => _ecology[paramiter].GetDetails();


    public S_Paramiter GetEcologyParamiter(int index) => _ecology[index];
    public int GetEcologyDetail(int paramiter, int detail) => _ecology[paramiter].GetDetail(detail);

    
    public void SetEcologyParamiter(int paramiter, S_Paramiter value) => _ecology[paramiter] = value;
    public void SetEcologyDetail(int paramiter, int detail, int value) => _ecology[paramiter].SetDetail(detail, value);
    public void SetEcologyParamiters(S_Paramiter[] paramiters) {
        for (int i = 0; i < paramiters.Length; ++i) {
            paramiters.Add(paramiters[i]);
        }
    }
    public void SetEcologyDetails(int paramiter, int[] details) {
        for (int i = 0; i < details.Length; ++i) {
            _ecology[paramiter].SetDetail(i, details[i]);
        }
    }


    public void AddEcologyParamiter(S_Paramiter value) =>_ecology.Add(value);
    public void AddEcologyDetail(int paramiter, int value) => _ecology[paramiter].AddDetail(value);


    public void RemoveEcologyParamiter(S_Paramiter value) =>_ecology.Remove(value);
    public void RemoveEcologyDetail(int paramiter, int value) => _ecology[paramiter].RemoveDetail(value);


    public void RemoveEcologyParamiterAt(int value) => _ecology.RemoveAt(value);
    public void RemoveEcologyDetailAt(int paramiter, int index) => _ecology[paramiter].RemoveDetailAt(index);


    public void ClearEcologyParamiters() => _ecology.Clear();
    public void ClearEcologyDetails(int paramiter) => _ecology[paramiter].ClearDetails();

    public int GetEcologyRichness(int paramiter) => _ecology[paramiter].AllDetails;
    public int GetEcologyMaxDetail(int paramiter) => _ecology[paramiter].MaxDetail;
    public int GetEcologyMaxIndex(int paramiter) => _ecology[paramiter].MaxIndex;
    #endregion

    #region Civilizations
    public int GetCountCivilizations() => _civilizations.Count;
    public int GetCountCivilizationParamiters(int civilization) => _civilizations[civilization].GetCountParamiters();
    public int GetCountCivilizationDetails(int civilization, int paramiter) => _civilizations[civilization].GetCountDetails(paramiter);


    public S_Civilization[] GetArrayCivilizations() => _civilizations.ToArray();
    public S_Paramiter[] GetArrayCivilizationParamiters(int civilization) => _civilizations[civilization].GetParamiters();
    public int[] GetArrayCivilizationDetails(int civilization, int paramiter) => _civilizations[civilization].GetDetails(paramiter);
    public int[] GetArrayPopulations() {
        int[] arr = new int[_civilizations.Count];
        for (int i = 0; i < arr.Length; ++i) {
            arr[i] = _civilizations[i].GetPopulation();
        }
        return arr;
    }
    public int[] GetArrayCivilizationByParamiters(int paramiter) {
        if (_civilizations.Count < 0) return new int[0];
        int[] arr = new int[_civilizations[0].GetCountDetails(paramiter)];
        for (int i = 0; i < _civilizations.Count; ++i) {
            for (int detailIndex = 0; detailIndex < _civilizations[i].GetCountDetails(paramiter); ++detailIndex) {
                arr[detailIndex] += _civilizations[i].GetDetail(paramiter, detailIndex);
            }
        }
        return arr;
    }

    public int GetPopulation(int index) => _civilizations[index].GetPopulation();
    public int GetTakenFood(int index) => _civilizations[index].GetTakenFood();
    public float GetGovernmentObstacle(int index) => _civilizations[index].GetGovernmentObstacle();
    public S_Civilization GetCivilization(int index) => _civilizations[index];
    public S_Paramiter GetCivilizationParamiter(int civilization, int paramiter) => _civilizations[civilization].GetParamiter(paramiter);
    public int GetCivilizationDetail(int civilization, int paramiter, int detail) => _civilizations[civilization].GetDetail(paramiter, detail);


    public void SetPopulation(int index, int value) => _civilizations[index].SetPopulation(value);
    public void SetTakenFood(int index, int value) => _civilizations[index].SetTakenFood(value);
    public void SetGovernmentObstacle(int index, int value) => _civilizations[index].SetGovernmentObstacle(value);
    public void SetCivilization(int index, S_Civilization value) => _civilizations[index] = value;
    public void SetCivilizationParamiter(int civilization, int paramiter, S_Paramiter value) => _civilizations[civilization].SetParamiter(paramiter, value);
    public void SetCivilizationDetail(int civilization, int paramiter, int detail, int value) => _civilizations[civilization].SetDetail(paramiter, detail, value);
    public void SetCivilizations(S_Civilization[] civilizations) {
        for (int i = 0; i < civilizations.Length; ++i) {
            _civilizations[i] = civilizations[i];
        }
    }
    public void SetCivilizationsParamiters(int civilization, S_Paramiter[] paramiters) {
        for (int i = 0; i < paramiters.Length; ++i) {
            _civilizations[civilization].SetParamiter(i, paramiters[i]);
        }
    }
    public void SetCivilizationsDetails(int civilization, int paramiter, int[] details) {
        for (int i = 0; i < details.Length; ++i) {
            _civilizations[civilization].SetDetail(paramiter, i, details[i]);
        }
    }
    

    public void AddCivilization(S_Civilization value) => _civilizations.Add(value);
    public void AddCivilizationParamiter(int civilization, S_Paramiter value) => _civilizations[civilization].AddParamiter(value);
    public void AddCivilizationDetail(int civilization, int paramiter, int value) => _civilizations[civilization].AddDetail(paramiter, value);
    

    public void RemoveCivilization(S_Civilization value) => _civilizations.Remove(value);
    public void RemoveCivilizationParamiter(int civilization, S_Paramiter value) => _civilizations[civilization].RemoveParamiter(value);
    public void RemoveCivilizationDetail(int civilization, int paramiter, int value) => _civilizations[civilization].RemoveDetail(paramiter, value);
    

    public void RemoveCivilizationAt(int value) => _civilizations.RemoveAt(value);
    public void RemoveCivilizationParamiterAt(int civilization, int paramiter) => _civilizations[civilization].RemoveParamiterAt(paramiter);
    public void RemoveCivilizationDetailAt(int civilization, int paramiter, int detail) => _civilizations[civilization].RemoveDetailAt(paramiter, detail);
    

    public void ClearCivilizations() => _civilizations.Clear();
    public void ClearCivilizationParamiters(int civilization) => _civilizations[civilization].ClearParamiters();
    public void ClearCivilizationDetails(int civilization, int paramiter) => _civilizations[civilization].ClearDetails(paramiter);

    public int GetAllPopulations() {
        int all = 0;
        for (int i = 0; i < _civilizations.Count; ++i) {
            all += _civilizations[i].GetPopulation();
        }
        return all;
    }
    public int GetRichness(int civilization, int paramiter) => _civilizations[civilization].GetRichness(paramiter);
    public int GetAllValues(int paramiter, int detail) {
        int all = 0;
        for (int i = 0; i < _civilizations.Count; ++i) {
            all += _civilizations[i].GetParamiter(paramiter).GetDetail(detail);
        }
        return all;
    }

    public int GetMaxCivilizationDetail(int civilization, int paramiter) => _civilizations[civilization].GetMaxDetail(paramiter);
    public int GetMaxCivilizationDetail(int paramiter) {
        int max = -1;
        for (int i = 0; i < _civilizations.Count; ++i) {
            int value = _civilizations[i].GetMaxDetail(paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }

    public int GetCivilizationMaxIndex(int civilization, int paramiter) => _civilizations[civilization].GetMaxIndex(paramiter);
    public int GetCivilizationMaxIndex(int paramiter) {
        int max = -1;
        int index = -1;
        for(int i = 0; i < _civilizations.Count; ++i) {
            int value = _civilizations[i].GetMaxDetail(paramiter);
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }
    public int GetMaxPopulationsIndex() {
        int max = -1;
        int index = -1;
        for (int i = 0; i < _civilizations.Count; ++i) {
            int value = _civilizations[i].GetPopulation();
            if (max < value) {
                max = value;
                index = i;
            }
        }
        return index;
    }
    #endregion
    #endregion


    public int GetEat() {
        int farmers = GetAllValues(1, 0);
        int hunters = GetAllValues(1, 1);
        int flora = GetEcologyDetail(2, 0);
        int fauna = GetEcologyDetail(3, 0);
        return farmers.Proportion(hunters) > 50 ? flora : fauna;
    }
    public void SetEat(int value) {
        int farmers = GetAllValues(1, 0);
        int hunters = GetAllValues(1, 1);
        if (farmers.Proportion(hunters) > 50) SetEcologyDetail(2, 0, value);
        else SetEcologyDetail(3, 0, value);
    }

    public void AddEvent(int value) => events.Add(value);
    public void RemoveEvent(int index) => events.Remove(index);
    public void ClearEvent() => events.Clear();

    public void ClearAll() {
        _ecology.Clear();
        _civilizations.Clear();
        _neighbours.Clear();
    }
}

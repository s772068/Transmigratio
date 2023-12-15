// Ecology
// 0: Terrain
// 1: Climat
// 2: Flora
// 3: Fauna

using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class S_Region {
    [SerializeField] private string _name;
    [SerializeField] private Color _color;
    [SerializeField] private int _eventChanceIndex;
    [SerializeField] private List<S_Paramiter> _ecology = new();
    [SerializeField] private SerializedDictionary<int, S_Civilization> _civilizations = new();
    [SerializeField] private List<int> _neighbours = new();
    [SerializeField] private List<int> events = new();

    public string GetName() => _name;
    public Color GetColor() => _color;
    public int GetEventChanceIndex() => _eventChanceIndex;

    public void SetName(string value) => _name = value;
    public void SetColor(Color value) => _color = value;
    public void SetEventChanceIndex(int value) => _eventChanceIndex = value;

    #region Lists
    #region Neighbours
    public int GetCountNeighbours() => _neighbours.Count;
    public bool GetRandomNeighbour(out int neighbour) {
        neighbour = 0;
        if (_neighbours == null || _neighbours.Count == 0) return false;
        neighbour = _neighbours[Randomizer.Random(_neighbours.Count)];
        return true;
    }
    public int GetNeighbour(int index) => _neighbours[index];
    public void AddNeighbour(int value) => _neighbours.Add(value);
    public void RemoveNeighbour(int value) => _neighbours.Remove(value);
    public void RemoveNeighbourAt(int index) => _neighbours.RemoveAt(index);
    public void ClearNeighbours() => _neighbours.Clear();
    #endregion

    #region Ecology
    public int GetCountEcologyParamiters() => _ecology.Count;
    public int GetCountEcologyDetails(int ecoIndex) => _ecology[ecoIndex].GetCountDetails();
    
    
    public S_Paramiter[] GetEcologyParamiters() => _ecology.ToArray();
    public float[] GetEcologyDetails(int ecoIndex) => _ecology[ecoIndex].GetDetails();


    public S_Paramiter GetEcologyParamiter(int ecoIndex) => _ecology[ecoIndex];
    public float GetEcologyDetail(int ecoIndex, int detail) => _ecology[ecoIndex].GetDetail(detail);

    
    public void SetEcologyParamiter(int ecoIndex, S_Paramiter value) => _ecology[ecoIndex] = value;
    public void SetEcologyDetail(int ecoIndex, int detail, float value) => _ecology[ecoIndex].SetDetail(detail, value);
    public void SetEcologyParamiters(S_Paramiter[] paramiters) {
        for (int i = 0; i < paramiters.Length; ++i) {
            paramiters.Add(paramiters[i]);
        }
    }
    public void SetEcologyDetails(int ecoIndex, float[] details) {
        for (int i = 0; i < details.Length; ++i) {
            _ecology[ecoIndex].SetDetail(i, details[i]);
        }
    }


    public void AddEcologyParamiter(S_Paramiter value) =>_ecology.Add(value);
    public void AddEcologyDetail(int ecoIndex, float value) => _ecology[ecoIndex].AddDetail(value);


    public void RemoveEcologyParamiter(S_Paramiter value) =>_ecology.Remove(value);
    public void RemoveEcologyDetail(int ecoIndex, float value) => _ecology[ecoIndex].RemoveDetail(value);


    public void RemoveEcologyParamiterAt(int value) => _ecology.RemoveAt(value);
    public void RemoveEcologyDetailAt(int ecoIndex, int index) => _ecology[ecoIndex].RemoveDetailAt(index);


    public void ClearEcologyParamiters() => _ecology.Clear();
    public void ClearEcologyDetails(int ecoIndex) => _ecology[ecoIndex].ClearDetails();

    public float GetEcologyRichness(int ecoIndex) => _ecology[ecoIndex].AllDetails;
    public float GetEcologyMaxDetail(int ecoIndex) => _ecology[ecoIndex].MaxDetail;
    public float GetEcologyMaxIndex(int ecoIndex) => _ecology[ecoIndex].MaxIndex;
    #endregion

    #region Civilizations
    public int GetCountCivilizations() => _civilizations.Count;
    public int GetCountCivilizationParamiters(int civID) => _civilizations[civID].GetCountParamiters();
    public int GetCountCivilizationDetails(int civID, int paramiter) => _civilizations[civID].GetCountDetails(paramiter);


    public int[] GetArrayCivilizationsKey() => _civilizations.Keys.ToArray();
    public S_Civilization[] GetArrayCivilizations() => _civilizations.Values.ToArray();
    public S_Paramiter[] GetArrayCivilizationParamiters(int civID) => _civilizations[civID].GetParamiters();
    public float[] GetArrayCivilizationDetails(int civID, int paramiter) => _civilizations[civID].GetDetails(paramiter);
    public float[] GetArrayPopulations() {
        float[] arr = new float[_civilizations.Count];
        for (int i = 0; i < arr.Length; ++i) {
            arr[i] = GetPopulation(i);
        }
        return arr;
    }
    public float[] GetArrayCivilizationByParamiters(int paramiter) {
        if (_civilizations.Count < 0) return new float[0];
        float[] arr = new float[_civilizations[0].GetCountDetails(paramiter)];
        for (int i = 0; i < _civilizations.Count; ++i) {
            for (int detailIndex = 0; detailIndex < _civilizations[i].GetCountDetails(paramiter); ++detailIndex) {
                arr[detailIndex] += _civilizations[i].GetDetail(paramiter, detailIndex);
            }
        }
        return arr;
    }


    public bool HasCivilization(int civID) => _civilizations.ContainsKey(civID);
    public bool HasCivilization(S_Civilization civilization) => _civilizations.ContainsValue(civilization);


    public int GetCivilizationID(int civID) => _civilizations[civID].GetID();
    public int GetPopulation(int civID) => _civilizations[civID].GetPopulation();
    public float GetTakenFood(int civID) => _civilizations[civID].GetTakenFood();
    public float GetGovernmentObstacle(int civID) => _civilizations[civID].GetGovernmentObstacle();
    public S_Civilization GetCivilization(int civID) => _civilizations[civID];
    public S_Paramiter GetCivilizationParamiter(int civID, int paramiter) => _civilizations[civID].GetParamiter(paramiter);
    public float GetCivilizationDetail(int civID, int paramiter, int detail) => _civilizations[civID].GetDetail(paramiter, detail);

    
    public void CopyCivilization(int civID, S_Civilization civilization) {
        if (!_civilizations.ContainsKey(civID)) _civilizations.Add(civID, new());
        _civilizations[civID].Copy(civilization);
    }


    public void SetCivilizationID(int civID, int value) => _civilizations[civID].SetID(value);
    public void SetPopulation(int civID, int value) => _civilizations[civID].SetPopulation(value);
    public void SetTakenFood(int civID, int value) => _civilizations[civID].SetTakenFood(value);
    public void SetGovernmentObstacle(int civID, int value) => _civilizations[civID].SetGovernmentObstacle(value);
    public void SetCivilization(int civID, S_Civilization value) => _civilizations[civID] = value;
    public void SetCivilizationParamiter(int civID, int paramiter, S_Paramiter value) => _civilizations[civID].SetParamiter(paramiter, value);
    public void SetCivilizationDetail(int civID, int paramiter, int detail, int value) => _civilizations[civID].SetDetail(paramiter, detail, value);
    public void SetCivilizations(S_Civilization[] civilizations) {
        for (int i = 0; i < civilizations.Length; ++i) {
            _civilizations[i] = civilizations[i];
        }
    }
    public void SetCivilizationsParamiters(int civID, S_Paramiter[] paramiters) {
        for (int i = 0; i < paramiters.Length; ++i) {
            _civilizations[civID].SetParamiter(i, paramiters[i]);
        }
    }
    public void SetCivilizationsDetails(int civID, int paramiter, int[] details) {
        for (int i = 0; i < details.Length; ++i) {
            _civilizations[civID].SetDetail(paramiter, i, details[i]);
        }
    }


    public void AddCivilization(int civID, S_Civilization value) {
        if (!_civilizations.ContainsKey(civID))
            _civilizations.Add(civID, value);
        else
            _civilizations[civID] = value;
    }
    public void AddCivilizationParamiter(int civID, S_Paramiter value) {
        if (!_civilizations.ContainsKey(civID))
            _civilizations.Add(civID, new());
        _civilizations[civID].AddParamiter(value);
    }
    public void AddCivilizationDetail(int civID, int paramiter, int value) {
        if (!_civilizations.ContainsKey(civID))
            _civilizations.Add(civID, new());
        _civilizations[civID].AddDetail(paramiter, value);
    }
    

    public void RemoveCivilization(int civID) => _civilizations.Remove(civID);
    public void RemoveCivilization(S_Civilization civilization) => _civilizations.Remove(civilization.GetID());
    public void RemoveCivilizationParamiter(int civID, S_Paramiter value) => _civilizations[civID].RemoveParamiter(value);
    public void RemoveCivilizationDetail(int civID, int paramiter, int value) => _civilizations[civID].RemoveDetail(paramiter, value);
    

    public void RemoveCivilizationParamiterAt(int civID, int paramiter) => _civilizations[civID].RemoveParamiterAt(paramiter);
    public void RemoveCivilizationDetailAt(int civID, int paramiter, int detail) => _civilizations[civID].RemoveDetailAt(paramiter, detail);
    

    public void ClearCivilizations() => _civilizations.Clear();
    public void ClearCivilizationParamiters(int civID) => _civilizations[civID].ClearParamiters();
    public void ClearCivilizationDetails(int civID, int paramiter) => _civilizations[civID].ClearDetails(paramiter);

    public int GetAllPopulations() {
        int all = 0;
        foreach (var civilization in _civilizations.Values) {
            all += civilization.GetPopulation();
        }
        return all;
    }
    public float GetRichness(int civID, int paramiter) => _civilizations[civID].GetRichness(paramiter);
    public float GetAllValues(int paramiter, int detail) {
        float all = 0;
        foreach (var civilization in _civilizations.Values) {
            all += civilization.GetParamiter(paramiter).GetDetail(detail);
        }
        return all;
    }

    public float GetMaxCivilizationDetail(int civID, int paramiter) => _civilizations[civID].GetMaxDetail(paramiter);
    public float GetMaxCivilizationDetail(int paramiter) {
        float max = -1;
        foreach (var civilization in _civilizations.Values) {
            float value = civilization.GetMaxDetail(paramiter);
            if (max < value) {
                max = value;
            }
        }
        return max;
    }

    public int GetCivilizationMaxIndex(int civID, int paramiter) => _civilizations[civID].GetMaxIndex(paramiter);
    public int GetCivilizationMaxIndex(int paramiter) {
        float max = -1;
        int index = -1;
        foreach(var civilization in _civilizations) {
            float value = civilization.Value.GetMaxDetail(paramiter);
            if (max < value) {
                max = value;
                index = civilization.Key;
            }
        }
        return index;
    }
    public int GetMaxPopulationsIndex() {
        int max = -1;
        int index = -1;
        foreach (var civilization in _civilizations) {
            int value = civilization.Value.GetPopulation();
            if (max < value) {
                max = value;
                index = civilization.Key;
            }
        }
        return index;
    }
    #endregion
    #endregion


    public float GetEat() {
        float farmers = GetAllValues(1, 0);
        float hunters = GetAllValues(1, 1);
        float flora = GetEcologyDetail(2, 0);
        float fauna = GetEcologyDetail(3, 0);
        return farmers.Proportion(hunters) > 50 ? flora : fauna;
    }
    public void SetEat(float value) {
        float farmers = GetAllValues(1, 0);
        float hunters = GetAllValues(1, 1);
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

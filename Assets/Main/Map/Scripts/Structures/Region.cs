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
public class Region {
    [SerializeField] private string _name;
    [SerializeField] private Color _color;
    [SerializeField] private float _takenFood;
    [SerializeField] private Paramiter terrain;
    [SerializeField] private Paramiter climate;
    [SerializeField] private Paramiter flora;
    [SerializeField] private Paramiter fauna;
    [SerializeField] private SerializedDictionary<float, float> _civilizations = new();
    [SerializeField] private List<int> _neighbours = new();

    public string Name {
        get => _name;
        set => _name = value;
    }
    public Color Color {
        get => _color;
        set => _color = value;
    }

    public Paramiter Terrain {
        get => terrain;
        set => terrain = value;
    }
    public Paramiter Climate {
        get => climate;
        set => climate = value;
    }
    public Paramiter Flora {
        get => flora;
        set => flora = value;
    }
    public Paramiter Fauna {
        get => fauna;
        set => fauna = value;
    }

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
    //public int GetCountEcologyParamiters() => _ecology.Count;
    //public int GetCountEcologyDetails(int ecoIndex) => _ecology[ecoIndex].CountDetails;
    
    
    //public Paramiter[] GetEcologyParamiters() => _ecology.ToArray();
    //public float[] GetEcologyDetails(int ecoIndex) => _ecology[ecoIndex].GetDetails();


    //public Paramiter GetEcologyParamiter(int ecoIndex) => _ecology[ecoIndex];
    //public float GetEcologyDetail(int ecoIndex, int detail) => _ecology[ecoIndex].GetValue(detail);

    
    //public void SetEcologyParamiter(int ecoIndex, Paramiter value) => _ecology[ecoIndex] = value;
    //public void SetEcologyDetail(int ecoIndex, int detail, float value) => _ecology[ecoIndex].SetDetail(detail, value);
    public void SetEcologyParamiters(Paramiter[] paramiters) {
        for (int i = 0; i < paramiters.Length; ++i) {
            paramiters.Add(paramiters[i]);
        }
    }
    //public void SetEcologyDetails(int ecoIndex, float[] details) {
    //    for (int i = 0; i < details.Length; ++i) {
    //        _ecology[ecoIndex].SetDetail(i, details[i]);
    //    }
    //}


    //public void AddEcologyParamiter(Paramiter value) =>_ecology.Add(value);
    //public void AddEcologyDetail(int ecoIndex, float value) => _ecology[ecoIndex].AddDetail(value);


    //public void RemoveEcologyParamiter(Paramiter value) =>_ecology.Remove(value);
    //// public void RemoveEcologyDetail(int ecoIndex, float value) => _ecology[ecoIndex].RemoveDetail(value);


    //public void RemoveEcologyParamiterAt(int value) => _ecology.RemoveAt(value);
    //public void RemoveEcologyDetailAt(int ecoIndex, int index) => _ecology[ecoIndex].RemoveDetailAt(index);


    //public void ClearEcologyParamiters() => _ecology.Clear();
    //public void ClearEcologyDetails(int ecoIndex) => _ecology[ecoIndex].ClearDetails();

    //public float GetEcologyRichness(int ecoIndex) => _ecology[ecoIndex].AllDetails;
    //public float GetEcologyMaxDetail(int ecoIndex) => _ecology[ecoIndex].MaxDetail;
    //public float GetEcologyMaxIndex(int ecoIndex) => _ecology[ecoIndex].MaxIndex;
    #endregion

    #region Civilizations
    public int GetCountCivilizations() => _civilizations.Count;
    public float GetCivilizationID(int index) => _civilizations.Keys.ToArray()[index];
    public float[] GetArrayPopulations() => _civilizations.Values.ToArray();


    public bool HasCivilization(float civID) => _civilizations.ContainsKey(civID);


    public bool GetRandomCivilizationID(out float civID) {
        civID = 0;
        if(_civilizations != null && _civilizations.Count > 0) {
            int random = Randomizer.Random(_civilizations.Count);
            civID = _civilizations.Keys.ToArray()[random];
            return true;
        }
        return false;
    }
    public float GetPopulation(float civID) => _civilizations[civID];
    public float GetTakenFood() => _takenFood;

    
    public void CopyCivilization(float civID, int population) {
        if (!_civilizations.ContainsKey(civID)) _civilizations.Add(civID, new());
        _civilizations[civID] = population;
    }



    public void SetCivilization(float oldCivID, float  newCivID) {
        _civilizations.Add(newCivID, _civilizations[oldCivID]);
        _civilizations.Remove(oldCivID);
    }
    public void SetPopulation(float civID, float value) {
        if (_civilizations == null) _civilizations = new();
        if (!_civilizations.ContainsKey(civID))
            _civilizations.Add(civID, value);
        else
            _civilizations[civID] = value;
    }
    public void SetTakenFood(float value) => _takenFood = value;
    
    public float GetAllPopulations() {
        float all = 0;
        foreach (float civilization in _civilizations.Values) {
            all += civilization;
        }
        return all;
    }

    public float GetMaxPopulationsIndex() {
        float max = -1;
        float index = -1;
        foreach (var civilization in _civilizations) {
            float value = civilization.Value;
            if (max < value) {
                max = value;
                index = civilization.Key;
            }
        }
        return index;
    }
    #endregion
    #endregion


    public void ClearAll() {
        //_ecology.Clear();
        _civilizations.Clear();
        _neighbours.Clear();
    }
}

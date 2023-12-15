// Paramiters
// 0: ProdMode
// 1: Economics
// 2: Governments

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_Civilization {
    [SerializeField] private int _id;
    [SerializeField, Min(0)] private int _population;
    [SerializeField] private float _takenFood;
    [SerializeField] private float _governmentObstacle;
    [SerializeField] private List<S_Paramiter> _paramiters = new();


    public int GetCountParamiters() => _paramiters.Count;
    public int GetCountDetails(int paramiter) => _paramiters[paramiter].GetCountDetails();


    public S_Paramiter[] GetParamiters() => _paramiters.ToArray();
    public float[] GetDetails(int paramiter) => _paramiters[paramiter].GetDetails();

    public int GetID() => _id;
    public int GetPopulation() => _population;
    public float GetTakenFood() => _takenFood;
    public float GetGovernmentObstacle() => _governmentObstacle;
    public S_Paramiter GetParamiter(int index) => _paramiters[index];
    public float GetDetail(int paramiter, int detail) => _paramiters[paramiter].GetDetail(detail);

    public void Copy(S_Civilization civilization) {
        _id = civilization.GetID();
        _population = civilization.GetPopulation();
        _takenFood = civilization.GetTakenFood();
        _governmentObstacle = civilization.GetGovernmentObstacle();
        
        if (_paramiters == null) _paramiters = new();
        for (int i = 0; i < civilization.GetCountParamiters(); ++i) {
            if (i == _paramiters.Count) _paramiters.Add(new());
            _paramiters[i].CopyTo(civilization.GetParamiter(i));
        }
    }

    public void SetID(int value) => _id = value;
    public void SetPopulation(int value) => _population = value;
    public void SetTakenFood(float value) => _takenFood = value;
    public void SetGovernmentObstacle(float value) => _governmentObstacle = value;
    public void SetParamiter(int index, S_Paramiter value) => _paramiters[index] = value;
    public void SetDetail(int paramiter, int detail, float value) => _paramiters[paramiter].SetDetail(detail, value);
    public void SetParamiters(S_Paramiter[] paramiters) {
        for(int i = 0; i < paramiters.Length; ++i) {
            _paramiters[i] = paramiters[i];
        }
    }
    public void SetDetails(int paramiter, int[] details) {
        for(int i = 0; i < details.Length; ++i) {
            _paramiters[paramiter].SetDetail(i, details[i]);
        }
    }


    public void AddParamiter(S_Paramiter value) {
        if (_paramiters == null) _paramiters = new();
        _paramiters.Add(value);
    }
    public void AddDetail(int paramiter, float value) {
        if (_paramiters == null) _paramiters = new();
        _paramiters[paramiter].AddDetail(value);
    }
    public void RemoveParamiter(S_Paramiter value) => _paramiters.Remove(value);
    public void RemoveDetail(int paramiter, float value) => _paramiters[paramiter].RemoveDetail(value);

    public void RemoveParamiterAt(int detail) => _paramiters.RemoveAt(detail);
    public void RemoveDetailAt(int paramiter, int detail) => _paramiters[paramiter].RemoveDetailAt(detail);

    public void ClearParamiters() => _paramiters.Clear();
    public void ClearDetails(int paramiter) => _paramiters[paramiter].ClearDetails();


    public float GetRichness(int paramiter) => _paramiters[paramiter].AllDetails;
    public float GetMaxDetail(int paramiter) => _paramiters[paramiter].MaxDetail;
    public int GetMaxIndex(int paramiter) => _paramiters[paramiter].MaxIndex;

    //public static S_Civilization operator + (S_Civilization val1, S_Civilization val2) {
    //    S_Civilization res = new();
    //    res._population = val1._population + val2._population;
    //    res._takenFood = val1._takenFood + val2._takenFood;
    //    res._governmentObstacle = val1._governmentObstacle + val2._governmentObstacle;

    //    res._paramiters = new() {

    //    }
    //    return new();
    //}
}

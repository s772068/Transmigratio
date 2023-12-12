// Paramiters
// 0: ProdMode
// 1: Economics
// 2: Governments

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_Civilization {
    [SerializeField] private int _population;
    [SerializeField] private int _takenFood;
    [SerializeField] private float _governmentObstacle;
    [SerializeField] private List<S_Paramiter> _paramiters;


    public int GetCountParamiters() => _paramiters.Count;
    public int GetCountDetails(int paramiter) => _paramiters[paramiter].GetCountDetails();


    public S_Paramiter[] GetParamiters() => _paramiters.ToArray();
    public int[] GetDetails(int paramiter) => _paramiters[paramiter].GetDetails();


    public int GetPopulation() => _population;
    public int GetTakenFood() => _takenFood;
    public float GetGovernmentObstacle() => _governmentObstacle;
    public S_Paramiter GetParamiter(int index) => _paramiters[index];
    public int GetDetail(int paramiter, int detail) => _paramiters[paramiter].GetDetail(detail);


    public void SetPopulation(int value) => _population = value;
    public void SetTakenFood(int value) => _takenFood = value;
    public void SetGovernmentObstacle(float value) => _governmentObstacle = value;
    public void SetParamiter(int index, S_Paramiter value) => _paramiters[index] = value;
    public void SetDetail(int paramiter, int detail, int value) => _paramiters[paramiter].SetDetail(detail, value);
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
        if (_paramiters == null)
            _paramiters = new();
        _paramiters.Add(value);
    }
    public void AddDetail(int paramiter, int value) {
        if (_paramiters == null)
            _paramiters = new();
        _paramiters[paramiter].AddDetail(value);
    }
    public void RemoveParamiter(S_Paramiter value) => _paramiters.Remove(value);
    public void RemoveDetail(int paramiter, int value) => _paramiters[paramiter].RemoveDetail(value);

    public void RemoveParamiterAt(int value) => _paramiters.RemoveAt(value);
    public void RemoveDetailAt(int paramiter, int detail) => _paramiters[paramiter].RemoveDetailAt(detail);

    public void ClearParamiters() => _paramiters.Clear();
    public void ClearDetails(int paramiter) => _paramiters[paramiter].ClearDetails();


    public int GetRichness(int paramiter) => _paramiters[paramiter].AllDetails;
    public int GetMaxDetail(int paramiter) => _paramiters[paramiter].MaxDetail;
    public int GetMaxIndex(int paramiter) => _paramiters[paramiter].MaxIndex;
}

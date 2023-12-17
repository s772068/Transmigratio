using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class S_Civ {
    [SerializeField] private float _reserveFood;
    [SerializeField] private float _governmentObstacle;
    [SerializeField] private List<int> _regions;
    [SerializeField] private List<S_Paramiter> _paramiters;

    public float GetReserveFood() => _reserveFood;
    public void SetReserveFood(int value) => _reserveFood = value;
    

    public float GetGovernmentObstacle() => _governmentObstacle;
    public void SetGovernmentObstacle(int value) => _governmentObstacle = value;


    public void AddRegion(int region) {
        if (_regions == null) _regions = new();
        if(_regions.Contains(region)) return;
        _regions.Add(region);
    }

    public bool HasRegion(int region) {
        if (_regions == null) return false;
        return _regions.Contains(region);
    }
    public void RemoveRegion(int region) {
        if (_regions == null) return;
        _regions.Remove(region);
    }

    public bool GetRandomRegion(out int region) {
        region = 0;
        if(_regions == null || _regions.Count == 0) return false;
        region = _regions[Randomizer.Random(_regions.Count)];
        return true;
    }







    public int GetCountParamiters() => _paramiters.Count;
    public int GetCountDetails(int paramiter) => _paramiters[paramiter].GetCountDetails();


    public float[] GetDetails(int paramiter) => _paramiters[paramiter].GetDetails();
    public S_Paramiter[] GetParamiters() => _paramiters.ToArray();

    public S_Paramiter GetParamiter(int index) => _paramiters[index];
    public float GetDetail(int paramiter, int detail) => _paramiters[paramiter].GetValue(detail);

    //public void Copy(S_Civilization civilization) {
    //    if (_paramiters == null) _paramiters = new();
    //    for (int i = 0; i < civilization.GetCountParamiters(); ++i) {
    //        if (i == _paramiters.Count) _paramiters.Add(new());
    //        _paramiters[i].CopyTo(civilization.GetParamiter(i));
    //    }
    //}

    public void SetParamiter(int index, S_Paramiter value) => _paramiters[index] = value;
    public void SetDetail(int paramiter, int detail, float value) => _paramiters[paramiter].SetDetail(detail, value);


    public void AddParamiter(S_Paramiter value) {
        if (_paramiters == null) _paramiters = new();
        _paramiters.Add(value);
    }

    public float GetMaxDetail(int paramiter) => _paramiters[paramiter].MaxDetail;
    public int GetMaxIndex(int paramiter) => _paramiters[paramiter].MaxIndex;
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Civilization {
    [SerializeField] private float _reserveFood;
    [SerializeField] private float _governmentObstacle;
    [SerializeField] private List<int> _regions;
    [SerializeField] private Paramiter prodMode;
    [SerializeField] private Paramiter ecoCulture;
    [SerializeField] private Paramiter goverment;
    [SerializeField] private Paramiter politic;

    public float ReserveFood {
        get => _reserveFood;
        set => _reserveFood = value;
    }

    public float GovernmentObstacle {
        get => _governmentObstacle;
        set => _governmentObstacle = value;
    }

    public int CountRegions => _regions.Count;
    public void AddRegion(int region) {
        if (_regions == null) _regions = new();
        if(_regions.Contains(region)) return;
        _regions.Add(region);
    }

    public bool HasRegion(int region) {
        if (_regions == null) return false;
        return _regions.Contains(region);
    }
    public int GetRegion(int index) => _regions[index];
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



    //public int GetCountParamiters() => _paramiters.Count;
    private int GetCountRegions() => _regions.Count;
    //public int GetCountDetails(int paramiter) => _paramiters[paramiter].CountDetails;


    //public float[] GetDetails(int paramiter) => _paramiters[paramiter].GetDetails();
    //public Paramiter[] GetParamiters() => _paramiters.ToArray();

    //public Paramiter GetParamiter(int index) => _paramiters[index];
    //public float GetDetail(int paramiter, int detail) => _paramiters[paramiter].GetValue(detail);

    public void Copy(Civilization civilization) {
        _reserveFood = civilization._reserveFood;
        _governmentObstacle = civilization.GovernmentObstacle;
        if (_regions == null) _regions = new();
        for (int i = 0; i < civilization.GetCountRegions(); ++i) {
            if (i == _regions.Count) _regions.Add(new());
            _regions[i] = civilization.GetRegion(i);
        }
        //CopyParamiters(civilization);
    }

    //public void CopyParamiters(Civilization civilization) {
    //    if (_paramiters == null) _paramiters = new();
    //    for (int i = 0; i < civilization.GetCountParamiters(); ++i) {
    //        if (i == _paramiters.Count) _paramiters.Add(new());
    //        _paramiters[i].CopyTo(civilization.GetParamiter(i));
    //    }
    //}

    //public void SetParamiter(int index, Paramiter value) => _paramiters[index] = value;
    //public void SetDetail(int paramiter, int detail, float value) => _paramiters[paramiter].SetDetail(detail, value);


    //public void AddParamiter(Paramiter value) {
    //    if (_paramiters == null) _paramiters = new();
    //    _paramiters.Add(value);
    //}

    //public float GetAllDetails(int paramiter) => _paramiters[paramiter].AllDetails;
    //public float GetMaxDetail(int paramiter) => _paramiters[paramiter].MaxDetail;
    //public int GetMaxIndex(int paramiter) => _paramiters[paramiter].MaxIndex;
    //public float GetAllByDetail(int detail) {
    //    float all = 0;
    //    for(int i = 0; i < _paramiters.Count; ++i) {
    //        all += _paramiters[i].GetDetail(detail).Value;
    //    }
    //    return all;
    //}
}

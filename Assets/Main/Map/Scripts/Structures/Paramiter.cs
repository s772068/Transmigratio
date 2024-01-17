using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paramiter {
    [SerializeField] private string name;
    [SerializeField] private Sprite pictogram;
    [SerializeField] private string metric;
    [SerializeField] private List<Detail> details = new();

    public string Name => name;
    public Sprite Pictogram => pictogram;
    public int CountDetails => details.Count;
    public Detail GetDetail(int index) => details[index];

    public float[] GetDetails() {
        float[] res = new float[details.Count];
        for(int i = 0; i < details.Count; ++i) {
            res[i] = details[i].Value;
        }
        return res;
    }
    // public Detail GetDetail(int index) => details[index];
    public float GetValue(int index) => details[index].Value;
    public void SetDetail(int index, float value) => details[index].Value = value;
    
    public void CopyTo(Paramiter paramiter) {
        if(details == null) details = new();
        for(int i = 0; i < paramiter.CountDetails; ++i) {
            if(i == details.Count)
                AddDetail(paramiter.GetDetails()[i]);
            else
                details[i] = paramiter.GetDetail(i);
        }
    }

    public void AddDetail(float detail) {
        if (details == null) details = new();
        Detail newDetail = new();
        newDetail.Value = detail;
        details.Add(newDetail);
    }

    public void RemoveDetailAt(int index) {
        details.RemoveAt(index);
        if (details.Count == 0) details = null;
    }
    public void ClearDetails() {
        details.Clear();
        details = null;
    }

    public float AllDetails {
        get {
            float all = 0;
            for(int i = 0; i < details.Count; ++i) {
                all += details[i].Value;
            }
            return all;
        }
    }

    public Detail MaxDetail {
        get {
            if(details.Count == 0) return null;
            int index = -1;
            float max = -1;
            for (int i = 0; i < details.Count; ++i) {
                float value = details[i].Value;
                if (max < value) {
                    max = value;
                    index = i;
                }
            }
            return details[index];
        }
    }

    public float MaxDetailValue {
        get {
            float max = -1;
            for (int i = 0; i < details.Count; ++i) {
                float value = details[i].Value;
                if (max < value) {
                    max = value;
                }
            }
            return max;
        }
    }

    public int MaxIndex {
        get {
            int index = -1;
            float max = -1;
            for (int i = 0; i < details.Count; ++i) {
                float value = details[i].Value;
                if (max < value) {
                    max = value;
                    index = i;
                }
            }
            return index;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_Paramiter {
    [SerializeField] private List<S_Detail> details = new();

    public int GetCountDetails() => details.Count;
    public float[] GetDetails() {
        float[] res = new float[details.Count];
        for(int i = 0; i < details.Count; ++i) {
            res[i] = details[i].GetValue();
        }
        return res;
    }
    public S_Detail GetDetail(int index) => details[index];
    public float GetValue(int index) => details[index].GetValue();
    public void SetDetail(int index, float value) => details[index].SetValue(value);
    
    public void CopyTo(S_Paramiter paramiter) {
        if(details == null) details = new();
        for(int i = 0; i < paramiter.GetCountDetails(); ++i) {
            if(i == details.Count)
                AddDetail(paramiter.GetDetails()[i]);
            else
                details[i] = paramiter.GetDetail(i);
        }
    }

    public void AddDetail(float detail) {
        if (details == null) details = new();
        S_Detail newDetail = new();
        newDetail.SetValue(detail);
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
                all += details[i].GetValue();
            }
            return all;
        }
    }

    public float MaxDetail {
        get {
            float max = -1;
            for (int i = 0; i < details.Count; ++i) {
                float value = details[i].GetValue();
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
                float value = details[i].GetValue();
                if (max < value) {
                    max = value;
                    index = i;
                }
            }
            return index;
        }
    }
}

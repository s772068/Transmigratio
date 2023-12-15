using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_Paramiter {
    [SerializeField] private List<float> details = new();

    public int GetCountDetails() => details.Count;
    public float[] GetDetails() => details.ToArray();
    public float GetDetail(int index) => details[index];
    public float SetDetail(int index, float value) => details[index] = value;
    
    public void CopyTo(S_Paramiter paramiter) {
        if(details == null) details = new();
        for(int i = 0; i < paramiter.GetCountDetails(); ++i) {
            if(i == details.Count)
                details.Add(paramiter.GetDetail(i));
            else
                details[i] = paramiter.GetDetail(i);
        }
    }

    public void AddDetail(float detail) {
        if (details == null) details = new();
        details.Add(detail);
    }
    public void RemoveDetail(float detail) {
        details.Remove(detail);
        if (details.Count == 0) details = null;
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
                all += details[i];
            }
            return all;
        }
    }

    public float MaxDetail {
        get {
            float max = -1;
            for (int i = 0; i < details.Count; ++i) {
                float value = details[i];
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
                float value = details[i];
                if (max < value) {
                    max = value;
                    index = i;
                }
            }
            return index;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_Paramiter {
    [SerializeField] private List<int> details;

    public int GetCountDetails() => details.Count;
    public int[] GetDetails() => details.ToArray();
    public int GetDetail(int index) => details[index];
    public int SetDetail(int index, int value) => details[index] = value;
    public void AddDetail(int detail) {
        if (details == null) details = new();
        details.Add(detail);
    }
    public void RemoveDetail(int detail) {
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

    public int AllDetails {
        get {
            int all = 0;
            for(int i = 0; i < details.Count; ++i) {
                all += details[i];
            }
            return all;
        }
    }

    public int MaxDetail {
        get {
            int max = -1;
            for (int i = 0; i < details.Count; ++i) {
                int value = details[i];
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
            int max = -1;
            for (int i = 0; i < details.Count; ++i) {
                int value = details[i];
                if (max < value) {
                    max = value;
                    index = i;
                }
            }
            return index;
        }
    }
}

using UnityEngine;

[System.Serializable]
public struct SL_Paramiter<TValue> {
    public string Name;
    public string Description;
    public TValue Value;
}

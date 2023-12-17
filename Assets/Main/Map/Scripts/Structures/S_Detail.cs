using UnityEngine;

[System.Serializable]
public struct S_Detail {
    [SerializeField, Range(0, 100)] private float _value;
    public float GetValue() => _value;
    public float SetValue(float value) => _value = value;
}

using UnityEngine;

[System.Serializable]
public class S_Detail {
    [SerializeField, Range(0, 100)] private float _value;
    public float GetValue() => _value;
    public void SetValue(float value) { _value = value; Debug.Log(_value); }
}

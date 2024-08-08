using System;
using UnityEngine;

[Serializable]
public class ParamiterValue {
    [SerializeField] private float _startValue;
    [SerializeField] private float _value;
    
    public Action<float, float> onUpdate;

    public float StartValue => _startValue;
    public float Value {
        get => _value;
        set {
            onUpdate?.Invoke(_value, value);
            _value = value;
        }
    }

    public ParamiterValue() { }

    public ParamiterValue(float val) {
        _startValue = val;
        _value = val;
    }
}

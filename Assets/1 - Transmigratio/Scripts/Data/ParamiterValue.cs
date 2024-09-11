using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ParamiterValue {
    [SerializeField] private float _startValue;
    public float value;

    // prev, cur, piece
    public Action<float, float, CivPiece> onUpdate;

    public float StartValue => _startValue;
    
    public ParamiterValue() { }

    public ParamiterValue(float val) {
        _startValue = val;
        value = val;
    }
}

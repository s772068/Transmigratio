using System;
using UnityEngine;

/// <summary>
/// Класс для расчётов населения. 
/// Население области, цивилизации или всего мира - переменные типа Population
/// </summary>
[Serializable]
public class Population {
    [SerializeField] private int _value;

    // prev, cur, piece
    public Action<int, int, CivPiece> onUpdate;

    public int Value {
        get => _value;
        set { if (value >= 0) _value = value; }
    }

    public Population(int val) {
        Value = val;
    }
}

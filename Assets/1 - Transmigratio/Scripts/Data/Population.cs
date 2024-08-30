using System;
using UnityEngine;

/// <summary>
/// ����� ��� �������� ���������. 
/// ��������� �������, ����������� ��� ����� ���� - ���������� ���� Population
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

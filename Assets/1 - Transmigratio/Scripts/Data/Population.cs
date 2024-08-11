using System;
using UnityEngine;

/// <summary>
/// ����� ��� �������� ���������. 
/// ��������� �������, ����������� ��� ����� ���� - ���������� ���� Population
/// </summary>
[Serializable]
public class Population {
    [SerializeField] private int _value;

    public Action<int, int> onUpdate;

    public int Value {
        get => _value;
        set {
            if (value >= 0) {
                onUpdate?.Invoke(_value, value);
                _value = value;
            }
        }
    }

    public Population(int val) {
        Value = val;
    }
}

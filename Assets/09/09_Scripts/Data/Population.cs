using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� ��� �������� ���������. 
/// ��������� �������, ����������� ��� ����� ���� - ���������� ���� Population
/// </summary>
[System.Serializable]
public class Population
{
    public int value;
    public int Value
    {
        get { return value; }
        set { if (value < 0) value = 0; }
    }
}

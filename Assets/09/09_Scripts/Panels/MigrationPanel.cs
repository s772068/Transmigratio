using System;
using UnityEngine;

public class MigrationPanel : MonoBehaviour {
    public Action onBreak;
    public Action onSpeedUp;

    public float Value { set {

        }
    }

    // � ������
    // ��� �������� ��������������� �����
    // ��� �������� ��������������  �����

    public void Break() => onBreak();
    public void SpeedUp() => onSpeedUp?.Invoke();
}

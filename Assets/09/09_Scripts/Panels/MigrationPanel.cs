using System;
using UnityEngine;

public class MigrationPanel : MonoBehaviour {
    public Action onBreak;
    public Action onSpeedUp;

    public float Value { set {

        }
    }

    // В панель
    // При открытии останавливается время
    // При закрытии возобновляется  время

    public void Break() => onBreak();
    public void SpeedUp() => onSpeedUp?.Invoke();
}

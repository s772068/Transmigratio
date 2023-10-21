using UnityEngine;

public class GameSettings : MonoSingleton<GameSettings> {
    [SerializeField] private int stepInterval;
    public int StepInterval => stepInterval;
}

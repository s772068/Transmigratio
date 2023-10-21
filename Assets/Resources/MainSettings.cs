using UnityEngine;

public class MainSettings : MonoSingleton<MainSettings> {
    [SerializeField] private string str;
    public string Str => str;
}
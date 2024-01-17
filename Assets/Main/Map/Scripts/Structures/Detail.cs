using UnityEngine;

[System.Serializable]
public class Detail {
    [SerializeField] private string name;
    [SerializeField] private Color color;
    [SerializeField, Range(0, 100)] private float _value;
    
    public string Name => name;
    public Color Color => color;
    public float Value {
        get => _value;
        set => _value = value;
    }
}

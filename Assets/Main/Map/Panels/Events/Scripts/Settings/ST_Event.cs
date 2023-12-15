using UnityEngine;

[System.Serializable]
public struct ST_Event {
    public string name;
    [Min(0)] public int IconSprite;
    [Min(0)] public int MarkerSprite;
}

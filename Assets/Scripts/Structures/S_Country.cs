using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_Country {
    public string Name;
    public int Population;
    public int EventChanceIndex;
    [Range(0, 100)]
    public int Flora;
    [Range(0, 100)]
    public int Fauna;
    public Color Color;
    public S_Paramiter<int[]>[] Paramiters;
    public int[] Neighbours;
    public List<int> Events;
}

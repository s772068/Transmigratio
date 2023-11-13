using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_Country {
    public int Population;
    public int EventChanceIndex;
    [Range(0, 100)]
    public int Flora;
    [Range(0, 100)]
    public int Fauna;
    public int[] Climate;
    public int[] Terrain;
    public int[] Production;
    public int[] Civilization;
    public int[] Economics;
    public int[] Goverment;
    public List<int> Neighbours;
    public List<int> Events;
}

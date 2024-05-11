using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

/// <summary>
/// Параметры цивилизаций, такие как способ производства, экономический уклад и политический строй
/// </summary>
[System.Serializable]
public class CivParam
{
    public SerializedDictionary<string, int> quantities;

    public void Init()
    {
        

    }
    public void SetValues()
    {

    }
}

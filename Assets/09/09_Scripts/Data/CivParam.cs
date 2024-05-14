using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;
using System;

/// <summary>
/// Параметры цивилизаций, такие как способ производства, экономический уклад и политический строй
/// </summary>
[System.Serializable]
public class CivParam
{
    public SerializedDictionary<string, int> quantities;
    public SerializedDictionary<string, int> procentQuantities;
    public string currentMax;                              //текущее максимальное значение


    public void Init()
    {
        
        RefreshParam();
    }
    public void SetValues()
    {

    }
    public void SetCurrent() //определяем максимум в quantities и задаём current=max
    {
        if (quantities.FirstOrDefault(x => x.Value == quantities.Values.Max()).Value >= 0)
            currentMax = quantities.FirstOrDefault(x => x.Value == quantities.Values.Max()).Key;
        else currentMax = "none";
    }
    public void QuantitiesToProcents()
    {
        procentQuantities = new SerializedDictionary<string, int>();
        procentQuantities.Clear();
        float sum = quantities.Sum(x => x.Value);
        foreach (var kvp in quantities)
        {
            int proc = (int)(kvp.Value / sum * 100);
            procentQuantities.Add(kvp.Key, proc);            
        }
    }
    public void RefreshParam()
    {
        QuantitiesToProcents();
        if (quantities != null)
        {
            SetCurrent();
        }
    }
}

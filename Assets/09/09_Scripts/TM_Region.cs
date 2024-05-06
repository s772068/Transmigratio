using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using AYellowpaper.SerializedCollections;
using static UnityEngine.EventSystems.EventTrigger;
[System.Serializable]
public class TM_Region
{
    public int id; //�������� �������, ������������ wmsk id
    public string name;
    public Population population;

    public EcologyParam flora;
    public EcologyParam fauna;
    public EcologyParam climate;
    public EcologyParam terrain;
    List<EcologyParam> ecologyParams = new List<EcologyParam>();


    public List<CivPiece> civPieces;
    public void Init()
    {
        ecologyParams.Clear();
        ecologyParams.Add(flora);
        ecologyParams.Add(fauna);
        ecologyParams.Add(climate);
        ecologyParams.Add(terrain);

        foreach (EcologyParam param in ecologyParams) { param.Init(); }
    }
    public void RefreshRegion() //���������� � �������� ���� �������� �������
    {
        foreach (EcologyParam param in ecologyParams) { param.RefreshParam(); }
    }
}

[System.Serializable]
public class EcologyParam
{

    public string currentMax;                              //������� ������������ ��������
    public float richness;                              //��������� ��������� (��� �����, �����)
    public bool isRichnessApplicable = true;
    // public S_Dictionary<string, float> quantities;        //�������� ��� ������ ������������. ���� forest:15, steppe:40. ����� current="steppe" (��������������� ������ ��� ����� SetCurrent())
    public SerializedDictionary<string, float> quantities;
    public void Init()
    {
        if (richness == -1) { isRichnessApplicable = false; }
        
        RefreshParam();
    }
    public void SetCurrent() //���������� �������� � quantities � ����� current=max
    {
        // if (quantities.FirstOrDefault(x => x.Value == quantities.Values.Max()).Value >= 0)
        //     currentMax = quantities.FirstOrDefault(x => x.Value == quantities.Values.Max()).Key;
        // else currentMax = "none";
    }
    /*
    public void QuantitiesToProcents()
    {
        float sum = quantities.GetValues().Sum();
        for(int i = 0; i < quantities.sources.Count; ++i) {
            var pair = quantities.sources[i];
            quantities[pair.Key] = (float) Math.Round(pair.Value / sum * 100, 1);
        }
    }
    */
    public void RefreshParam()
    {
        //QuantitiesToProcents();
        if (quantities != null) 
        {
            SetCurrent();
        }
    }
}
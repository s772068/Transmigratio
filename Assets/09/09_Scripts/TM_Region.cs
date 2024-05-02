using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using AYellowpaper.SerializedCollections;
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
    public SerializedDictionary<string, float> quantities;        //�������� ��� ������ ������������. ���� forest:15, steppe:40. ����� current="steppe" (��������������� ������ ��� ����� SetCurrent())
    //public Dictionary<string, float> quantities;
    public void Init()
    {
        if (richness == -1) { isRichnessApplicable = false; }
        
        RefreshParam();
    }
    public void SetCurrent() //���������� �������� � quantities � ����� current=max
    {
        if (quantities.FirstOrDefault(x => x.Value == quantities.Values.Max()).Value >= 0)
            currentMax = quantities.FirstOrDefault(x => x.Value == quantities.Values.Max()).Key;
        else currentMax = "none";
    }
    public void QuantitiesToProcents()
    {
        float sum = quantities.Sum(x => x.Value);
        foreach (KeyValuePair<string, float> entry in quantities)
        {
            quantities[entry.Key] = (float)Math.Round(entry.Value / sum * 100, 1);
        }
    }
    public void RefreshParam()
    {
        //QuantitiesToProcents();
        if (quantities != null) 
        {
            SetCurrent();
        }
    }
}
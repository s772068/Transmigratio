using System;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class TM_Region
{
    public int id;
    public int id_WMSK;
    public string name;
    public string name2;
    public List<int> extraNeighbours;
    public bool isPopulated;

    public Param terrain;
    public Param climate;
    public Param fauna;
    public Param flora;




    public int population;
    public Param productionMode;
    public Param ecoCulture;
    public Param government;

    public Param civAttachment;
    //public Civilization civAttachment;

    //public Param civilization;
    //public List<Civilization> presentedCivs;

    public List<Param> paramList = new List<Param>();

    public void Init()
    {
        isPopulated = false;
        paramList.Clear();
        paramList.Add(climate); 
        paramList.Add(flora);
        paramList.Add(fauna); 
        paramList.Add(terrain);
        paramList.Add(productionMode); 
        paramList.Add(ecoCulture); 
        paramList.Add(government);
        paramList.Add(civAttachment);

        foreach (Param param in paramList) { param.Init(); }
    }
    /// <summary>
    /// ���������� � �������� ���� �������� �������
    /// </summary>
    public void RefreshRegion() 
    {
        foreach (Param param in paramList) { param.RefreshParam(); }
        

    }
    public void SetPopulatedDefault()           // ��� ������� �������� ��� �������������� ���������
    {
        productionMode.quantities["primitive_communism"] = 100f;
        ecoCulture.quantities["hunters"] = 100f;
        government.quantities["leaderism"] = 100f;
    }
}

[System.Serializable]
public class Param
{
    
    public string currentMax;                              //������� ������������ ��������
    public float richness;                              //��������� ��������� (��� �����, �����)
    public bool isRichnessApplicable = true;
    public Dictionary<string, float> quantities;        //�������� ��� ������ ������������. ���� forest:15, steppe:40. ����� current="steppe" (��������������� ������ ��� ����� SetCurrent())
    
    public void Init()
    {
        if (richness == 0) { isRichnessApplicable = false; }
    }
    /// <summary>
    /// ���������� �������� � quantities � ����� current=max
    /// </summary>
    public void SetCurrent() 
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
        SetCurrent();
        
    }
}


using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// ��������� Civilization - ��� ���� �����������
/// </summary>
[System.Serializable]
public class Civilization
{
    public int civIndex;
    public string name;

    public List<CivPiece> civPiecesList;      //��������� ��������� ����������� - �������� � "��������"

    public CivParam ecoCulture;
    public CivParam prodMode;
    public CivParam government;

    //�� ����� �������� � CivParam
    public float prodModeK = 0.6f;                 // ����������� ������� ������������
    public float governmentCorruption = 0.4f;      // ��������� 

    public Population population;

    public Civilization ancestor;

    public void Init(TM_Region region)        //������ ��� ������ ����. ��� ������ ����������� ����� ����������
    {
        civPiecesList = new List<CivPiece>();
        civPiecesList.Clear();
        population = new Population();
        civIndex = 0;
        name = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "uncivTitle");
        AddPiece(region, 1000, 100);

        TotalCivilizationPopCalculation();
        Debug.Log("Civilization init. \rid:" + civIndex + "\rpopulation:" + population.Value + "\rregionID:" + region.name);
    }

    public void TotalCivilizationPopCalculation()
    {
        population.value = civPiecesList.Sum(x => x.population.Value);
    }
    public void AddPiece(TM_Region region, int pop, float reserve)    //����� ����������� ���������� �� ����� ����������, ������� ����� ��������� CivPiece. ������� ���� ��������� ���������, id �������
    {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, pop, this.civIndex, reserve);
        civPiecesList.Add(newPieceOfCiv);
        //region.AddCivPiece(newPieceOfCiv);
    }
    public void RemovePiece(TM_Region region)      // ������� ����������� �� ����� �������
    {
        foreach (var piece in civPiecesList)
        {
            if (piece.regionResidence == region) { civPiecesList.Remove(piece); }
        }
    }

}
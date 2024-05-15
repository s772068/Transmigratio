using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void Init(int regionIndex)        //������ ��� ������ ����. ��� ������ ����������� ����� ����������
    {
        civPiecesList = new List<CivPiece>();
        civPiecesList.Clear();
        population = new Population();
        civIndex = 0;
        name = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "uncivTitle");
        AddPiece(regionIndex, 1000, 100);

        TotalCivilizationPopCalculation();
        Debug.Log("Civilization init. \rid:" + civIndex + "\rpopulation:" + population.Value + "\rregionID:" + regionIndex);
    }

    public void TotalCivilizationPopCalculation()
    {
        population.value = civPiecesList.Sum(x => x.population.Value);
    }
    public void AddPiece(int regionIndex, int pop, float reserve)    //����� ����������� ���������� �� ����� ����������, ������� ����� ��������� CivPiece. ������� ���� ��������� ���������, id �������
    {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(regionIndex, pop, this.civIndex, reserve);
        civPiecesList.Add(newPieceOfCiv);
        //region.AddCivPiece(newPieceOfCiv);
    }
    public void RemovePiece(int region)      // ������� ����������� �� ����� �������
    {
        foreach (var piece in civPiecesList)
        {
            if (piece.regionResidenceIndex == region) { civPiecesList.Remove(piece); }
        }
    }

}
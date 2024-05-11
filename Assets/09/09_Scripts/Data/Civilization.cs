using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// ��������� Civilization - ��� ���� �����������
/// </summary>
[System.Serializable]
public class Civilization
{
    public Population population;
    public int civId;
    public string name;

    public List<CivPiece> civPiecesList;      //��������� ��������� ����������� - �������� � "��������"

    public CivParam ecoCulture;
    public CivParam prodMode;
    public CivParam government;

    public Civilization ancestor;

    public void Init(int wmsk_id)        //������ ��� ������ ����. ��� ������ ����������� ����� ����������
    {
        civId = 0;
        name = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "uncivTitle");
        AddPiece(wmsk_id, 1000);

        TotalCivilizationPopCalculation();
        Debug.Log("Civilization init. \rid:" + civId + "\rpopulation:" + population.Value + "\rregionID:" + wmsk_id);
    }

    public int TotalCivilizationPopCalculation()
    {
        return civPiecesList.Sum(x => x.population.Value);
    }
    public void AddPiece(int wmsk_id, int pop)    //����� ����������� ���������� �� ����� ����������, ������� ����� ��������� CivPiece. ������� ���� ��������� ��������� � id �������
    {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(wmsk_id, pop, this);
        civPiecesList.Add(newPieceOfCiv);
    }
    public void RemovePiece(int wmsk_id)
    {
        foreach (var piece in civPiecesList)
        {
            if (piece.regionId == wmsk_id) { civPiecesList[piece.regionId] = null; }
        }
    }
}
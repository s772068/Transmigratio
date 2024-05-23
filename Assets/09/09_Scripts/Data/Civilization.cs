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

    public Paramiter ecoCulture = new(true);
    public Paramiter prodMode = new(true);
    public Paramiter government = new(true);

    //�� ����� �������� � CivParam
    public float prodModeK = 0.6f;                 // ����������� ������� ������������
    public float governmentCorruption = 0.4f;      // ���������

    public int Population => civPiecesList.Sum(x => x.population.Value);

    public Civilization ancestor;

    public void Init(int regionIndex)        //������ ��� ������ ����. ��� ������ ����������� ����� ����������
    {
        civPiecesList = new List<CivPiece>();
        civPiecesList.Clear();
        civIndex = 0;
        name = "uncivTitle";
        AddPiece(regionIndex, GameSettings.startPopulation, 100);

        ecoCulture.Init("hunters", "farmers", "nomads", "mountain", "city");
        prodMode.Init("primitive communism", "slavery", "feodalism", "capitalism", "socialism", "communism");
        government.Init("leaderism", "monarchy", "city - state", "imperium", "federation", "national state", "anarchy");

        Debug.Log("Civilization init. \rid:" + civIndex + "\rpopulation:" + Population + "\rregionID:" + regionIndex);
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
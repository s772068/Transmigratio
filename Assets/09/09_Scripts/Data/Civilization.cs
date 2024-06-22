using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��������� Civilization - ��� ���� �����������
/// </summary>
[System.Serializable]
public class Civilization {
    public string name;

    public SerializedDictionary<int, CivPiece> pieces;      //��������� ��������� ����������� - �������� � "��������"

    public Paramiter ecoCulture = new(true);
    public Paramiter prodMode = new(true);
    public Paramiter government = new(true);

    //�� ����� �������� � CivParam
    public float prodModeK = 0.6f;                 // ����������� ������� ������������
    public float governmentCorruption = 0.4f;      // ���������

    public int Population => pieces.Sum(x => x.Value.population.Value);

    public void Init(TM_Region region, string civName)        //������ ��� ������ ����. ��� ������ ����������� ����� ����������
    {
        pieces = new();
        pieces.Clear();
        name = civName;
        AddPiece(region, GameSettings.startPopulation, 100);

        ecoCulture.Init("Hunters", "Farmers", "Nomads", "Mountain", "City");
        prodMode.Init("PrimitiveCommunism", "Slavery", "Feodalism", "Capitalism", "Socialism", "Communism");
        government.Init("Leaderism", "Monarchy", "CityState", "Imperium", "Federation", "NationalState", "Anarchy");

        Debug.Log("Civilization init. \rpopulation:" + Population + "\rregionID:" + region.id);

        GameEvents.onTickLogic += UpdatePieces;
    }

    /// <summary>
    /// ����� ����������� ���������� �� ����� ����������, ������� ����� ��������� CivPiece. ������� ���� ��������� ���������, id �������
    /// </summary>
    public void AddPiece(TM_Region region, int population, float reserve) {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, this, population, reserve);
        pieces[region.id] = newPieceOfCiv;
        //region.AddCivPiece(newPieceOfCiv);
    }
    /// <summary>
    /// ������� ����������� �� ����� �������
    /// </summary>
    public void RemovePiece(int region) {
        pieces.Remove(region);
    }

    public void UpdatePieces() {
        foreach (var pair in pieces) {
            pair.Value.DeltaPop();
        }
    }

}
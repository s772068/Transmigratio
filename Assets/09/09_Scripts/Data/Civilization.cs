using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��������� Civilization - ��� ���� �����������
/// </summary>
[System.Serializable]
public class Civilization {
    public string Name;

    public SerializedDictionary<int, CivPiece> Pieces;      //��������� ��������� ����������� - �������� � "��������"

    public Paramiter EcoCulture = new(true);
    public Paramiter ProdMode = new(true);
    public Paramiter Government = new(true);

    //�� ����� �������� � CivParam
    public float ProdModeK = 0.6f;                 // ����������� ������� ������������
    public float GovernmentCorruption = 0.4f;      // ���������

    public int Population => Pieces.Sum(x => x.Value.Population.Value);

    public void Init(int region, string civName)        //������ ��� ������ ����. ��� ������ ����������� ����� ����������
    {
        Pieces = new();
        Pieces.Clear();
        Name = civName;
        AddPiece(region, GameSettings.startPopulation, 100);

        EcoCulture.Init("Hunters", "Farmers", "Nomads", "Mountain", "City");
        ProdMode.Init("PrimitiveCommunism", "Slavery", "Feodalism", "Capitalism", "Socialism", "Communism");
        Government.Init("Leaderism", "Monarchy", "CityState", "Imperium", "Federation", "NationalState", "Anarchy");

        Debug.Log("Civilization init. \rpopulation:" + Population + "\rregionID:" + region);

        GameEvents.TickLogic += UpdatePieces;
    }

    /// <summary>
    /// ����� ����������� ���������� �� ����� ����������, ������� ����� ��������� CivPiece. ������� ���� ��������� ���������, id �������
    /// </summary>
    public void AddPiece(int region, int population, float reserve) {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, Name, population, reserve);
        newPieceOfCiv.Destroy = () => RemovePiece(region);
        Pieces[region] = newPieceOfCiv;
        //region.AddCivPiece(newPieceOfCiv);
    }
    /// <summary>
    /// ������� ����������� �� ����� �������
    /// </summary>
    public void RemovePiece(int region) {
        GameEvents.RemoveCivPiece(Pieces[region]);
        Pieces[region].Region.CivsList.Remove(Name);
        Pieces.Remove(region);
    }

    public void UpdatePieces() {
        for (int i = 0; i < Pieces.Count; ++i) {
            Pieces.ElementAt(i).Value.DeltaPop();
        }
    }

}
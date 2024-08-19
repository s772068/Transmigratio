using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;
using System;

/// <summary>
/// ��������� Civilization - ��� ���� �����������
/// </summary>
[System.Serializable]
public class Civilization {
    public string Name;

    public SerializedDictionary<int, CivPiece> Pieces;      //��������� ��������� ����������� - �������� � "��������"

    public Paramiter EcoCulture { get {
            Paramiter res = new(true);
            foreach (var piece in Pieces) {
                res += piece.Value.EcoCulture;
            }
            return res;
        }
    }
    public Paramiter ProdMode { get {
            Paramiter res = new(true);
            foreach (var piece in Pieces) {
                res += piece.Value.ProdMode;
            }
            return res;
        }
    }
    public Paramiter Government { get {
            Paramiter res = new(true);
            foreach (var piece in Pieces) {
                res += piece.Value.Government;
            }
            return res;
        }
    }

    //�� ����� �������� � CivParam
    /// <summary>
    /// ����������� ������� ������������
    /// </summary>
    public float GovernmentCorruption = 0.4f;      // ���������

    public static Action<CivPiece> onAddPiece;
    public static Action<CivPiece> onRemovePiece;

    public int Population => Pieces.Sum(x => x.Value.Population.Value);

    public Civilization(string civName) {
        Pieces = new();
        Name = civName;
    }

    /// <summary>
    /// ������ ��� ������ ����. ��� ������ ����������� ����� ����������
    /// </summary>
    public void StartGame(int region) {
        AddPiece(region, GameSettings.StartPopulation);
        Debug.Log("Civilization init. \rpopulation:" + Population + "\rregionID:" + region);
    }

    /// <summary>
    /// ����� ����������� ���������� �� ����� ����������, ������� ����� ��������� CivPiece. ������� ���� ��������� ���������, id �������
    /// </summary>
    public void AddPiece(int region, int population) {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, Name, population);
        newPieceOfCiv.Destroy = () => RemovePiece(region);
        Pieces[region] = newPieceOfCiv;
        onAddPiece?.Invoke(newPieceOfCiv);
        //region.AddCivPiece(newPieceOfCiv);
    }

    public void AddPiece(CivPiece piece, string newCivName, int newCategory) {
        Pieces[piece.RegionID] = piece;
        Pieces[piece.RegionID].CivName = newCivName;
        Pieces[piece.RegionID].Category = newCategory;
    }

    /// <summary>
    /// ������� ����������� �� ����� �������
    /// </summary>
    public void RemovePiece(int region) {
        onRemovePiece?.Invoke(Pieces[region]);
        Pieces[region].Region.CivsList.Remove(Name);
        Pieces.Remove(region);
    }

    public void Play() {
        int count = Pieces.Count;
        for (int i = 0; i < count; ++i) {
            Pieces.ElementAt(i).Value.Play();
        }
    }
}
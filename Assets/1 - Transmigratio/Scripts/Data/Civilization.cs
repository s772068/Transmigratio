using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using WorldMapStrategyKit;

/// <summary>
/// ��������� Civilization - ��� ���� �����������
/// </summary>
[System.Serializable]
public class Civilization {
    public string Name;

    public SerializedDictionary<int, CivPiece> Pieces;      //��������� ��������� ����������� - �������� � "��������"

    public float AllGold => Pieces.Sum(x => x.Value.gold);

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
        piece.Region.AddCivilization(newCivName);
        CivPiece newPieceOfCiv = new();
        newPieceOfCiv.Init(piece, newCivName, newCategory);
        newPieceOfCiv.Destroy = () => RemovePiece(newPieceOfCiv.RegionID);
        Pieces[newPieceOfCiv.RegionID] = newPieceOfCiv;
        onAddPiece?.Invoke(newPieceOfCiv);
    }

    /// <summary>
    /// ������� ����������� �� ����� �������
    /// </summary>
    public void RemovePiece(int region) {
        if (!Pieces.ContainsKey(region)) 
            return;

        onRemovePiece?.Invoke(Pieces[region]);
        Pieces[region].Region.CivsList.Remove(Name);
        Pieces.Remove(region);
    }

    //UNDONE: ������ ��� ���������� ��������� ������
    public void Play() {
        Dictionary<int, CivPiece> tickPieces = new(Pieces);
        foreach (var piece in tickPieces) {
            piece.Value.Play();
        }
    }
}
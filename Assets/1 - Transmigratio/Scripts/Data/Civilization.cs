using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using Gameplay.Scenarios;
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

    public Paramiter EcoCulture = new(true);
    public Paramiter ProdMode = new(true);
    public Paramiter Government = new(true);

    //�� ����� �������� � CivParam
    /// <summary>
    /// ����������� ������� ������������
    /// </summary>
    public float ProdModeK {
        get {
            List<float> prodModeList = new() {
                ProdMode["PrimitiveCommunism"].Value,
                ProdMode["Slavery"].Value,
                ProdMode["Feodalism"].Value
            };
            return prodModeList.IndexOf(prodModeList.Max()) switch {
                0 => Demography.data.prodModeK_PC,
                1 => Demography.data.prodModeK_S,
                _ => 1
            };
        }
    }
    public bool IsFarmers {
        get {
            List<float> list = new() {
                EcoCulture["Farmers"].Value,
                EcoCulture["Hunters"].Value,
                EcoCulture["Nomads"].Value
            };
            return list.IndexOf(list.Max()) == 0;
        }
    }
    public float GovernmentCorruption = 0.4f;      // ���������


    public static Action<CivPiece> RemoveCivPiece;

    public int Population => Pieces.Sum(x => x.Value.Population.Value);

    public void Init(int region, string civName)        //������ ��� ������ ����. ��� ������ ����������� ����� ����������
    {
        Pieces = new();
        Pieces.Clear();
        Name = civName;

        EcoCulture.Init(("Hunters", GameSettings.StartHunters));
        EcoCulture.Init("Farmers", "Nomads", "Mountain", "City");

        ProdMode.Init(("PrimitiveCommunism", GameSettings.StartPrimitiveCommunism));
        ProdMode.Init("Slavery", "Feodalism", "Capitalism", "Socialism", "Communism");

        Government.Init(("Leaderism", GameSettings.StartLeaderism));
        Government.Init("Monarchy", "CityState", "Imperium", "Federation", "NationalState", "Anarchy");
        
        AddPiece(region, GameSettings.StartPopulation);

        Debug.Log("Civilization init. \rpopulation:" + Population + "\rregionID:" + region);
    }

    /// <summary>
    /// ����� ����������� ���������� �� ����� ����������, ������� ����� ��������� CivPiece. ������� ���� ��������� ���������, id �������
    /// </summary>
    public void AddPiece(int region, int population) {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, Name, population, ProdModeK, IsFarmers);
        newPieceOfCiv.Destroy = () => RemovePiece(region);
        Pieces[region] = newPieceOfCiv;
        //region.AddCivPiece(newPieceOfCiv);
    }
    /// <summary>
    /// ������� ����������� �� ����� �������
    /// </summary>
    public void RemovePiece(int region) {
        RemoveCivPiece?.Invoke(Pieces[region]);
        Pieces[region].Region.CivsList.Remove(Name);
        Pieces.Remove(region);
    }

    public void Play() {
        for (int i = 0; i < Pieces.Count; ++i) {
            Pieces.ElementAt(i).Value.Play();
        }
    }
}
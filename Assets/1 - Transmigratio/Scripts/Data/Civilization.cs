using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;
using System;

/// <summary>
/// Экземпляр Civilization - это одна цивилизация
/// </summary>
[System.Serializable]
public class Civilization {
    public string Name;

    public SerializedDictionary<int, CivPiece> Pieces;      //суммируем население цивилизации - собираем с "кусочков"

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

    //их нужно засунуть в CivParam
    /// <summary>
    /// Коэффициент способа производства
    /// </summary>
    public float GovernmentCorruption = 0.4f;      // коррупция

    public static Action<CivPiece> onAddPiece;
    public static Action<CivPiece> onRemovePiece;

    public int Population => Pieces.Sum(x => x.Value.Population.Value);

    public Civilization(string civName) {
        Pieces = new();
        Name = civName;
    }

    /// <summary>
    /// версия для старта игры. Для других цивилизаций нужна перегрузка
    /// </summary>
    public void StartGame(int region) {
        AddPiece(region, GameSettings.StartPopulation);
        Debug.Log("Civilization init. \rpopulation:" + Population + "\rregionID:" + region);
    }

    /// <summary>
    /// Когда цивилизация появляется на новой территории, создаем новый экземпляр CivPiece. Передаём туда стартовое население, id региона
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
    /// уберает цивилизацию из этого региона
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
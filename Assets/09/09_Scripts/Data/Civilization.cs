using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Ёкземпл€р Civilization - это одна цивилизаци€
/// </summary>
[System.Serializable]
public class Civilization {
    public string Name;

    public SerializedDictionary<int, CivPiece> Pieces;      //суммируем население цивилизации - собираем с "кусочков"

    public Paramiter EcoCulture = new(true);
    public Paramiter ProdMode = new(true);
    public Paramiter Government = new(true);

    //их нужно засунуть в CivParam
    public float ProdModeK = 0.6f;                 // коэффициент способа производства
    public float GovernmentCorruption = 0.4f;      // коррупци€

    public int Population => Pieces.Sum(x => x.Value.Population.Value);

    public void Init(int region, string civName)        //верси€ дл€ старта игры. ƒл€ других цивилизаций нужна перегрузка
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
    ///  огда цивилизаци€ по€вл€етс€ на новой территории, создаем новый экземпл€р CivPiece. ѕередаЄм туда стартовое население, id региона
    /// </summary>
    public void AddPiece(int region, int population, float reserve) {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, Name, population, reserve);
        newPieceOfCiv.Destroy = () => RemovePiece(region);
        Pieces[region] = newPieceOfCiv;
        //region.AddCivPiece(newPieceOfCiv);
    }
    /// <summary>
    /// уберает цивилизацию из этого региона
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
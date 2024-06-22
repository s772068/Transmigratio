using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Ёкземпл€р Civilization - это одна цивилизаци€
/// </summary>
[System.Serializable]
public class Civilization {
    public string name;

    public SerializedDictionary<int, CivPiece> pieces;      //суммируем население цивилизации - собираем с "кусочков"

    public Paramiter ecoCulture = new(true);
    public Paramiter prodMode = new(true);
    public Paramiter government = new(true);

    //их нужно засунуть в CivParam
    public float prodModeK = 0.6f;                 // коэффициент способа производства
    public float governmentCorruption = 0.4f;      // коррупци€

    public int Population => pieces.Sum(x => x.Value.population.Value);

    public void Init(TM_Region region, string civName)        //верси€ дл€ старта игры. ƒл€ других цивилизаций нужна перегрузка
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
    ///  огда цивилизаци€ по€вл€етс€ на новой территории, создаем новый экземпл€р CivPiece. ѕередаЄм туда стартовое население, id региона
    /// </summary>
    public void AddPiece(TM_Region region, int population, float reserve) {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, this, population, reserve);
        pieces[region.id] = newPieceOfCiv;
        //region.AddCivPiece(newPieceOfCiv);
    }
    /// <summary>
    /// уберает цивилизацию из этого региона
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
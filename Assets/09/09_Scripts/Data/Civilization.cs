using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Ёкземпл€р Civilization - это одна цивилизаци€
/// </summary>
[System.Serializable]
public class Civilization
{
    public int civIndex;
    public string name;

    public List<CivPiece> civPiecesList;      //суммируем население цивилизации - собираем с "кусочков"

    public Paramiter ecoCulture = new(true);
    public Paramiter prodMode = new(true);
    public Paramiter government = new(true);

    //их нужно засунуть в CivParam
    public float prodModeK = 0.6f;                 // коэффициент способа производства
    public float governmentCorruption = 0.4f;      // коррупци€

    public int Population => civPiecesList.Sum(x => x.population.Value);

    public Civilization ancestor;

    public void Init(int regionIndex)        //верси€ дл€ старта игры. ƒл€ других цивилизаций нужна перегрузка
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

    public void AddPiece(int regionIndex, int pop, float reserve)    //когда цивилизаци€ по€вл€етс€ на новой территории, создаем новый экземпл€р CivPiece. ѕередаЄм туда стартовое население, id региона
    {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(regionIndex, pop, this.civIndex, reserve);
        civPiecesList.Add(newPieceOfCiv);
        //region.AddCivPiece(newPieceOfCiv);
    }

    public void RemovePiece(int region)      // убирает цивилизацию из этого региона
    {
        foreach (var piece in civPiecesList)
        {
            if (piece.regionResidenceIndex == region) { civPiecesList.Remove(piece); }
        }
    }

}
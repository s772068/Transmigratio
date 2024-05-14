using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// Ёкземпл€р Civilization - это одна цивилизаци€
/// </summary>
[System.Serializable]
public class Civilization
{
    public int civIndex;
    public string name;

    public List<CivPiece> civPiecesList;      //суммируем население цивилизации - собираем с "кусочков"

    public CivParam ecoCulture;
    public CivParam prodMode;
    public CivParam government;

    //их нужно засунуть в CivParam
    public float prodModeK = 0.6f;                 // коэффициент способа производства
    public float governmentCorruption = 0.4f;      // коррупци€ 

    public Population population;

    public Civilization ancestor;

    public void Init(TM_Region region)        //верси€ дл€ старта игры. ƒл€ других цивилизаций нужна перегрузка
    {
        civPiecesList = new List<CivPiece>();
        civPiecesList.Clear();
        population = new Population();
        civIndex = 0;
        name = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "uncivTitle");
        AddPiece(region, 1000, 100);

        TotalCivilizationPopCalculation();
        Debug.Log("Civilization init. \rid:" + civIndex + "\rpopulation:" + population.Value + "\rregionID:" + region.name);
    }

    public void TotalCivilizationPopCalculation()
    {
        population.value = civPiecesList.Sum(x => x.population.Value);
    }
    public void AddPiece(TM_Region region, int pop, float reserve)    //когда цивилизаци€ по€вл€етс€ на новой территории, создаем новый экземпл€р CivPiece. ѕередаЄм туда стартовое население, id региона
    {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(region, pop, this.civIndex, reserve);
        civPiecesList.Add(newPieceOfCiv);
        //region.AddCivPiece(newPieceOfCiv);
    }
    public void RemovePiece(TM_Region region)      // убирает цивилизацию из этого региона
    {
        foreach (var piece in civPiecesList)
        {
            if (piece.regionResidence == region) { civPiecesList.Remove(piece); }
        }
    }

}
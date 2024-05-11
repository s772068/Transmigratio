using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// Ёкземпл€р Civilization - это одна цивилизаци€
/// </summary>
[System.Serializable]
public class Civilization
{
    public Population population;
    public int civId;
    public string name;

    public List<CivPiece> civPiecesList;      //суммируем население цивилизации - собираем с "кусочков"

    public CivParam ecoCulture;
    public CivParam prodMode;
    public CivParam government;

    public Civilization ancestor;

    public void Init(int wmsk_id)        //верси€ дл€ старта игры. ƒл€ других цивилизаций нужна перегрузка
    {
        civId = 0;
        name = LocalizationSettings.StringDatabase.GetLocalizedString("TransmigratioLocalizationTable", "uncivTitle");
        AddPiece(wmsk_id, 1000);

        TotalCivilizationPopCalculation();
        Debug.Log("Civilization init. \rid:" + civId + "\rpopulation:" + population.Value + "\rregionID:" + wmsk_id);
    }

    public int TotalCivilizationPopCalculation()
    {
        return civPiecesList.Sum(x => x.population.Value);
    }
    public void AddPiece(int wmsk_id, int pop)    //когда цивилизаци€ по€вл€етс€ на новой территории, создаем новый экземпл€р CivPiece. ѕередаЄм туда стартовое население и id региона
    {
        CivPiece newPieceOfCiv = new CivPiece();
        newPieceOfCiv.Init(wmsk_id, pop, this);
        civPiecesList.Add(newPieceOfCiv);
    }
    public void RemovePiece(int wmsk_id)
    {
        foreach (var piece in civPiecesList)
        {
            if (piece.regionId == wmsk_id) { civPiecesList[piece.regionId] = null; }
        }
    }
}
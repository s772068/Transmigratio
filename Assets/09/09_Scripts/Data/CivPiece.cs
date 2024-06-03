using System.Collections.Generic;
using WorldMapStrategyKit;
using System;
using UnityEngine;


/// <summary>
/// Ёкземпл€р CivPiece - это один "кусочек" цивилизации в конкретном регионе. 
/// ≈сли цивилизаци€ есть в трЄх регионах, это значит, что она состоит из трЄх объектов CivPiece
/// </summary>
[System.Serializable]
public class CivPiece {
    //public int regionId;                //wmsk_id того региона, где живЄт этот объект
    public TM_Region region;   //может лучше сам регион, а не айди?
    
    //public Civilization civBelonging;      // если мы обращаемс€ к CivPiece только из списка civPiecesList, эта штука нам не нужна
    public Civilization civilization;

    public Population population;
    public float populationGrow;
    public float givenFood;
    public float requestFood;
    public float reserveFood;
    public float takenFood;

    private Map Map => Transmigratio.Instance.tmdb.map;
    private WMSK WMSK => Map.wmsk;

    /// <summary>
    /// »нициализаци€ при по€влении в области после миграции или при старте игры
    /// </summary>
    public void Init(TM_Region region, Civilization civilization, int startPopulation, float reserve) {
        this.region = region;
        population = new Population();
        population.value = startPopulation;
        this.civilization = civilization;
        reserveFood = reserve;  //изначальное количество еды у кусочка
    }

    /// <summary>
    /// »зменение населени€ кусочка за тик
    /// </summary>
    public void DeltaPop() {
        float faunaKr = (float)(Math.Pow(region.fauna.GetMax().Value, 0.58d) / 10);
        takenFood = population.value / 100 * faunaKr * civilization.prodModeK;
        requestFood = population.Value / 150f;
        if (reserveFood > requestFood) givenFood = requestFood;
        else                           givenFood = reserveFood;
        reserveFood += takenFood - givenFood;
        populationGrow = population.Value * civilization.governmentCorruption * givenFood / requestFood - population.Value / 3;

        //return (int)(populationGrow);
        population.value += (int)populationGrow;
        //reserveFood < requestFood -> миграци€ туда где больше фауна
        GameEvents.onUpdateDeltaPopulation?.Invoke(this);
    }
}

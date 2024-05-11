using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


/// <summary>
/// Экземпляр CivPiece - это один "кусочек" цивилизации в конкретном регионе. 
/// Если цивилизация есть в трёх регионах, это значит, что она состоит из трёх объектов CivPiece
/// </summary>
public class CivPiece
{
    public int regionId;          //wmsk_id того региона, где живёт этот объект
    public Population population;
    public Civilization belonging;

    public void Init(int wmsk_id, int startPop, Civilization from)         //
    {
        regionId = wmsk_id;
        population = new Population();
        population.Value = startPop;
        belonging = from;
    }
    public void addPieceToRegion(TM_Region region, CivPiece piece)
    {
        region.civPieces.Add(piece);
        
    }
}

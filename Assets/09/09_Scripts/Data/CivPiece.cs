using System.Collections.Generic;
using WorldMapStrategyKit;
using System;
using UnityEngine;


/// <summary>
/// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
/// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
/// </summary>
[System.Serializable]
public class CivPiece {
    //public int regionId;                //wmsk_id ���� �������, ��� ���� ���� ������
    public TM_Region region;   //����� ����� ��� ������, � �� ����?
    
    //public Civilization civBelonging;      // ���� �� ���������� � CivPiece ������ �� ������ civPiecesList, ��� ����� ��� �� �����
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
    /// ������������� ��� ��������� � ������� ����� �������� ��� ��� ������ ����
    /// </summary>
    public void Init(TM_Region region, Civilization civilization, int startPopulation, float reserve) {
        this.region = region;
        population = new Population();
        population.value = startPopulation;
        this.civilization = civilization;
        reserveFood = reserve;  //����������� ���������� ��� � �������
    }

    /// <summary>
    /// ��������� ��������� ������� �� ���
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
        //reserveFood < requestFood -> �������� ���� ��� ������ �����
        GameEvents.onUpdateDeltaPopulation?.Invoke(this);
    }
}

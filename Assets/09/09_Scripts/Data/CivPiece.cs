using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;


/// <summary>
/// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
/// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
/// </summary>
[System.Serializable]
public class CivPiece
{
    //public int regionId;                //wmsk_id ���� �������, ��� ���� ���� ������
    public TM_Region regionResidence;   //����� ����� ��� ������, � �� ����?
    
    //public Civilization civBelonging;      // ���� �� ���������� � CivPiece ������ �� ������ civPiecesList, ��� ����� ��� �� �����
    public int civIndex;

    public Population population;
    public float populationGrow;
    public float givenFood;
    public float requestFood;
    public float reserveFood;
    public float takenFood;

    public void Init(TM_Region to, int startPop, int index, float reserve)         // ������������� ��� ��������� � ������� ����� �������� ��� ��� ������ ����
    {
        regionResidence = to;
        population = new Population();
        population.value = startPop;
        civIndex = index;
        reserveFood = reserve;  //����������� ���������� ��� � �������
    }
    public int DeltaPop()        // ��������� ��������� ������� �� ���
    {
        Civilization civBelonging = Humanity.GetCivByIndex(civIndex);
        float faunaKr = (float)(Math.Pow(regionResidence.fauna.richness, 0.58d) / 10);
        takenFood = population.value / 100 * faunaKr * civBelonging.prodModeK;
        requestFood = population.Value / 150f;
        if (reserveFood > requestFood) { givenFood = requestFood; }
        else { givenFood = reserveFood; }
        reserveFood += takenFood - givenFood;
        populationGrow = population.Value * civBelonging.governmentCorruption * givenFood / requestFood - population.Value / 3;

        return (int)(populationGrow);
    }
}

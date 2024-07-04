using WorldMapStrategyKit;
using System;
using UnityEngine;
using Unity.VisualScripting;


/// <summary>
/// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
/// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
/// </summary>
[System.Serializable]
public class CivPiece {
    public Population population;
    public float populationGrow;
    public float givenFood;
    public float requestFood;
    public float reserveFood;
    public float takenFood;
    
    public int regionID;
    public string civName;

    public Action onDestroy;

    public TM_Region Region => TMDB.map.allRegions[regionID];
    public Civilization Civilization => TMDB.humanity.civilizations[civName];
    private TMDB TMDB => Transmigratio.Instance.tmdb;

    /// <summary>
    /// ������������� ��� ��������� � ������� ����� �������� ��� ��� ������ ����
    /// </summary>
    public void Init(int region, string civilization, int startPopulation, float reserve) {
        regionID = region;
        population = new Population();
        population.value = startPopulation;
        civName = civilization;
        reserveFood = reserve;  //����������� ���������� ��� � �������
    }

    /// <summary>
    /// ��������� ��������� ������� �� ���
    /// </summary>
    public void DeltaPop() {
        float faunaKr = (float)(Math.Pow(Region.fauna.GetMaxQuantity().value, 0.58d) / 10f);
        takenFood = population.value / 100f * faunaKr * Civilization.prodModeK;
        requestFood = population.Value / 150f;
        if (reserveFood > requestFood) givenFood = requestFood;
        else                           givenFood = reserveFood;
        reserveFood += takenFood - givenFood;
        populationGrow = population.Value * Civilization.governmentCorruption * givenFood / requestFood - population.Value / 3f;

        population.value += (int)populationGrow;
        if (population.value <= 50) { onDestroy?.Invoke(); return; }

        if (populationGrow < 0) {
            GameEvents.onActivateHunger?.Invoke(this);
            MigrationController.Instance.TryMigration(this);
        } else {
            GameEvents.onDeactivateHunger?.Invoke(this);
        }
    }
}

using WorldMapStrategyKit;
using System;


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
        float faunaKr = (float)(Math.Pow(region.fauna.GetMaxQuantity().value, 0.58d) / 10f);
        takenFood = population.value / 100f * faunaKr * civilization.prodModeK;
        requestFood = population.Value / 150f;
        if (reserveFood > requestFood) givenFood = requestFood;
        else                           givenFood = reserveFood;
        reserveFood += takenFood - givenFood;
        populationGrow = population.Value * civilization.governmentCorruption * givenFood / requestFood - population.Value / 3f;

        //return (int)(populationGrow);
        population.value += (int)populationGrow;
        if(populationGrow < 0) GameEvents.onActivateHunger?.Invoke(this);
        else                   GameEvents.onDeactivateHunger?.Invoke(this);
    }
}

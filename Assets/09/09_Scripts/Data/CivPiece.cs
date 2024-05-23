using System;


/// <summary>
/// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
/// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
/// </summary>
[System.Serializable]
public class CivPiece {
    //public int regionId;                //wmsk_id ���� �������, ��� ���� ���� ������
    public int regionResidenceIndex;   //����� ����� ��� ������, � �� ����?
    
    //public Civilization civBelonging;      // ���� �� ���������� � CivPiece ������ �� ������ civPiecesList, ��� ����� ��� �� �����
    public int civIndex;

    public Population population;
    public float populationGrow;
    public float givenFood;
    public float requestFood;
    public float reserveFood;
    public float takenFood;

    public TM_Region Region => Transmigratio.Instance.GetRegion(regionResidenceIndex);
    public Civilization Civilization => Transmigratio.Instance.GetCiv(civIndex);

    /// <summary>
    /// ������������� ��� ��������� � ������� ����� �������� ��� ��� ������ ����
    /// </summary>
    public void Init(int regionIndex, int startPop, int index, float reserve) {
        regionResidenceIndex = regionIndex;
        population = new Population();
        population.value = startPop;
        civIndex = index;
        reserveFood = reserve;  //����������� ���������� ��� � �������
        GameEvents.onTickLogic += DeltaPop;
    }

    /// <summary>
    /// ��������� ��������� ������� �� ���
    /// </summary>
    public void DeltaPop() {
        float faunaKr = (float)(Math.Pow(Region.fauna.GetMax().Value, 0.58d) / 10);
        takenFood = population.value / 100 * faunaKr * Civilization.prodModeK;
        requestFood = population.Value / 150f;
        if (reserveFood > requestFood) { givenFood = requestFood; }
        else { givenFood = reserveFood; }
        reserveFood += takenFood - givenFood;
        populationGrow = population.Value * Civilization.governmentCorruption * givenFood / requestFood - population.Value / 3;

        //return (int)(populationGrow);
        population.value += (int)populationGrow;
    }
}

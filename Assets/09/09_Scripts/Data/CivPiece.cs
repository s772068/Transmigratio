using System;


/// <summary>
/// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
/// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
/// </summary>
[System.Serializable]
public class CivPiece
{
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

    public TM_Region GetRegion(int index) => Transmigratio.Instance.GetRegion(index);
    public Civilization GetCiv(int index) => Transmigratio.Instance.GetCiv(index);

    public void Init(int regionIndex, int startPop, int index, float reserve)         // ������������� ��� ��������� � ������� ����� �������� ��� ��� ������ ����
    {
        regionResidenceIndex = regionIndex;
        population = new Population();
        population.value = startPop;
        civIndex = index;
        reserveFood = reserve;  //����������� ���������� ��� � �������
    }
    public int DeltaPop()        // ��������� ��������� ������� �� ���
    {
        Civilization civBelonging = GetCiv(civIndex);
        float faunaKr = (float)(Math.Pow(GetRegion(regionResidenceIndex).fauna.richness, 0.58d) / 10);
        takenFood = population.value / 100 * faunaKr * civBelonging.prodModeK;
        requestFood = population.Value / 150f;
        if (reserveFood > requestFood) { givenFood = requestFood; }
        else { givenFood = reserveFood; }
        reserveFood += takenFood - givenFood;
        populationGrow = population.Value * civBelonging.governmentCorruption * givenFood / requestFood - population.Value / 3;

        return (int)(populationGrow);
    }
}

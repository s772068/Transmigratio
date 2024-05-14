using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// ����� "������������" - ���������/���������� ���� �� ������������
/// </summary>
[System.Serializable]
public class Humanity
{
    public static List<Civilization> civsList;

    public Population totalEarthPop;

    public void Init()
    {
        totalEarthPop = new Population();
        totalEarthPop.Value = 0;
        civsList = new List<Civilization>();
        civsList.Clear();
        Debug.Log("Humanity init");
    }
    public void TotalEartPopCalculation() //��������� �� ��������� ���� ����������� - ��� � ���� ��������� ���� ������
    {
        totalEarthPop.Value = civsList.Sum(x => x.population.Value);
    }
    public void AddCivilization(TM_Region region)       //�������� �������������� ����������� (����� ����)
    {
        Civilization newCiv = new Civilization();
        newCiv.Init(region);
        civsList.Add(newCiv);
        newCiv.civIndex = civsList.Count - 1;
    }
    public static Civilization GetCivByIndex(int index)
    {
        return civsList[index];
    }
}
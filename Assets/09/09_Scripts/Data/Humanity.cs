using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Класс "человечество" - суммарная/усреднённая инфа по цивилизациям
/// </summary>
[System.Serializable]
public class Humanity
{
    public List<Civilization> civsList;

    public Population totalEarthPop;

    public void Init()
    {
        totalEarthPop = new Population();
        totalEarthPop.Value = 0;
        civsList = new List<Civilization>();
        civsList.Clear();
        Debug.Log("Humanity init");
    }
    public void TotalEartPopCalculation() //суммируем всё население всех цивилизаций - это и есть население мира сейчас
    {
        totalEarthPop.Value = civsList.Sum(x => x.population.Value);
    }
    public void AddCivilization(int regionIndex)       //создание первоначальной цивилизации (старт игры)
    {
        Civilization newCiv = new Civilization();
        newCiv.Init(regionIndex);
        civsList.Add(newCiv);
        newCiv.civIndex = civsList.Count - 1;
    }
    public Civilization GetCivByIndex(int index)
    {
        return civsList[index];
    }
}
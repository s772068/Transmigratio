using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Класс "человечество" - суммарная/усреднённая инфа по цивилизациям
/// </summary>
[System.Serializable]
public class Humanity {
    public List<Civilization> civsList;

    public Population totalEarthPop;

    public Civilization GetCivByIndex(int index) => civsList[index];

    public void Init() {
        totalEarthPop = new Population();
        totalEarthPop.Value = 0;
        civsList = new List<Civilization>();
        civsList.Clear();
        Debug.Log("Humanity init");
    }

    /// <summary>
    /// Суммируем всё население всех цивилизаций - это и есть население мира сейчас
    /// </summary>
    public void TotalEartPopCalculation() {
        totalEarthPop.Value = civsList.Sum(x => x.population.Value);
    }

    /// <summary>
    /// Создание первоначальной цивилизации (старт игры)
    /// </summary>
    public int AddCivilization(int regionIndex) {
        Civilization newCiv = new Civilization();
        newCiv.Init(regionIndex);
        civsList.Add(newCiv);
        newCiv.civIndex = civsList.Count - 1;
        return civsList.Count - 1;
    }
}
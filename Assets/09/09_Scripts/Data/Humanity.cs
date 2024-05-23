using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Класс "человечество" - суммарная/усреднённая инфа по цивилизациям
/// </summary>
[System.Serializable]
public class Humanity {
    public List<Civilization> civsList;

    public int TotalEarthPop => civsList.Sum(x => x.Population);

    public Civilization GetCivByIndex(int index) => civsList[index];

    public void Init() {
        civsList = new List<Civilization>();
        civsList.Clear();
        Debug.Log("Humanity init");
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
using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Класс "человечество" - суммарная/усреднённая инфа по цивилизациям
/// </summary>
[System.Serializable]
public class Humanity {
    public SerializedDictionary<string, Civilization> Civilizations;

    public int TotalEarthPop => Civilizations.Sum(x => x.Value.Population) + MigrationController.Instance.Population;

    public void Init() {
        Civilizations = new();
        Civilizations.Clear();
        Debug.Log("Humanity init");
    }

    /// <summary>
    /// Создание первоначальной цивилизации (старт игры)
    /// </summary>
    public Civilization AddCivilization(int region, string civName) {
        Civilization newCiv = new Civilization();
        newCiv.Init(region, civName);
        Civilizations[civName] = newCiv;
        return newCiv;
    }
}
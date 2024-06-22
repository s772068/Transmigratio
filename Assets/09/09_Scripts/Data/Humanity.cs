using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Класс "человечество" - суммарная/усреднённая инфа по цивилизациям
/// </summary>
[System.Serializable]
public class Humanity {
    public SerializedDictionary<string, Civilization> civilizations;

    public int TotalEarthPop => civilizations.Sum(x => x.Value.Population) + MigrationController.Instance.Population;

    public void Init() {
        civilizations = new();
        civilizations.Clear();
        Debug.Log("Humanity init");
    }

    /// <summary>
    /// Создание первоначальной цивилизации (старт игры)
    /// </summary>
    public Civilization AddCivilization(TM_Region region, string civName) {
        Civilization newCiv = new Civilization();
        newCiv.Init(region, civName);
        civilizations[civName] = newCiv;
        return newCiv;
    }
}
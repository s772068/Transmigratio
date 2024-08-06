using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GlobalEvents = Events.Controllers.Global;

/// <summary>
/// Класс "человечество" - суммарная/усреднённая инфа по цивилизациям
/// </summary>
[System.Serializable]
public class Humanity {
    public SerializedDictionary<string, Civilization> Civilizations;
    public List<Gameplay.Scenarios.Base> scenarios;

    public int TotalEarthPop => Civilizations.Sum(x => x.Value.Population) + GlobalEvents.Migration.GetPopulation();

    public void Init() {
        Civilizations = new();
        Civilizations.Clear();
        Debug.Log("Humanity init");

        for(int i = 0; i < scenarios.Count; ++i) {
            scenarios[i].Init();
        }

        Timeline.TickLogic += Play;
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

    public void Play() {
        for (int i = 0; i < Civilizations.Count; ++i) {
            Civilizations.ElementAt(i).Value.Play();
        }
    }
}
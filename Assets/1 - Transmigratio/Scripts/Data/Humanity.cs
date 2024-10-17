using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GlobalEvents = Gameplay.Scenarios.Events.Global;

/// <summary>
/// Класс "человечество" - суммарная/усреднённая инфа по цивилизациям
/// </summary>
[System.Serializable]
public class Humanity {
    public int maxPopulation;
    public SerializedDictionary<string, Civilization> Civilizations;
    public List<Gameplay.Scenarios.Base> scenarios;

    [SerializeField] private SerializedDictionary<string, Sprite> _icons;

    public int TotalEarthPop => Civilizations.Sum(x => x.Value.Population) + GlobalEvents.Migration.GetPopulation();

    public Sprite GetIcon(string civName) => _icons[civName];

    public void Init() {
        Civilizations = new();
        Debug.Log("Humanity init");

        for(int i = 0; i < scenarios.Count; ++i) {
            scenarios[i].Init();
        }

        Timeline.TickLogic += Play;
    }
    ~Humanity()
    {
        Timeline.TickLogic -= Play;
    }

    /// <summary>
    /// Создание первоначальной цивилизации (старт игры)
    /// </summary>
    public Civilization StartGame(int region, string civName) {
        Civilization newCiv = new Civilization(civName);
        newCiv.StartGame(region);
        Civilizations[civName] = newCiv;
        return newCiv;
    }

    public void Play() {
        int count = Civilizations.Count;
        for (int i = 0; i < count; ++i) {
            Civilizations.ElementAt(i).Value.Play();
        }
    }

    public void RemovePiece(string civName, int region) {
        Civilizations[civName].RemovePiece(region);
        if(Civilizations[civName].Pieces.Count == 0)
            Civilizations.Remove(civName);
    }
}
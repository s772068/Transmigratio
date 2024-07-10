using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// ����� "������������" - ���������/���������� ���� �� ������������
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
    /// �������� �������������� ����������� (����� ����)
    /// </summary>
    public Civilization AddCivilization(int region, string civName) {
        Civilization newCiv = new Civilization();
        newCiv.Init(region, civName);
        Civilizations[civName] = newCiv;
        return newCiv;
    }
}
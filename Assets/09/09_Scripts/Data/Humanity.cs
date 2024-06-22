using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

/// <summary>
/// ����� "������������" - ���������/���������� ���� �� ������������
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
    /// �������� �������������� ����������� (����� ����)
    /// </summary>
    public Civilization AddCivilization(TM_Region region, string civName) {
        Civilization newCiv = new Civilization();
        newCiv.Init(region, civName);
        civilizations[civName] = newCiv;
        return newCiv;
    }
}
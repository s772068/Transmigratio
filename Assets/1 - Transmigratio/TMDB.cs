using Gameplay;
using System.Collections.Generic;
using UnityEngine;

//Не рефакторить
[CreateAssetMenu(fileName = "TMDB", menuName = "ScriptableObjects/TMDB", order = 1)]
public class TMDB : ScriptableObject {
    public int startAge;
    [SerializeField] private List<NewsSO> _news;
    public Humanity humanity = new();
    public Map map = new();

    public List<NewsSO> News => _news;

    public int Year => startAge - Timeline.Instance.Tick * GameSettings.YearsByTick;

    public Dictionary<string, float> GetParam(int index, string name) => name switch {
        "Flora" => map.AllRegions[index].Flora.GetValues(),
        "Fauna" => map.AllRegions[index].Fauna.GetValues(),
        "Climate" => map.AllRegions[index].Climate.GetValues(),
        "Terrain" => map.AllRegions[index].Terrain.GetValues(),
        "Civilizations" => map.AllRegions[index].GetCivParamiter(),
        "EcoCulture" => map.AllRegions[index].CivMain.EcoCulture.GetValues(),
        "ProdMode" => map.AllRegions[index].CivMain.ProdMode.GetValues(),
        "Government" => map.AllRegions[index].CivMain.Government.GetValues(),
        _ => default
    };

    public void TMDBInit() {
        humanity.Init();
        map.Init();
    }

    public void StartGame(int region) {
        Civilization civilization = humanity.StartGame(region, "Uncivilized");
        map.AllRegions[region].AddCivilization(civilization.Name);
    }
}
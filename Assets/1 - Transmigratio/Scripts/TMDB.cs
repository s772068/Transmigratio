using AYellowpaper.SerializedCollections;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;

//�� �����������
[CreateAssetMenu(fileName = "TMDB", menuName = "ScriptableObjects/TMDB", order = 1)]
public class TMDB : ScriptableObject {
    public int startAge;
    public int maxPopulation;
    public Map map = new();
    public Humanity humanity = new();
    [SerializeField] private List<NewsSO> _news;
    [SerializeField] private SerializedDictionary<string, SerializedDictionary<string, Color>> _paramiterColors;

    public List<NewsSO> News => _news;

    public int Year => startAge - Timeline.Instance.Tick * GameSettings.YearsByTick;

    public Color GetParamiterColor(string paramiter, string element) {
        return _paramiterColors[paramiter][element];
    }

    public Dictionary<string, float> GetParam(string paramiter) => paramiter switch {
        "Flora" => map.AllRegions[MapData.RegionID].Flora.GetValues(),
        "Fauna" => map.AllRegions[MapData.RegionID].Fauna.GetValues(),
        "Climate" => map.AllRegions[MapData.RegionID].Climate.GetValues(),
        "Terrain" => map.AllRegions[MapData.RegionID].Terrain.GetValues(),
        "Civilizations" => map.AllRegions[MapData.RegionID].GetCivParamiter(),
        "EcoCulture" => map.AllRegions[MapData.RegionID].CivMain.EcoCulture.GetValues(),
        "ProdMode" => map.AllRegions[MapData.RegionID].CivMain.ProdMode.GetValues(),
        "Government" => map.AllRegions[MapData.RegionID].CivMain.Government.GetValues(),
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
using System.Collections.Generic;
using UnityEngine;

//Не рефакторить
[CreateAssetMenu(fileName = "TMDB", menuName = "ScriptableObjects/TMDB", order = 1)]
public class TMDB : ScriptableObject {
    public Humanity humanity = new();
    public Map map = new();

    public Dictionary<string, int> GetParam(int index, string name) => name switch {
        "Flora" => map.AllRegions[index].Flora.GetQuantities(),
        "Fauna" => map.AllRegions[index].Fauna.GetQuantities(),
        "Climate" => map.AllRegions[index].Climate.GetQuantities(),
        "Terrain" => map.AllRegions[index].Terrain.GetQuantities(),
        "Civilizations" => map.AllRegions[index].GetCivParamiter(),
        "EcoCulture" => map.AllRegions[index].CivMain.EcoCulture.GetQuantities(),
        "ProdMode" => map.AllRegions[index].CivMain.ProdMode.GetQuantities(),
        "Government" => map.AllRegions[index].CivMain.Government.GetQuantities(),
        _ => default
    };

    public void TMDBInit() {
        humanity.Init();
        map.Init();
    }

    public void StartGame(int region) {
        Civilization civilization = humanity.AddCivilization(region, "Uncivilized");
        map.AllRegions[region].AddCivilization(civilization.Name);
    }

    /*
    public void MakeSaveFile(int tick)      // пока что записывает только данные по карте, нужно добавить данные по человечеству
    {
        string path;   
        string mapJson;
        string humanityJson;
        if (Application.isMobilePlatform)
        { path = "jar:file://" + Application.dataPath + "!/assets" + "/Ticks/tick" + tick + ".json"; }
        else
        { path = Application.streamingAssetsPath + "/Ticks/tick" + tick + ".json"; }

        mapJson = JsonConvert.SerializeObject(map.allRegions);
        
        if (File.Exists(path)) { File.Delete(path); }
        File.WriteAllText(path, mapJson);
    }
    */
}
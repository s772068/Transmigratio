using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TMDB", menuName = "ScriptableObjects/TMDB", order = 1)]
public class TMDB : ScriptableObject {
    public Humanity humanity = new();
    public Map map = new();

    public Dictionary<string, int> GetParam(int index, string name) => name switch {
        "Flora" => map.allRegions[index].flora.GetQuantities(),
        "Fauna" => map.allRegions[index].fauna.GetQuantities(),
        "Climate" => map.allRegions[index].climate.GetQuantities(),
        "Terrain" => map.allRegions[index].terrain.GetQuantities(),
        "Civilizations" => map.allRegions[index].GetCivParamiter(),
        "EcoCulture" => map.allRegions[index].CivMain.ecoCulture.GetQuantities(),
        "ProdMode" => map.allRegions[index].CivMain.prodMode.GetQuantities(),
        "Government" => map.allRegions[index].CivMain.government.GetQuantities(),
        _ => default
    };

    public void TMDBInit() {
        humanity.Init();
        map.Init();
    }

    public void StartGame(int region) {
        Civilization civilization = humanity.AddCivilization(region, "Uncivilized");
        map.allRegions[region].AddCivilization(civilization.name);
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
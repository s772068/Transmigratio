using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TMDB", menuName = "ScriptableObjects/TMDB", order = 1)]
public class TMDB : ScriptableObject {
    public Humanity humanity = new Humanity();
    public Map map = new Map();

    public Dictionary<string, int> GetParam(int index, string name) => name switch {
        "flora" => map.allRegions[index].flora.Quantities,
        "fauna" => map.allRegions[index].fauna.Quantities,
        "climate" => map.allRegions[index].climate.Quantities,
        "terrain" => map.allRegions[index].terrain.Quantities,
        "civilizations" => map.allRegions[index].GetCivParamiter(),
        "ecoCulture" => map.allRegions[index].CivMain.ecoCulture.Quantities,
        "prodMode" => map.allRegions[index].CivMain.prodMode.Quantities,
        "government" => map.allRegions[index].CivMain.government.Quantities,
        _ => default
    };

    public void TMDBInit() {
        humanity.Init();
        map.Init();
    }

    public void StartGame(TM_Region region) {
        Civilization civilization = humanity.AddCivilization(region, "unciv");
        region.civsList.Add(civilization);
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
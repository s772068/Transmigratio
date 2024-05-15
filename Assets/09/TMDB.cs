using UnityEngine;
using UnityEngine.InputSystem.HID;

[CreateAssetMenu(fileName = "TMDB", menuName = "ScriptableObjects/TMDB", order = 1)]
public class TMDB : ScriptableObject {
    public Humanity humanity = new Humanity();
    public Map map = new Map();
    public int tick;

    public void TMDBInit() {
        humanity.Init();
        map.Init();
        tick = 0;
    }

    public void StartGame(int regionIndex) {
        int civIndex = humanity.AddCivilization(regionIndex);
        map.StartGame(regionIndex, civIndex);
    }

    /*
    public void MakeSaveFile(int tick)      // ���� ��� ���������� ������ ������ �� �����, ����� �������� ������ �� ������������
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
    public void NextTick() {
        map.RefreshMap();
        Debug.Log("Tick: " + tick);
        tick++;
    }
}
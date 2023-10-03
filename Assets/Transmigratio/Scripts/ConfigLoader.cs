
using UnityEngine;
using System.IO;
//using Leguar.TotalJSON;

[System.Serializable]
public class ConfigLoader
{
    string pathTest = File.ReadAllText(Application.streamingAssetsPath + "/Config/testJson.json");
    string pathRegion = File.ReadAllText(Application.streamingAssetsPath + "/Config/RegionStartParams.json");


    public void Load()
    {
        
    }


}
[System.Serializable]
public class ParseClass
{
    public string s1;
    public string s2;
    public float f;
    public static ParseClass CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ParseClass>(jsonString);
    }

}
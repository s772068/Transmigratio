using UnityEngine;
using System.IO;

public static class IOHelper {
    public static void SaveToJson<T>(this T t) where T : struct {
        string jsonData = JsonUtility.ToJson(t);
        string path = Application.persistentDataPath + "/" + typeof(T) + ".json";
        File.WriteAllText(path, jsonData);
    }

    public static bool LoadFromJson<T>(out T owner) where T : struct {
        owner = default;
        string path = Application.persistentDataPath + "/" + typeof(T) + ".json";
        
        if (!File.Exists(path)) {
            Debug.LogWarning("Path is not founded: " + path);
            return false;
        }

        string jsonData = File.ReadAllText(path);

        if (jsonData == "") {
            Debug.LogWarning("File is empty: " + path);
            return false;
        }

        owner = JsonUtility.FromJson<T>(jsonData);
        return true;
    }
}

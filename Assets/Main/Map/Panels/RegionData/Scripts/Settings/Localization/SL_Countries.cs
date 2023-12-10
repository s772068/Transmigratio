using AYellowpaper.SerializedCollections;

[System.Serializable]
public struct SL_Countries {
    public string Name;
    public SerializedDictionary<string, string> Names;
}

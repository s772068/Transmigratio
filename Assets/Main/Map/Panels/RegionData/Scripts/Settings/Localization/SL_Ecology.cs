using AYellowpaper.SerializedCollections;

[System.Serializable]
public struct SL_Ecology {
    public string Name;
    public SerializedDictionary<string, SL_Paramiter> Paramiters;
}

using AYellowpaper.SerializedCollections;

[System.Serializable]
public struct SL_Paramiter {
    public string Name;
    public string Description;
    public SerializedDictionary<string, SL_Detail> Details;
}

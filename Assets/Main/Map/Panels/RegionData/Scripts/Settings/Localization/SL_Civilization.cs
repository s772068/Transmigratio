using AYellowpaper.SerializedCollections;

[System.Serializable]
public struct SL_Civilization {
    public string Name;
    public string Population;
    public string EmptyPopulation;
    public SerializedDictionary<string, SL_Paramiter> Paramiters;
}

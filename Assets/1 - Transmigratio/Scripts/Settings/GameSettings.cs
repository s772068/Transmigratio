using System.Collections.Generic;

public static class GameSettings {
    public static int StartPopulation = 1000;
    public static int StartHunters = 1;
    public static int StartPrimitiveCommunism = 1;
    public static int StartLeaderism = 1;
    public static int YearsByTick = 10;

    public static List<string> CivDetails = new() {
        "Government",
        "EcoCulture",
        "ProdMode",
        "Civilizations"
    };

    public static List<string> RegionDetails = new() {
        "Climate",
        "Terrain",
        "Flora",
        "Fauna"
    };
}
